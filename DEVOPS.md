# DevOps-vurdering: GitHub Actions, AWS og Terraform

Dette dokumentet er en **vurdering og læringsplan**, ikke en ferdig pipeline. Målet er å forklare hvor enkelt det er å legge til CI/CD med GitHub Actions og deploye til AWS, der **all AWS-infrastruktur styres med Terraform** (Infrastructure as Code). Du trenger ikke kunne DevOps fra før.

Kort konklusjon: **ja, dette er overkommelig for dette prosjektet**, fordi appen allerede kjører i Docker. Det som tar tid er ikke GitHub Actions-filen, men å forstå AWS-bitene og å holde kostnad og sikkerhet under kontroll.

---

## Hva du har i dag

Prosjektet er allerede ganske «deployment-klart»:

| Del | Teknologi | Status |
| --- | --- | --- |
| Frontend | Vue 3 + Vite | `norway_weather_ui/Dockerfile` |
| API | ASP.NET Core (.NET 10) | `norway_weather_api/Dockerfile` |
| Database | PostgreSQL 17 | `docker-compose.yml` |
| Lokal orkestrering | Docker Compose | API på 8080, UI på 3000, Postgres på 5432 |

Det finnes **ikke** GitHub Actions-workflows, **ikke** Terraform-kode og **ikke** AWS-oppsett ennå. Det er forventet. CI/CD og sky kommer som et eget lag utenpå det du har.

Hvorfor Docker hjelper: GitHub Actions kan bygge de samme image-ene som du kjører lokalt, og AWS kan kjøre de samme image-ene. Du slipper å finne opp en ny «produksjonsmåte» for selve appen.

---

## Hva de tre verktøyene gjør (og hva de *ikke* gjør)

Tenk på tre roller. Bland dem ikke:

1. **Terraform** — beskriver *hvilke AWS-ting som skal finnes*: nettverk, database, container-tjeneste, lastbalanserer, IAM-roller. Du endrer `.tf`-filer, kjører `terraform plan` / `apply`, og AWS oppdateres. **AWS-ressurser skal ikke klikkes sammen i konsollen som hovedløsning.** Konsollen er til å *se* og *debugge*, ikke til å eie sannheten.
2. **GitHub Actions** — kjører automatikk når du pusher kode: bygg, test, bygg Docker-image, pushe image, og si til AWS «kjør ny versjon». Den deployer *appen*, den eier ikke infrastrukturen.
3. **AWS** — der ting faktisk kjører. Du snakker med AWS via Terraform (infrastruktur) og via Actions (deploy av image/oppdatering av tjeneste).

Typisk flyt, **splittet på GitHub-branch**:

```text
feature-branch ──PR──►  dev  ──PR──►  main
                          │              │
                          ▼              ▼
                   GitHub Actions   GitHub Actions
                   deploy til AWS   deploy til AWS
                   miljø: dev       miljø: prod
```

**Ikke** la GitHub Actions opprette VPC, RDS og ECS fra bunnen av med `aws`-kommandoer. Da blir skyen uleselig. Terraform eier ressursene. Actions eier releasen.

---

## Hvor enkelt er det, ærlig talt?

På en skala der 1 er «en YAML-fil og ferdig» og 10 er «full produksjonsplattform»:

| Oppgave | Vanskelighet | Kommentar |
| --- | --- | --- |
| GitHub Actions som bare bygger og tester | 2/10 | YAML i `.github/workflows/`. God nybegynnerstart. |
| Bygge Docker-image og pushe til Amazon ECR | 4/10 | Standard mønster, mye ferdige actions. |
| Terraform for *noe* på AWS (f.eks. ECR + S3) | 4/10 | Krever at du forstår state, plan/apply og at du ikke commiter secrets. |
| Full app i sky: API + UI + Postgres + nettverk | 7/10 | VPC, security groups, RDS, ECS, lastbalanserer, secrets. Dette er «ekte» DevOps. |
| Trygg produksjon (OIDC, `dev`/`main`, logger, backup, kostnad) | 8/10 | Ikke dag 1. Kom etter at **dev**-miljøet virker. |

**GitHub Actions + direkte AWS-deploy er altså enkelt å *starte*, men ikke trivielt å *gjøre skikkelig*.** For denne appen er flaskehalsen AWS-nettverk og database, ikke YAML.

Realistisk tid hvis du er ny og følger videoene under. Bygg **dev først**, prod som kopi etterpå:

- Uke 1: forstå CI vs CD, grenene `dev`/`main`, Terraform-grunnlag, AWS-konto og billing-alarm.
- Uke 2: Terraform for VPC + ECR + RDS i **dev**-miljøet.
- Uke 3: ECS Fargate for API, S3/CloudFront for UI, Actions som deployer fra `dev`.
- Senere: samme stack i **prod**, trigget fra `main`.

Ikke prøv alt i én kveld.

---

## Anbefalt arkitektur for *denne* appen

Du har tre ulike ting. De hører hjemme på ulike AWS-tjenester.

### 1. Frontend (Vue/Vite)

Bygg statiske filer med `npm run build` (`dist/`). Legg dem på **S3** og server dem via **CloudFront**.

Hvorfor: UI-en er en SPA, ikke en langvarig serverprosess. Du trenger ikke en container 24/7 til Vue. Det blir billigere og enklere enn å kjøre UI-containeren i ECS.

GitHub Actions: `npm ci` → `npm run build` → last opp `dist/` til S3 → invalider CloudFront-cache.

`VITE_API_BASE_URL` må peke på det **offentlige API-domenet for det miljøet du bygger**, ikke `http://localhost:8080/api`. Bygg mot `dev`-API når du deployer `dev`, og mot `prod`-API når du deployer `main`.

### 2. API (ASP.NET)

Kjør Docker-imaget på **Amazon ECS med Fargate** bak en **Application Load Balancer (ALB)**.

Hvorfor Fargate og ikke EC2: du slipper å administrere virtuelle maskiner. Du sier «kjør denne containeren», AWS kjører den.

Alternativer du kan høre om, og hvorfor de *ikke* er førstevalg her:

- **Elastic Beanstalk** — enklere klikk, men passer dårlig med «alt i Terraform» og blir fort en svart boks.
- **App Runner** — veldig enkelt for én container, men du får mindre kontroll, og Postgres + nettverk blir fort knotete.
- **EKS (Kubernetes)** — overkill. Ikke start her.
- **Rå EC2 + docker compose på en VM** — fristende fordi det ligner lokalt, men da eier du patching, SSH og «serveren som snøfnugg». Unngå som hovedløsning.

### 3. PostgreSQL

Bruk **Amazon RDS for PostgreSQL**, ikke Postgres-container i Compose på AWS.

Hvorfor: backup, patching, disk og omstart er RDS sitt problem. En Postgres-container på Fargate uten skikkelig disk/backup er en felle.

Tilkoblingsstreng (host, user, password) skal ligge i **AWS Secrets Manager** eller **SSM Parameter Store**, ikke i git. API-containeren leser den ved oppstart.

### 4. Image-lager

**Amazon ECR** lagrer `norway_weather_api`-imaget. Actions bygger og pusher. ECS henter derfra.

### 5. Nettverk og tilgang (det som føles vanskeligst)

Terraform bør opprette omtrent dette:

- VPC med public og private subnets i minst to availability zones
- ALB i public subnet (internett treffer API her)
- ECS-oppgaver i private subnet
- RDS i private subnet (ikke åpen `0.0.0.0/0` på port 5432)
- Security groups: ALB → API på 8080, API → RDS på 5432
- IAM-roller: ECS-task role, GitHub OIDC-role (Actions får lov til ECR + ECS uten langlivede nøkler)

Dette er grunnen til at totalvurderingen er 7/10, ikke 3/10. Selve ideen er enkel. AWS-nettverk er det du må lære.

Samme mønster **to ganger**: ett sett ressurser for **dev**, ett for **prod** (egne navn, egen database, egen ALB, egen S3/CloudFront). De deler region og konto, ikke data.

---

## Branch-basert deploy: `dev` og `main`

Deploy skal **ikke** gå til ett felles AWS-miljø. GitHub-grenene styrer hvilket miljø som oppdateres.

| GitHub-branch | AWS-miljø | Når det deployer | Hva det er |
| --- | --- | --- | --- |
| `dev` | **dev** (staging) | Push/merge til `dev` | Der du tester i skyen. Kan være ødelagt. Egen database. |
| `main` | **prod** | Push/merge til `main` | Det brukerne treffer. Bare kode som allerede har vært på `dev`. |

Arbeidsflyt i git:

1. Jobb på en feature-branch (`feature/...`).
2. Åpne PR inn mot **`dev`**. CI må være grønn.
3. Merge til `dev` → Actions deployer til **AWS dev**.
4. Når det ser bra ut i dev: PR fra **`dev` inn mot `main`**.
5. Merge til `main` → Actions deployer til **AWS prod**.

Regler som gjør dette verdt det:

- **`main` er beskyttet.** Ingen direkte push. Bare PR, med CI som må passere. Helst at PR-en kommer fra `dev`, ikke fra en tilfeldig feature-branch.
- **`dev` er også beskyttet** mot force-push, men kan ta feature-PRs.
- Prod skal aldri få kode som ikke har vært gjennom `dev`.
- Dev og prod har **ikke** samme RDS. Testdata i dev skal ikke kunne ødelegge prod.
- Terraform bruker variabel `environment` (`dev` / `prod`) og **to state-filer** (f.eks. `env:/dev/terraform.tfstate` og `env:/prod/terraform.tfstate`, eller to nøkler i S3). Én apply treffer ett miljø.
- GitHub Environments: `dev` og `prod`. OIDC-rollen for `prod` skal bare kunne antas fra `main`. Rollen for `dev` bare fra `dev`. Da kan ikke en workflow på `dev` skrive til prod-ECS ved en feil.
- Prod-deploy: start med **manuell godkjenning** på GitHub Environment `prod` (required reviewers). Dev kan auto-deploye.

Dette dobler AWS-kostnad sammenlignet med ett miljø. Det er prisen for å ikke teste i produksjon. I lærefasen kan prod vente til dev-pipen virker — men *modellen* skal være to grener fra starten, så du ikke bygger alt rundt «bare `main`».

---

## Hvordan GitHub Actions bør se ut (konsept)

Du trenger ikke én mega-workflow. To (eller tre) er ryddigere.

### CI (hver pull request inn mot `dev` eller `main`)

Kjør alltid, også før AWS finnes:

- UI: `npm ci`, `npm run build` (senere: lint/test når du har det)
- API: `dotnet restore`, `dotnet build`, `dotnet test` (når du har tester)

Dette er **det enkleste og mest verdifulle første steget**. Det koster nesten ingenting og fanger ødelagte bygg.

### CD (deploy splittet på branch)

Samme steg, ulike AWS-mål:

| Trigger | GitHub Environment | Terraform `environment` | ECR/ECS/S3 |
| --- | --- | --- | --- |
| Push til `dev` | `dev` | `dev` | dev-ressursene |
| Push til `main` | `prod` | `prod` | prod-ressursene |

Etter at Terraform har laget skyen:

1. Logg inn på AWS med **OIDC** mot *den* rollen som matcher branchen (ikke `AWS_ACCESS_KEY_ID` som langvarig secret).
2. Bygg API-image, tag med git SHA (og gjerne `dev` / `prod`), push til **det** miljøets ECR — eller ett ECR med tags `dev-...` / `prod-...`. Enklest for nybegynner: **ett ECR-repo per miljø**.
3. Oppdater **det** miljøets ECS-tjeneste til ny image-tag.
4. Bygg UI med riktig `VITE_API_BASE_URL`, last opp til **det** miljøets S3, invalider **det** miljøets CloudFront.

Én workflow-fil kan gjøre begge deler med `if: github.ref == 'refs/heads/dev'` / `'refs/heads/main'`, eller to filer (`deploy-dev.yml`, `deploy-prod.yml`). To filer er lettere å lese når du er ny.

### Terraform-jobb (egen workflow eller manuelt først)

I starten: kjør Terraform **på din maskin** (`plan` / `apply`) mot **dev** til du forstår det.

Når det sitter:

- PR som endrer `infra/`: `terraform plan` mot **dev** (og mot prod som «hva *ville* skjedd», uten apply).
- Apply til **dev** ved merge til `dev` (eller manuelt første tiden).
- Apply til **prod** bare fra `main`, med manuell godkjenning. Aldri `apply` i blinde mot produksjon.

---

## Terraform: hvordan AWS skal eies

Regler som sparer deg for smerte:

- Én mappe, f.eks. `infra/`, med `.tf`-filer. Ingen ressurser «ved siden av» i konsollen som Terraform ikke kjenner.
- **Remote state** i S3 + lås i DynamoDB (eller S3 native lock når du setter det opp), slik at state ikke ligger bare på laptop og ikke commites.
- Variabler for region, miljønavn (`dev` / `prod`), instance-størrelser. Dev kan være mindre (f.eks. `db.t4g.micro`, 0.25 vCPU på Fargate). Prod kan skaleres opp senere.
- Outputs: ALB-URL, CloudFront-URL, ECR-URI — slikt Actions og du trenger.
- Secrets: aldri i `.tf` som plaintext. Bruk Secrets Manager, eller Terraform som *oppretter* secret-ressursen mens verdien settes utenfor git.
- `.gitignore` må ignorere `.terraform/`, `*.tfstate`, `*.tfvars` med hemmeligheter.

Første Terraform-mål, i denne rekkefølgen:

1. S3-bucket for Terraform state
2. ECR-repo
3. VPC + subnets + security groups
4. RDS Postgres (liten instance, f.eks. `db.t4g.micro`, i **dev**)
5. ALB + ECS-cluster + Fargate-service + task definition
6. S3 + CloudFront for UI
7. IAM OIDC-provider for GitHub + **to roller** (Actions på `dev` antar `dev`-rollen, Actions på `main` antar `prod`-rollen)

Punkt 1–2 kan du gjøre som «hello world». Punkt 3–5 er selve løftet.

---

## Sikkerhet og kostnad (les dette før du lager konto)

- Sett **billing alarm** i AWS dag 0. NAT Gateway, ALB og RDS koster også når appen er «liten».
- Bruk **én region**, f.eks. `eu-north-1` (Stockholm) fordi appen er Norge-orientert.
- **dev** først (branch `dev`). **prod** (branch `main`) kommer etterpå. To miljøer dobler kostnad — forvent det.
- GitHub Actions skal bruke **OIDC mot AWS**, ikke permanente access keys i GitHub Secrets.
- RDS skal ikke være publicly accessible i produksjon.
- `.env` og databasepassord skal aldri inn i git (du har allerede `.env` i `.gitignore` — behold det slik).
- Fargate + ALB + RDS er sjelden «gratis». For læring: slå av / `terraform destroy` når du ikke øver, *etter* at du vet at destroy er trygt på **dev**. Kjør aldri destroy mot prod «for å øve».

---

## Hva som er *enkelt* vs det som lurer deg

**Enkelt**

- Legge til CI som bygger UI og API
- Dockerfile-ene du allerede har
- Push til ECR
- «Push til `dev` → AWS dev, push til `main` → AWS prod» som idé

**Det som lurer nybegynnere**

- VPC og «hvorfor får ikke containeren snakket med databasen»
- CORS og HTTPS: UI på CloudFront, API på ALB, ulike domener
- Vite baker inn `VITE_*` ved *build-tid*, ikke runtime
- Terraform state som blir borte eller committet
- ECS «service unstable» fordi health check treffer feil path/port
- Overraskende AWS-regning (NAT Gateway er klassikeren; to miljøer = to ganger)
- Å deploye feature-brancher rett til prod og hoppe over `dev`
- Å klikke i konsollen «bare denne ene gangen» og så matcher ikke Terraform lenger

---

## Læringssti (YouTube og rekkefølge)

Se videoene **i denne rekkefølgen**. Ikke hopp rett til «full CI/CD til ECS» før Terraform-begrepene sitter. Du trenger ikke betalte kurs til å komme i gang.

### Steg 0 — hva CI/CD er (30–45 min)

Søk og se en kort intro om CI vs CD, f.eks. TechWorld with Nana: *CI/CD Pipeline explained* (søk tittelen på YouTube). Du skal sitte igjen med:

- CI = automatisk bygg/test på hver endring
- CD = automatisk (eller knappestyrt) utrulling
- Pipeline = jobb som GitHub kjører i skyen, ikke på PC-en din

### Steg 1 — GitHub Actions (1–2 timer)

- Offisiell GitHub: søk **«GitHub Actions tutorial»** og ta den korte «first workflow»-videoen.
- Deretter en video som viser `on: pull_request`, `jobs`, `steps`, `uses:` vs `run:`.

Øv i *dette* repoet: en workflow som bare bygger UI og API. Ingen AWS ennå. Det er den beste første seieren.

### Steg 2 — Terraform-begreper (1–2 timer)

- [Terraform explained in 15 mins | Terraform Tutorial for Beginners](https://www.youtube.com/watch?v=l5k1ai_GBDE) — TechWorld with Nana. Se denne først. Den forklarer *hva* Terraform er, declarative vs imperative, og `init` / `plan` / `apply`.
- Deretter en hands-on «first AWS resource with Terraform» (ECR eller S3). Unngå Kubernetes-eksempler.

Du skal kunne forklare: provider, resource, state, at `plan` er ufarlig og `apply` endrer skyen, at `destroy` sletter det Terraform eier.

### Steg 3 — AWS-byggeklosser uten å miste deg selv (2–4 timer)

Ikke se «komplett AWS-sertifisering» ennå. Se korte videoer om **akkurat** disse, én om gangen:

1. VPC, subnet, security group (hva som er offentlig vs privat)
2. ECR (Docker-registry hos AWS)
3. ECS Fargate + task vs service (container som kjører)
4. ALB (HTTPS inn, videresend til container)
5. RDS PostgreSQL (administrert database)

AWS har egne «What is Amazon ECS / RDS / VPC»-snutter. De er kjedelige og nyttige. Skip EKS, Lambda-tunge arkitekturer og «serverless for alt» til senere.

### Steg 4 — lime det sammen (2–3 timer)

Når du kjenner brikkene:

- [Set Up CI/CD to Auto Deploy to AWS ECS (GitHub + GitLab)](https://www.youtube.com/watch?v=gfCkScWWTvk) — mønsteret «bygg image → ECR → oppdater ECS».
- [CI/CD with Terraform & GitHub Actions](https://www.youtube.com/watch?v=D0w_1a3fYhM) — viser Terraform + Actions i samme prosjekt. Arkitekturen der (EC2/Node) er **ikke** 1:1 med denne appen, men flyten «IaC eier sky, Actions deployer app» er den du skal kopiere mentalt.

Les også GitHub-docs (ikke video, men viktig): [Configuring OpenID Connect in Amazon Web Services](https://docs.github.com/en/actions/how-tos/secure-your-work/security-harden-deployments/oidc-in-aws). Det er den riktige måten for Actions å snakke med AWS på.

### Hva du skal *unngå* å se først

- 8–20 timers «Complete DevOps bootcamp» som hopper til Kubernetes dag 2
- Videoer som deployer med langlivede AWS-nøkler og sier «bare lim inn i Secrets»
- «ClickOps»: 40 minutter med pek-og-klikk i AWS Console uten Terraform
- Terraform-kurs som bruker Google Cloud eller Azure når du har valgt AWS (konseptene ligner, detaljene stjeler fokus)

### Norske vs engelske videoer

De beste, oppdaterte tutorialene er nesten alltid på engelsk. Det er greit. Begrepene (`state`, `task definition`, `security group`) er de samme i jobben senere. Bruk norsk til å notere *hva du forstod* i dette dokumentet eller egne notater, og engelsk til å lære verktøyet.

---

## Foreslått rekkefølge *i dette repoet* (når du er klar til å bygge)

Ikke implementer alt på en gang. Hvert steg skal virke alene.

1. **CI uten sky** — `.github/workflows/ci.yml` som bygger UI og API.
2. **AWS-konto + billing alarm + IAM-bruker/rolle kun til din laptop** for Terraform lokalt.
3. **Beskytt `dev` og `main` på GitHub.** Feature-brancher går inn i `dev`; `dev` går inn i `main`.
4. **`infra/` med Terraform** — state-backend, så ECR, så VPC, så RDS — først med `environment = "dev"`.
5. **Kjør API mot RDS manuelt** én gang i **dev** (forstå connection string og security groups).
6. **ECS + ALB i dev** — API svarer på en offentlig dev-URL.
7. **S3 + CloudFront i dev** — UI lastes, kaller dev-API.
8. **GitHub OIDC + CD** — push til `dev` ruller ut til AWS **dev**.
9. Når det er stabilt: gjenta Terraform for `environment = "prod"`, CD på `main`, manuell godkjenning, egne hemmeligheter og egen database.
10. Først da: custom domener (f.eks. `dev.example.com` / `example.com`) og HTTPS (ACM).

Punkt 1 kan du gjøre uten å bruke en krone på AWS. Det er anbefalt start.

---

## Oppsummering

| Spørsmål | Svar |
| --- | --- |
| Er GitHub Actions CI/CD «enkelt å legge til»? | **Ja for bygg/test. Middels for deploy.** YAML er den lette delen. |
| Skal AWS styres i konsollen? | **Nei.** Terraform er kilden til sannhet for ressurser. |
| Skal Actions lage infrastruktur? | **Nei.** Actions bygger og deployer appen oppå det Terraform har laget. |
| Hvordan splittes deploy? | Branch **`dev`** → AWS **dev**. Branch **`main`** → AWS **prod**. Feature-brancher deployer ikke. |
| Passer dette prosjektet? | **Ja.** Docker + delt API/UI/Postgres er et klassisk, lærbart oppsett. Kubernetes trengs ikke. |
| Hva bør du lære først? | CI i GitHub Actions, grenflyt `dev`/`main`, så Terraform-intro (Nana), så VPC/ECS/RDS, så lim sammen med OIDC. |
| Største risiko? | Kostnad (to miljøer), åpen database, nøkler i git, deploy til prod utenom `dev`, og å klikke i AWS uten å oppdatere Terraform. |

Når du vil gå fra denne vurderingen til ekte filer, er naturlig første PR: en CI-workflow som bare bygger — fortsatt uten AWS.

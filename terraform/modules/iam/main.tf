
data "aws_iam_policy_document" "ecs_task_assume_role" {
  statement {
    effect  = "Allow"
    actions = ["sts:AssumeRole"]

    principals {
      type        = "Service"
      identifiers = ["ecs-tasks.amazonaws.com"]
    }
  }
}

resource "aws_iam_role" "ecs_task_execution" {
  name               = "${var.project_name}-${var.environment}-ecs-execution-role"
  assume_role_policy = data.aws_iam_policy_document.ecs_task_assume_role.json

  tags = {
    Project     = var.project_name
    Environment = var.environment
  }
}

resource "aws_iam_role_policy_attachment" "ecs_task_execution" {
  role       = aws_iam_role.ecs_task_execution.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy"
}

resource "aws_iam_role" "ecs_task" {
  name               = "${var.project_name}-${var.environment}-ecs-task-role"
  assume_role_policy = data.aws_iam_policy_document.ecs_task_assume_role.json

  tags = {
    Project     = var.project_name
    Environment = var.environment
  }
}

resource "aws_iam_role_policy" "ecs_task_execution_secrets" {
  name = "${var.project_name}-${var.environment}-secrets-access"
  role = aws_iam_role.ecs_task_execution.id

  policy = jsonencode({
    Version = "2012-10-17"

    Statement = [
      {
        Sid    = "AllowReadDatabaseAndJWTSecrets"
        Effect = "Allow"

        Action = [
          "secretsmanager:GetSecretValue"
        ]

        Resource = [
          var.db_secret_arn,
          var.jwt_secret_arn,
          var.postgres_connection_string_secret_arn,
          var.metno_contact_email_secret_arn,
          var.cloudfront_domain_secret_arn
        ]
      }
    ]
  })
}

resource "aws_iam_role_policy" "github_actions_ecr_push" {
  name = "github-actions-ecr-push"
  role = "GitHubActionsCICDAssumeRole"

  policy = jsonencode({
    Version = "2012-10-17"

    Statement = [
      {
        Sid      = "ECRAuthorizationToken"
        Effect   = "Allow"
        Action   = ["ecr:GetAuthorizationToken"]
        Resource = "*"
      },
      {
        Sid    = "ECRPushImage"
        Effect = "Allow"

        Action = [
          "ecr:BatchCheckLayerAvailability",
          "ecr:InitiateLayerUpload",
          "ecr:UploadLayerPart",
          "ecr:CompleteLayerUpload",
          "ecr:PutImage"
        ]

        Resource = "arn:aws:ecr:eu-central-1:714781437459:repository/norway-weather-app-development-backend"
      },
      {
        Sid      = "DescribeBackendTaskDefinition"
        Effect   = "Allow"
        Action   = ["ecs:DescribeTaskDefinition"]
        Resource = "*"
      },
      {
        Sid      = "RegisterBackendTaskDefinition"
        Effect   = "Allow"
        Action   = ["ecs:RegisterTaskDefinition"]
        Resource = "arn:aws:ecs:eu-central-1:714781437459:task-definition/norway-weather-app-development-backend:*"
      },
      {
        Sid      = "UpdateBackendService"
        Effect   = "Allow"
        Action   = ["ecs:UpdateService"]
        Resource = "arn:aws:ecs:eu-central-1:714781437459:service/norway-weather-app-development-cluster/norway-weather-app-development-backend"
      },
      {
        Sid      = "DescribeBackendServiceForMigration"
        Effect   = "Allow"
        Action   = ["ecs:DescribeServices"]
        Resource = "arn:aws:ecs:eu-central-1:714781437459:service/norway-weather-app-development-cluster/norway-weather-app-development-backend"
      },
      {
        Sid      = "RunBackendMigrationTask"
        Effect   = "Allow"
        Action   = ["ecs:RunTask"]
        Resource = "arn:aws:ecs:eu-central-1:714781437459:task-definition/norway-weather-app-development-backend:*"

        Condition = {
          ArnEquals = {
            "ecs:cluster" = "arn:aws:ecs:eu-central-1:714781437459:cluster/norway-weather-app-development-cluster"
          }
        }
      },
      {
        Sid      = "DescribeBackendMigrationTasks"
        Effect   = "Allow"
        Action   = ["ecs:DescribeTasks"]
        Resource = "arn:aws:ecs:eu-central-1:714781437459:task/norway-weather-app-development-cluster/*"
      },
      {
        Sid    = "PassEcsTaskRoles"
        Effect = "Allow"
        Action = ["iam:PassRole"]
        Resource = [
          "arn:aws:iam::714781437459:role/norway-weather-app-development-ecs-execution-role",
          "arn:aws:iam::714781437459:role/norway-weather-app-development-ecs-task-role"
        ]
        Condition = {
          StringLike = {
            "iam:PassedToService" = "ecs-tasks.amazonaws.com"
          }
        }
      }
    ]
  })
}

data "aws_iam_policy_document" "github_actions_assume_role" {
  statement {
    effect  = "Allow"
    actions = ["sts:AssumeRoleWithWebIdentity"]

    principals {
      type = "Federated"
      identifiers = [
        "arn:aws:iam::714781437459:oidc-provider/token.actions.githubusercontent.com"
      ]
    }

    condition {
      test     = "StringEquals"
      variable = "token.actions.githubusercontent.com:aud"
      values   = ["sts.amazonaws.com"]
    }

    condition {
      test     = "StringLike"
      variable = "token.actions.githubusercontent.com:sub"
      values   = [
        "repo:nilsndev@99693030/norway_weather_app@1415276526:ref:refs/heads/main"
      ]
    }
  }
}

resource "aws_iam_role" "github_actions" {
  name = "GitHubActionsCICDAssumeRole"

  assume_role_policy = data.aws_iam_policy_document.github_actions_assume_role.json
}

resource "aws_iam_role_policy" "github_actions_frontend_deploy" {
  name = "github-actions-frontend-deploy"
  role = "GitHubActionsCICDAssumeRole"

  policy = jsonencode({
    Version = "2012-10-17"

    Statement = [
      {
        Sid    = "ListFrontendBucket"
        Effect = "Allow"

        Action = [
          "s3:ListBucket",
          "s3:GetBucketLocation",
          "s3:ListBucketMultipartUploads"
        ]

        Resource = "arn:aws:s3:::norway-weather-app-development-frontend"
      },
      {
        Sid    = "UploadFrontendFiles"
        Effect = "Allow"

        Action = [
          "s3:PutObject",
          "s3:DeleteObject",
          "s3:AbortMultipartUpload",
          "s3:ListMultipartUploadParts"
        ]

        Resource = "arn:aws:s3:::norway-weather-app-development-frontend/*"
      },
      {
        Sid      = "InvalidateCloudFrontCache"
        Effect   = "Allow"
        Action   = ["cloudfront:CreateInvalidation"]
        Resource = "arn:aws:cloudfront::714781437459:distribution/E19LXMKM87QLG2"
      }
    ]
  })
}
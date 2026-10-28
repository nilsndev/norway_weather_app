module "networking" {
  source = "../../modules/networking"

  project_name = var.project_name
  environment  = var.environment
}

module "iam" {
  source = "../../modules/iam"

  project_name = var.project_name
  environment  = var.environment

  jwt_secret_arn = var.jwt_secret_arn
  db_secret_arn  = module.database.db_secret_arn
  postgres_connection_string_secret_arn = var.postgres_connection_string_secret_arn
  metno_contact_email_secret_arn = var.metno_contact_email_secret_arn
}

module "ecr" {
  source = "../../modules/ecr"

  project_name = var.project_name
  environment  = var.environment
}

module "database" {
  source = "../../modules/database"

  project_name = var.project_name
  environment  = var.environment

  db_subnet_ids         = module.networking.db_subnet_ids
  rds_security_group_id = module.networking.rds_security_group_id
}

module "alb" {
  source = "../../modules/alb"

  project_name = var.project_name
  environment  = var.environment

  vpc_id                = module.networking.vpc_id
  public_subnet_ids     = module.networking.public_subnet_ids
  alb_security_group_id = module.networking.alb_security_group_id
}

module "ecs" {
  source = "../../modules/ecs"

  project_name = var.project_name
  environment  = var.environment
  aws_region   = var.aws_region

  backend_repository_url = module.ecr.backend_repository_url
  image_tag              = var.image_tag

  ecs_task_execution_role_arn = module.iam.ecs_task_execution_role_arn
  ecs_task_role_arn           = module.iam.ecs_task_role_arn

  app_subnet_ids        = module.networking.app_subnet_ids
  ecs_security_group_id = module.networking.ecs_security_group_id

  backend_target_group_arn = module.alb.backend_target_group_arn

  db_endpoint   = module.database.db_endpoint
  db_port       = module.database.db_port
  db_name       = module.database.db_name
  db_secret_arn = module.database.db_secret_arn

  jwt_secret_arn = var.jwt_secret_arn
  postgres_connection_string_secret_arn = var.postgres_connection_string_secret_arn
  metno_contact_email_secret_arn = var.metno_contact_email_secret_arn
}


module "frontend" {
  source = "../../modules/frontend"

  project_name = var.project_name
  environment  = var.environment
}




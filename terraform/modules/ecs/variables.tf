
variable "project_name" {
  description = "Name of the project"
  type        = string
}

variable "environment" {
  description = "Deployment environment"
  type        = string
}

variable "aws_region" {
  description = "AWS region"
  type        = string
  default     = "eu-central-1"
}

variable "backend_repository_url" {
  description = "URL of the backend ECR repository"
  type        = string
}

variable "image_tag" {
  description = "Unique Docker image tag already pushed to ECR"
  type        = string
}

variable "ecs_task_execution_role_arn" {
  description = "ARN of the ECS task execution role"
  type        = string
}

variable "ecs_task_role_arn" {
  description = "ARN of the ECS task role"
  type        = string
}

variable "app_subnet_ids" {
  description = "Private application subnet IDs"
  type        = list(string)
}

variable "ecs_security_group_id" {
  description = "Security group ID for ECS tasks"
  type        = string
}

variable "backend_target_group_arn" {
  description = "ALB target group ARN for the backend"
  type        = string
}

variable "db_endpoint" {
  description = "RDS endpoint address, without port"
  type        = string
}

variable "db_port" {
  description = "RDS PostgreSQL port"
  type        = number
  default     = 5432
}

variable "db_name" {
  description = "PostgreSQL database name"
  type        = string
}

variable "db_secret_arn" {
  description = "ARN of the RDS-managed master credentials secret"
  type        = string
}

variable "jwt_secret_arn" {
  description = "ARN of the JWT secret in Secrets Manager"
  type        = string
}

variable "postgres_connection_string_secret_arn" {
  description = "ARN of the PostgreSQL connection string in AWS Secrets Manager"
  type        = string
}

variable "metno_contact_email_secret_arn" {
  description = "ARN of the MET Norway contact email secret in AWS Secrets Manager"
  type        = string
}

variable "task_cpu" {
  description = "Fargate task CPU units"
  type        = number
  default     = 256
}

variable "task_memory" {
  description = "Fargate task memory in MiB"
  type        = number
  default     = 512
}

variable "desired_count" {
  description = "Desired number of backend tasks"
  type        = number
  default     = 1
}

variable "cloudfront_domain_secret_arn" {
  description = "ARN of the Secrets Manager secret containing the CloudFront domain"
  type        = string
}
variable "aws_region" {
  description = "AWS region"
  type        = string
}

variable "project_name" {
  description = "Project name"
  type        = string
}

variable "environment" {

  description = "Deployment environment"
  type        = string
}

variable "image_tag" {
  description = "Taggen til backend Docker-imaget som allerede er pushet til ECR"
  type        = string
}

variable "jwt_secret_arn" {
  description = "ARN til JWT-secret i AWS Secrets Manager"
  type        = string
}

variable "postgres_connection_string_secret_arn" {
  description = "ARN of the PostgreSQL connection string in AWS Secrets Manager"
  type        = string
}
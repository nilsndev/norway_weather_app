
variable "project_name" {
  description = "Name of the project"
  type        = string
}

variable "environment" {
  description = "Deployment environment, for example development"
  type        = string
}

variable "jwt_secret_arn" {
  description = "ARN of the JWT secret in Secrets Manager"
  type        = string
}

variable "db_secret_arn" {
  description = "ARN of the RDS-managed master credentials secret"
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

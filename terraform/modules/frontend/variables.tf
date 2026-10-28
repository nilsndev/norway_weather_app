
variable "project_name" {
  description = "Name of the project"
  type        = string
}

variable "environment" {
  description = "Deployment environment"
  type        = string
}

variable "alb_dns_name" {
  description = "DNS name of the ALB used as the API origin"
  type        = string
}

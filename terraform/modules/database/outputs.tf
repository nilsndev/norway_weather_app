
output "db_endpoint" {
  description = "RDS database endpoint"
  value       = aws_db_instance.main.address
}

output "db_port" {
  description = "PostgreSQL port"
  value       = aws_db_instance.main.port
}

output "db_name" {
  description = "Name of the PostgreSQL database"
  value       = aws_db_instance.main.db_name
}

output "db_secret_arn" {
  description = "ARN of the RDS-managed master credentials secret"
  value       = aws_db_instance.main.master_user_secret[0].secret_arn
}

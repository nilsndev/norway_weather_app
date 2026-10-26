
output "frontend_bucket_name" {
  description = "Name of the S3 bucket for frontend files"
  value       = aws_s3_bucket.frontend.bucket
}

output "cloudfront_distribution_id" {
  description = "ID of the CloudFront distribution"
  value       = aws_cloudfront_distribution.frontend.id
}

output "cloudfront_domain_name" {
  description = "Default domain name of the CloudFront distribution"
  value       = aws_cloudfront_distribution.frontend.domain_name
}

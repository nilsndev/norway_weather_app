resource "aws_ecs_cluster" "main" {
  name = "${var.project_name}-${var.environment}-cluster"

  tags = {
    Project     = var.project_name
    Environment = var.environment
  }
}

resource "aws_cloudwatch_log_group" "backend" {
  name              = "/ecs/${var.project_name}-${var.environment}-backend"
  retention_in_days = 14

  tags = {
    Project     = var.project_name
    Environment = var.environment
  }
}

resource "aws_ecs_task_definition" "backend" {
  family                   = "${var.project_name}-${var.environment}-backend"
  requires_compatibilities = ["FARGATE"]
  network_mode             = "awsvpc"

  cpu    = var.task_cpu
  memory = var.task_memory

  execution_role_arn = var.ecs_task_execution_role_arn
  task_role_arn      = var.ecs_task_role_arn

  container_definitions = jsonencode([
    {
      name      = "backend"
      image     = "${var.backend_repository_url}:${var.image_tag}"
      essential = true

      portMappings = [
        {
          containerPort = 8080
          hostPort      = 8080
          protocol      = "tcp"
        }
      ]

      environment = [
        {
          name  = "ASPNETCORE_URLS"
          value = "http://+:8080"
        },
        {
          name  = "POSTGRES_HOST"
          value = var.db_endpoint
        },
        {
          name  = "POSTGRES_PORT"
          value = tostring(var.db_port)
        },
        {
          name  = "POSTGRES_DATABASE"
          value = var.db_name
        }
      ]

      secrets = [
        {
          name      = "POSTGRES_USER"
          valueFrom = "${var.db_secret_arn}:username::"
        },
        {
          name      = "POSTGRES_PASSWORD"
          valueFrom = "${var.db_secret_arn}:password::"
        },
        {
          name      = "JWT_SECRET_KEY"
          valueFrom = var.jwt_secret_arn
        }
      ]

      logConfiguration = {
        logDriver = "awslogs"
        options = {
          "awslogs-group"         = aws_cloudwatch_log_group.backend.name
          "awslogs-region"        = var.aws_region
          "awslogs-stream-prefix" = "backend"
        }
      }
    }
  ])

  tags = {
    Project     = var.project_name
    Environment = var.environment
  }
}

resource "aws_ecs_service" "backend" {
  name            = "${var.project_name}-${var.environment}-backend"
  cluster         = aws_ecs_cluster.main.id
  task_definition = aws_ecs_task_definition.backend.arn
  desired_count   = var.desired_count
  launch_type     = "FARGATE"

  ifecycle {
    ignore_changes = [task_definition]
  }

  network_configuration {
    subnets          = var.app_subnet_ids
    security_groups  = [var.ecs_security_group_id]
    assign_public_ip = false
  }

  load_balancer {
    target_group_arn = var.backend_target_group_arn
    container_name   = "backend"
    container_port   = 8080
  }

  depends_on = [
    aws_cloudwatch_log_group.backend
  ]

  tags = {
    Project     = var.project_name
    Environment = var.environment
  }
}

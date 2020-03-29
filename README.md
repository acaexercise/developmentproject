# ACA.Domain (.Net Standard 2.0)
- contains simple POCOs shared across the other projects

# ACA.Data  (.Net Standard 2.0)
- contains data services for accessing the csv data files

# ACA.Classes.Tests (.Net Core 3.1)
- contains tests to validate various functionality

# ACA Classes.ConsoleApp (.Net Core 3.1)
- command line executable to generating the output

# ACA.Classes.Blazor (.Net Core 3.1)
  
- [Blazor Server App](https://docs.microsoft.com/en-us/aspnet/core/blazor/hosting-models?view=aspnetcore-3.1#blazor-server) for UI POC displaying Class Data

- Deployed to [https://aca.jimkeeley.com/](https://aca.jimkeeley.com/)

- UI Widgets built with [Telerik UI for Blazor](https://www.telerik.com/blazor-ui) - requires access to Telerik Private Nuget Feed

- Currently Hosted in AWS using ECS [Fargate](https://aws.amazon.com/fargate/) with an ELB routing across 3 AZs

![ECS](https://acapublicimages.s3.us-east-2.amazonaws.com/EcsCluster2.png "ECS")

- Data Files Securely Read from and Written to S3 Bucket

![S3](https://acapublicimages.s3.us-east-2.amazonaws.com/s32.png "S3")

- Logging via .NET Core's Built in ILogger routed to Cloudwatch LogGroup

![Cloudwatch](https://acapublicimages.s3.us-east-2.amazonaws.com/CloudWatch2.png "Cloudwatch")

# CI/CD

- Build Pipeline setup to automatical trigger build on commit to master branch using Github webhooks

![CI/CD](https://acapublicimages.s3.us-east-2.amazonaws.com/CICD2.png "CI/CD")

- //TODO:Setup CodeDeploy to auto deploy
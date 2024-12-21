var builder = DistributedApplication.CreateBuilder(args);

var apiProductService = builder.AddProject<Projects.Ecommerce_ProductService>("apiservice-product");
var apiOrderService = builder.AddProject<Projects.Ecommerce_OrderService>("apiservice-order");

builder.AddProject<Projects.Ecommerce_Web>("webfrontend")
    .WithReference(apiProductService)
    .WithReference(apiOrderService);

builder.Build().Run();

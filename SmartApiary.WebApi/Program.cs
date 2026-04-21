using Microsoft.OpenApi;
using SmartApiary.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

var webApiConfig = builder.Configuration.GetSection("WebApi");
string corsPolicyName = webApiConfig.GetValue<string>("CorsPolicyName") ?? "_reactAppPolicy";
string reactOrigin = webApiConfig.GetValue<string>("ReactOrigin") ?? "http://localhost:5173";
string notificationHubRoute = webApiConfig.GetValue<string>("NotificationHubRoute") ?? "/notification-hub";

builder.Services
    .AddWebApiServices(builder.Configuration)
    .AddWebApiCors(reactOrigin)
    .AddWebApiSignalR();

var app = builder.Build();

app.UseCors(corsPolicyName);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_1;
    });
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
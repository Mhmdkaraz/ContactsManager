using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using Serilog;
using CRUDExample.Filters.ActionFilters;
using CRUDExample.StartupExtensions;
using CRUDExample.Middleware;

var builder = WebApplication.CreateBuilder(args);

//Logging
//builder.Host.ConfigureLogging(loggingProvider => {
//    loggingProvider.ClearProviders();
//    loggingProvider.AddConsole();
//    loggingProvider.AddDebug();
//    loggingProvider.AddEventLog();
//});

//Serilog
builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) => {
    loggerConfiguration.ReadFrom.Configuration(context.Configuration) //read counfiguration settings from built-in IConfiguration
    .ReadFrom.Services(services); // read out current app's services and make them available to serilog
});

builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();
if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
} else {
    app.UseExceptionHandler("/Error");
    app.UseExceptionHandlingMiddleware();
}
app.UseHsts();
app.UseHttpsRedirection();
app.UseSerilogRequestLogging();


app.UseHttpLogging();
//app.Logger.LogDebug("debug-message");
//app.Logger.LogInformation("information-message");
//app.Logger.LogWarning("warning-message");
//app.Logger.LogError("error-message");
//app.Logger.LogCritical("critical-message");

if (builder.Environment.IsEnvironment("Test") == false)
    Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");

app.UseStaticFiles();
app.UseRouting();//Identifying action method based on route
app.UseAuthentication();//Reading Identity cookie
app.UseAuthorization();//validates access permissions of the user
app.MapControllers();//Excecute the filter pipeline (action + filters)
app.UseEndpoints(endpoints => {
    endpoints.MapControllerRoute(name: "areas", pattern: "{area:exists}/{controller=Home}/{action=Index}");
    //Admin/Home/Index
    //Admin
    endpoints.MapControllerRoute(name: "default", pattern: "{controller}/{action}/{id?}");
});
app.Run();

public partial class Program { } //make the auto-generated Program accessible programmatically
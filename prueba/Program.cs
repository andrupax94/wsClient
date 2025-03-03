using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using prueba;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var configurationService = new ConfigurationService();
builder.Services.AddSingleton(configurationService.GetConfiguration());

builder.Services.AddScoped<SignalRService>();
builder.Services.AddScoped<ConfigurationService>();
builder.Services.AddScoped<StatusMessageService>();
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
await builder.Build().RunAsync();

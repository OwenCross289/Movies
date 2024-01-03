using BlazorSsrDynamic.Client.Pages;
using BlazorSsrDynamic.Components;
using Movies.Api.Sdk;
using Refit;

var builder = WebApplication.CreateBuilder(args);

// Adds all of the Aspire goodness
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// add the sdk for the web api
builder.Services
    .AddHttpClient()
    .AddRefitClient<IMoviesApi>()
    .ConfigureHttpClient(x =>
        x.BaseAddress = new("http://apiservice"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Counter).Assembly);

app.Run();

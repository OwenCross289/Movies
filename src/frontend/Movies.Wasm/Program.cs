using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Movies.Wasm;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.AddServiceDefaults();
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .AddHttpClient()
    .AddRefitClient<IMoviesApi>()
    .ConfigureHttpClient(x =>
        x.BaseAddress = new("http://apiservice"));


await builder.Build().RunAsync();
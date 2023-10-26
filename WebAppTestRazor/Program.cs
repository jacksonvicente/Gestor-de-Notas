using Google.Api;
using WebAppTestRazor.Pages;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Adicione o servi�o HttpClient para a classe DadosController
builder.Services.AddHttpClient<DadosController>(client =>
{
    client.BaseAddress = new Uri("https://0232-187-108-133-151.ngrok-free.app/WSEAIDataExchange?tipoinformacao=101&emitentes=45181198000195&datainicial=2023-10-22&datafinal=2023-10-23");
    // Outras configura��es do HttpClient, se necess�rio.
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
    name: "default",
     pattern: "[Controller=Login]/[actions=Login]/[id?]");
});

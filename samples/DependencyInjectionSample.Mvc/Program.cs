using DependencyInjectionSample.Mvc.Data;
using RavenDB.AspNetCore.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services
    .AddRavenManager(
        builder.Configuration.GetSection("RavenManager"))
    .AddScopedAsyncSession();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Initialize database: create DB, deploy indexes, seed test data
    var manager = app.Services.GetRequiredService<IRavenManager>();
    var store = manager.GetStore();
    await DatabaseInitializer.InitializeAsync(store);
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{**id}");

app.Run();

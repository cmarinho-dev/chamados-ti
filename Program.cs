using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}

var versaoMySql = ServerVersion.Parse("9.6.0");

builder.Services.AddDbContext<ChamadosTI.Data.ContextoChamados>(options =>
    options.UseMySql(connectionString, versaoMySql));

var app = builder.Build();

/*
// ... após app.Build()

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ChamadosTI.Data.ContextoChamados>();
    // Certifique-se de que o banco está criado
    await context.Database.MigrateAsync(); // se quiser aplicar migrações automaticamente
    // Agora chame o método de importação (lembre-se de passar a connection string do banco legado)
    await context.ImportarDadosLegadoAsync();
}

app.Run();
*/

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

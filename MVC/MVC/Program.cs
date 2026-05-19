using Microsoft.EntityFrameworkCore;
using MVC.Data;
using Rotativa.AspNetCore;
using MVC.ModelBinders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    // Acepta decimales con coma o punto (evita errores al guardar/editar precios)
    options.ModelBinderProviders.Insert(0, new DecimalModelBinderProvider());
});

builder.Services.AddDbContext<AppDbContext>(option =>
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddSession();

var app = builder.Build();

// IMPORTANTE: en Development muestra el error real en el navegador
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// IMPORTANTE: necesario para servir archivos en wwwroot (incluye /imagenes/*)
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

RotativaConfiguration.Setup(@"C:\Program Files\wkhtmltopdf\bin", "");

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
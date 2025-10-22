using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ECOS.Digital.Data;
using ECOS.Digital.Models;

var builder = WebApplication.CreateBuilder(args);

// Configurar la cadena de conexión
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Data Source=app.db";

// Agregar DbContext con SQLite (o SQL Server si prefieres)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Configurar ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    // Configuración de contraseñas (puedes ajustar según necesites)
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI(); // Esto agrega las páginas de Identity predeterminadas

// Agregar servicios
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Necesario para Identity UI

var app = builder.Build();

// Configurar el pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// IMPORTANTE: El orden es crucial
app.UseAuthentication(); // Agregar autenticación
app.UseAuthorization();

app.MapStaticAssets();

// Mapear rutas de Razor Pages (para Identity)
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
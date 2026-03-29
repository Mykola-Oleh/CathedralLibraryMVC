using CathedralLibraryDomain.Model;
using CathedralLibraryInfrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DbCathedralLibraryContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .UseSnakeCaseNamingConvention());

// Підключення контексту для Identity
builder.Services.AddDbContext<IdentityContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("IdentityConnection")));

// Налаштування Identity
builder.Services.AddIdentity<User, IdentityRole>(options => {
    // Тут можна налаштувати вимоги до пароля
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<IdentityContext>()
.AddDefaultTokenProviders(); // Додайте цей рядок

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<User>>();
        var rolesManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await RoleInitializer.InitializeAsync(userManager, rolesManager);
    }
    catch (Exception ex)
    {
        // Логування помилки
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Помилка при ініціалізації ролей та користувачів.");
    }
}

// 1. Обробка помилок
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// 2. Статичні файли (CSS, JS) - до авторизації!
app.UseStaticFiles();

// 3. Маршрутизація
app.UseRouting();

// 4. АВТЕНТИФІКАЦІЯ (Хто ви?) - обов'язково ПЕРЕД Authorization
app.UseAuthentication();

// 5. АВТОРИЗАЦІЯ (Що вам можна?)
app.UseAuthorization();

// 6. Кінцеві точки (Контролери)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


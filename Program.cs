using CRM.Web.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Сервисы в контейнер
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();
builder.Services.AddScoped<DatabaseService>();

WebApplication app = builder.Build();

// Настройка конвейера HTTP запросов
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
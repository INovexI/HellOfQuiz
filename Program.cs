using Microsoft.EntityFrameworkCore;
using HellOfQuiz.Data;
using HellOfQuiz.Services;
using HellOfQuiz.Hubs;

var builder = WebApplication.CreateBuilder(args);

// MVC servisini ekle
builder.Services.AddControllersWithViews();

// MySQL veritabanı bağlantısını yapılandır
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Servisler
builder.Services.AddSingleton<RoomService>();
builder.Services.AddSignalR();

// Session desteğini ekle (kullanıcı oturumları için)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // 60 dakika oturum süresi
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Hata yönetimi
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

// Session middleware'ini etkinleştir
app.UseSession();

// Rotaları yapılandır
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<QuizHub>("/quizHub");

// Uygulama başlarken veritabanını otomatik oluştur
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
}

app.Run();

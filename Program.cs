using Microsoft.AspNetCore.Authentication.Cookies;
using RC.Services;

var builder = WebApplication.CreateBuilder(args);

// 1) إضافة خدمة MVC
builder.Services.AddControllersWithViews();

// 2) تسجيل الخدمات المختصرة (هذا يحل مشكلة الشاشة البيضاء)
builder.Services.AddSingleton<DbS>();
builder.Services.AddSingleton<ImgS>();
builder.Services.AddSingleton<PwdS>();

// 3) إعداد المصادقة
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Acc/Login";
        options.AccessDeniedPath = "/Acc/Denied";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Cookie.Name = "RC.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// 4) تغيير مسار البداية ليفتح واجهتنا الرئيسية (Hom) بدلاً من (Home)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Hom}/{action=Index}/{id?}");

app.Run();
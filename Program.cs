using IdentityRazor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var serverVersion = new MySqlServerVersion(new Version(8, 0, 31));
var connectionString = builder.Configuration.GetConnectionString("Default");

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<MyBlogContext>(
    dbContextOptions => dbContextOptions
        .UseMySql(connectionString, serverVersion)
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors()
);

//Dang ky Identity
// builder.Services.AddIdentity<AppUser, IdentityRole>()
//                 .AddEntityFrameworkStores<MyBlogContext>()
//                 .AddDefaultTokenProviders();
// builder.Services.AddDefaultIdentity<AppUser>()
//        .AddEntityFrameworkStores<MyBlogContext>()
//        .AddDefaultTokenProviders();

builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<MyBlogContext>();

builder.Services.AddOptions();
var mailSetting = builder.Configuration.GetSection("MailSettings");
builder.Services.Configure<MailSettings>(mailSetting);
builder.Services.AddSingleton<IEmailSender, SendMailService>();

// Truy cập IdentityOptions
builder.Services.Configure<IdentityOptions> (options => {
    // Thiết lập về Password
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

    // Cấu hình Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes (1); // Khóa 5 phút
    options.Lockout.MaxFailedAccessAttempts = 3; // Thất bại 3 lần thì khóa
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;  // Email là duy nhất

    // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
    options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
    options.SignIn.RequireConfirmedAccount = true;         // Xác thực tài khoản khi đăng nhập

});

builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = "/login/";
    options.LogoutPath = "/logout/";
    //Duong dan toi trang khi user bi cam truy cap
    options.AccessDeniedPath = "/notaccess.html";
});

builder.Services.AddAuthentication()
                .AddGoogle(options => {
                    var ggconfig = builder.Configuration.GetSection("Authentication:Google");
                    options.ClientId = ggconfig["ClientId"];
                    options.ClientSecret = ggconfig["ClientSecret"];
                    //Mặc định là http://localhost:5296/signin-google
                    options.CallbackPath = "/login-from-google";
                })
                .AddFacebook(options => {
                    var fbconfig = builder.Configuration.GetSection("Authentication:Facebook");
                    options.AppId = fbconfig["AppId"];
                    options.AppSecret = fbconfig["AppSecret"];
                    options.CallbackPath = "/login-from-facebook";
                })
                // .AddTwitter()
                // .AddMicrosoftAccount()
                ;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// IdentityUser user;
// IdentityDbContext context;
// SignInManager<AppUser> s;
// UserManager<AppUser> u;

app.Run();

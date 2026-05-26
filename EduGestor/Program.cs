using EduGestor.Data;
using EduGestor.Extensions;
using EduGestor.Services;
using Microsoft.EntityFrameworkCore;
using EduGestor.Models.Identity;
using Microsoft.AspNetCore.Identity;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<EduGestorContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("EduGestorContext")
    ?? throw new InvalidOperationException("Connection string 'EduGestorContext' not found.")));

QuestPDF.Settings.License = LicenseType.Community;

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services
    .AddIdentity<AppUser, IdentityRole>(options =>
    {
        // PASSWORD
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;

        // USER
        options.User.RequireUniqueEmail = true;
    })
    
    .AddRoles<IdentityRole>()

    .AddEntityFrameworkStores<EduGestorContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";

    options.AccessDeniedPath =
        "/Account/AccessDenied";
});

builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<GuardianService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<TeacherService>();
builder.Services.AddScoped<StudentClassService>();
builder.Services.AddScoped<RegistrationService>();
builder.Services.AddScoped<DisciplineService>();
builder.Services.AddScoped<GradeService>();
builder.Services.AddScoped<DisciplineClassService>();
builder.Services.AddScoped<ValidateExtensions>();
builder.Services.AddScoped<PortalService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<TeacherPortalService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=HomeRedirect}/{action=Index}/{id?}")
    .WithStaticAssets();

await SeedRolesAsync(app);

await SeedAdminAsync(app);

app.Run();

static async Task SeedRolesAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();

    var roleManager =
        scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles =
    [
        "Admin",
        "Secretary",
        "Teacher",
        "Guardian"
    ];

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(
                new IdentityRole(role));
        }
    }
}

static async Task SeedAdminAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();

    var userManager =
        scope.ServiceProvider
            .GetRequiredService<UserManager<AppUser>>();

    var adminEmail = "admin@edugestor.com";

    var admin =
        await userManager.FindByEmailAsync(adminEmail);

    if (admin == null)
    {
        var user = new AppUser
        {
            FullName = "System Administrator",
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(
            user,
            "Admin@123");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(
                user,
                "Admin");
        }
    }
}

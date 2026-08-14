using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechChallenge;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Adicionar serviço de Banco de Dados (EntityFramework)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Adicionar o serviço de Identidade da MicroSoft
builder.Services.AddIdentity<IdentityUser, IdentityRole>(
   options =>
   {
       options.Password.RequireDigit = false;
       options.Password.RequiredLength = 4;
       options.Password.RequireNonAlphanumeric = false;
       options.Password.RequireUppercase = false;

       options.User.RequireUniqueEmail = true;
       options.SignIn.RequireConfirmedEmail = false;
       options.SignIn.RequireConfirmedAccount = false;
   } 
).AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(
    options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
    }
);


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
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// ============================================================
// SEED DE DADOS INICIAL
// ============================================================
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = {"Admin", "Professor", "Aluno"};

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    var userManager = scope.ServiceProvider
        .GetRequiredService<UserManager<IdentityUser>>();

    string adminEmail = "admin@techchallenge.com.br";

    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var admin = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(
            admin,"Admin@123");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin,"Admin");
        }
    }
}


app.Run();

using FerienspassWebApp.Areas.Identity;
using FerienspassWebApp.Areas.Identity.Data;
using FerienspassWebApp.Data;
using FerienspassWebApp.Models;
using FerienspassWebApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Blazorise.Scheduler;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException(
        $"DefaultConnection fehlt. Environment: {builder.Environment.EnvironmentName}");

}
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDbContextFactory<ApplicationDbContext>(
    options => options.UseSqlServer(connectionString),
    ServiceLifetime.Scoped);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services
    .AddDefaultIdentity<ApplicationUser>(options => { options.SignIn.RequireConfirmedAccount = false; }) //wieder auf true setzen
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<DocxService>();
builder.Services.AddTransient<IEmailSender, EmailService>();
builder.Services
       .AddBlazorise()
       .AddBootstrap5Providers()
       .AddFontAwesomeIcons();
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"keys"))
    .SetApplicationName("Ferienspass");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
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

app.MapControllers();
app.MapBlazorHub();
app.MapRazorPages();
app.MapFallbackToPage("/_Host");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    //added 22.05.2026
    var db = services.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate(); 
    
    try { 
        await SeedRoles.CreateRoles(services);
        await SeedRoles.CreateAdmin(services);
    }
    catch (Exception ex) { Console.WriteLine("Speed Fehler: " + ex.Message); }

    
}

    app.Run();

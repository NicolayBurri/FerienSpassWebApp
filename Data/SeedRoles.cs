using FerienspassWebApp.Models;
using Microsoft.AspNetCore.Identity;

public static class SeedRoles
{
    public static async Task CreateRoles(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        string[] roles = { "Admin", "Kursleiter", "Eltern" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }


    public static async Task CreateAdmin(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var config = serviceProvider.GetRequiredService<IConfiguration>();

        var email = config["SeedAdmin:Email"];
        var password = config["SeedAdmin:Password"];

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            return;


        var adminUser = await userManager.FindByEmailAsync(email);

        if (adminUser == null)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true

            };

            var result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }

        }
    }
}

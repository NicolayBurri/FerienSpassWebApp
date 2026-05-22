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


    //New added 22.05.2026
    public static async Task CreateAdmin(IServiceProvider serviceProvider)
    {
        Console.WriteLine("Admin Seed started");

        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var config = serviceProvider.GetRequiredService<IConfiguration>();

        var email = config["SeedAdmin:Email"];
        var password = config["SeedAdmin:Password"];

        Console.WriteLine($"Email: {email}");

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Console.WriteLine("SeedAdmin config missing");
            return;
        }


        var adminUser = await userManager.FindByEmailAsync(email);

        if (adminUser != null)
        {
            Console.WriteLine("Admin already exists");
            return;
        }

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
                Console.WriteLine("Admin created successfully");

                var roleResult = await userManager.AddToRoleAsync(user, "Admin");

                if (roleResult.Succeeded)
                {
                    Console.WriteLine("Admin role assigned");
                }
                else
                {
                    foreach (var error in roleResult.Errors)
                    {
                        Console.WriteLine(error.Description);
                    }
                }
            }
            else
            {
                Console.WriteLine("Admin creation failed");

                foreach (var error in result.Errors)
                {
                    Console.WriteLine(error.Description);
                }
            }

        }
    }
}

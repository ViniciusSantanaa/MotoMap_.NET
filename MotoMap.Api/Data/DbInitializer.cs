using MotoMap.Api.Models;
using System.Linq;

namespace MotoMap.Api.Data
{
    public static class DbInitializer
    {
        public static void Seed(AppDbContext context)
        {

            context.Database.EnsureCreated();

            var admin = context.Users.FirstOrDefault(u => u.Username == "admin");
            var adminPasswordPlain = "admin"; 

            if (admin == null)
            {
                admin = new User
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPasswordPlain),
                    Role = "Admin"
                };
                context.Users.Add(admin);
            }
            else
            {
                admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(adminPasswordPlain);
                context.Users.Update(admin);
            }

            context.SaveChanges();
        }
    }
}

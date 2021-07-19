using System.Linq;
using System.Threading.Tasks;
using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager){

            if(!userManager.Users.Any()){
                var userDefault = new AppUser{
                    DisplayName = "Bob",
                    Email = "bob@test.com",
                    UserName = "bob@test.com",
                    Address = new Address{
                        FirstName = "Bob",
                        LastName = "Bobbity",
                        Street = "20th The Street",
                        City = "New York",
                        State = "NY",
                        ZipCode = "90218"
                    }
                };

                await userManager.CreateAsync(userDefault, "Pa$$w0rd");
            }
        }
    }
}
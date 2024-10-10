using LearnSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace LearnSystem;

public class CustomUserClaimsPrincipalFactory(UserManager<User> userManager, RoleManager<Roles> roleManager, IOptions<IdentityOptions> options) : UserClaimsPrincipalFactory<User, Roles>(userManager, roleManager, options)
{
}

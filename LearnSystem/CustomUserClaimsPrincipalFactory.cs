using LearnSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace LearnSystem;

public class CustomUserClaimsPrincipalFactory(UserManager<User> userManager, RoleManager<ApplicationRole> roleManager, IOptions<IdentityOptions> options) : UserClaimsPrincipalFactory<User, ApplicationRole>(userManager, roleManager, options)
{
}

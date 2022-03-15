using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using PrivacyManager.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PrivacyManager.Data
{
    // Configure the application sign-in manager which is used in this application.
    public class PrivacyManagerSignInManager : SignInManager<PrivacyManagerUser, string>
    {
        public PrivacyManagerSignInManager(PrivacyManagerUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(PrivacyManagerUser user)
        {
            return user.GenerateUserIdentityAsync((PrivacyManagerUserManager)UserManager);
        }

        public static PrivacyManagerSignInManager Create(IdentityFactoryOptions<PrivacyManagerSignInManager> options, IOwinContext context)
        {
            return new PrivacyManagerSignInManager(context.GetUserManager<PrivacyManagerUserManager>(), context.Authentication);
        }
    }
}

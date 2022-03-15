using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrivacyManager.Entities.CustomEntities
{
    public class UserWithRoleEntity
    {
        public PrivacyManagerUser User { get; set; }

        public List<IdentityRole> Roles { get; set; }
    }
}

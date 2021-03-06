using Microsoft.AspNet.Identity.EntityFramework;
using PrivacyManager.Data;
using PrivacyManager.Entities.CustomEntities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrivacyManager.Services
{
    public class ControlPanelService
    {
        #region Define as Singleton
        private static ControlPanelService _Instance;

        public static ControlPanelService Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new ControlPanelService();
                }

                return (_Instance);
            }
        }

        private ControlPanelService()
        {
        }
        #endregion
        
        public List<IdentityRole> GetAllRoles()
        {
            using (var context = new PrivacyManagerContext())
            {
                return context.Roles
                                        .OrderBy(x => x.Name)
                                        .ToList();
            }
        }

        public RolesSearch GetRoles(string searchTerm, int pageNo, int pageSize)
        {
            using (var context = new PrivacyManagerContext())
            {
                var search = new RolesSearch();

                if (string.IsNullOrEmpty(searchTerm))
                {
                    search.Roles = context.Roles
                                        .Include(u => u.Users)
                                        .OrderBy(x => x.Name)
                                        .Skip((pageNo - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToList();

                    search.TotalCount = context.Users.Count();
                }
                else
                {
                    search.Roles = context.Roles
                                        .Where(u => u.Name.ToLower().Contains(searchTerm.ToLower()))
                                        .Include(u => u.Users)
                                        .OrderBy(x => x.Name)
                                        .Skip((pageNo - 1) * pageSize)
                                        .Take(pageSize)
                                        .ToList();

                    search.TotalCount = context.Roles
                                        .Where(u => u.Name.ToLower().Contains(searchTerm.ToLower())).Count();
                }

                return search;
            }
        }

        public RoleWithUsersEntity GetRoleByID(string ID)
        {
            using (var context = new PrivacyManagerContext())
            {
                return context.Roles
                    .Where(r => r.Id == ID)
                    .Include(u => u.Users)
                    .Select(x => new RoleWithUsersEntity()
                    {
                        Role = x,
                        Users = x.Users.Select(userRole => context.Users.Where(user => user.Id == userRole.UserId).FirstOrDefault()).ToList()
                    }).FirstOrDefault();
            }
        }

        public async Task<bool> NewRole(IdentityRole role)
        {
            using (var context = new PrivacyManagerContext())
            {
                context.Roles.Add(role);

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> UpdateRole(IdentityRole role)
        {
            using (var context = new PrivacyManagerContext())
            {
                var oldRole = context.Roles.Find(role.Id);

                if(oldRole != null)
                {
                    oldRole.Name = role.Name;

                    context.Entry(oldRole).State = System.Data.Entity.EntityState.Modified;
                }

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> DeleteRole(string ID)
        {
            using (var context = new PrivacyManagerContext())
            {
                var role = context.Roles.Find(ID);

                if (role != null)
                {
                    context.Entry(role).State = System.Data.Entity.EntityState.Deleted;
                }

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> AddUserRole(string userID, string roleID)
        {
            using (var context = new PrivacyManagerContext())
            {
                var user = context.Users.Find(userID);

                var role = context.Roles.Find(roleID);

                if(user != null && role != null)
                {
                    user.Roles.Add(new IdentityUserRole() { UserId = userID, RoleId = roleID });
                }

                return await context.SaveChangesAsync() > 0;
            }
        }

        public async Task<bool> RemoveUserRole(string userID, string roleID)
        {
            using (var context = new PrivacyManagerContext())
            {
                var user = context.Users.Find(userID);
                
                if (user != null)
                {
                    var userRole = user.Roles.Where(r => r.RoleId == roleID).FirstOrDefault();

                    if(user != null) user.Roles.Remove(userRole);
                }

                return await context.SaveChangesAsync() > 0;
            }
        }
    }
}

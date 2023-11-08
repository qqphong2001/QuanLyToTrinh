using Database;
using Database.Models;
using Repositories.BaseRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Repositories
{
    public interface IUserRepository : IBaseRepository<AppUser>
    {        
    }
    public class UserRepository : BaseRepository<AppUser>, IUserRepository
    {
        public UserRepository(QLTTrContext context) : base(context)
        {

        }
    }
    public interface IRoleRepository : IBaseRepository<AppRole>
    {
    }
    public class RoleRepository : BaseRepository<AppRole>, IRoleRepository
    {
        public RoleRepository(QLTTrContext context) : base(context)
        {

        }
    }

    public interface IUserInRoleRepository : IBaseRepository<AppUserInRole>
    {
    }
    public class UserInRoleRepository : BaseRepository<AppUserInRole>, IUserInRoleRepository
    {
        public UserInRoleRepository(QLTTrContext context) : base(context)
        {

        }
    }

    //public interface IMembershipRepository : IBaseRepository<AspnetMembership>
    //{
    //}
    //public class MembershipRepository : BaseRepository<AspnetMembership>, IMembershipRepository
    //{
    //    public MembershipRepository(QLTTrContext context) : base(context)
    //    {

    //    }
    //}

    
}

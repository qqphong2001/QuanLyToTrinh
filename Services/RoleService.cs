using Database.Models;
using Repositories;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IRoleSerice
    {
        Task<IEnumerable<AppRole>> GetAllRoles();
    }
    public class RoleService : IRoleSerice
    {
        private readonly IRoleRepository _roleRepository;
        public RoleService(IRoleRepository roleRepository) 
        { 
            _roleRepository = roleRepository;
        }

        public async Task<IEnumerable<AppRole>> GetAllRoles()
        {
            var result =   _roleRepository.GetAll().ToList();
            if (result == null)
            {
                throw new NotImplementedException();
            }
            return result;
        }
    }
}

using Repositories;
using Services.DTO;
using Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LinqKit;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime;
using System.Security.Claims;
using AutoMapper;

namespace Services
{
    public interface IAppUserService
    {
        Task<Guid> UserSignUp(SignUpDTO payload);
        Task<ResponseTokenDTO> UserLogin(LogInDTO payload);
        Task<bool> ChangePassword(ChangePasswordDTO payload);
        Task<IEnumerable<UserInfoDTO>> GetAllInfoUser();

        Task<bool> ResetPassword(string userId);
    }
    public class AppUserService : IAppUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserInRoleRepository _userInRoleRepository;
        private readonly IMapper _mapper;
        private string TokenSecret = "77D3624E-1F44-4F7E-A472-D5E449D69F2C-77D3624E-1F44-4F7E-A472-D5E449D69F2C";
        private int TokenExpiryDay = 1;
        public AppUserService(IMapper mapper,IUserRepository userRepository, IRoleRepository roleRepository, IUserInRoleRepository userInRoleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userInRoleRepository = userInRoleRepository;
            _mapper = mapper;
        }
        public async Task<Guid> UserSignUp(SignUpDTO payload)
        {
            if (await CheckSignUpExisted(payload.Username, payload.Email, payload.PhoneNumber)) throw new Exception("Username, PhoneNumber or Email already existed");

            Guid newUserId = new Guid();
            string salt = await GenerateSalt();
            var userToCreate = new AppUser()
            {
                UserId = newUserId,
                UserName = payload.Username,
                UserFullName = payload.FullName,
                Email = payload.Email,
                PhoneNumber = payload.PhoneNumber,
                PasswordSalt = salt,
                HashedPassword = await PasswordHashing(payload.Password, salt)
            };

            var userRoles = from p in payload.RoleIds select new AppUserInRole() { Id = 0, UserId = userToCreate.UserId, RoleId = p };

            try
            {
                await _userRepository.AddAsync(userToCreate);
                await _userRepository.SaveChanges();
                await _userInRoleRepository.AddRangeAsync(userRoles);
                await _userInRoleRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error occured when add new user into databse: " + ex.Message);
            }
            return userToCreate.UserId;
        }

        public async Task<ResponseTokenDTO> UserLogin(LogInDTO payload)
        {
            Expression<Func<AppUser, bool>> baseFilter = f => true;
            baseFilter = baseFilter.And(x => x.UserName.ToLower() == payload.Username.ToLower());
            var userToAuthenticate = await _userRepository.GetSingleByCondition(baseFilter);
            if (userToAuthenticate == null)
            {
                throw new Exception("Authentication failed. Please check your username and password");
            }
            var inputPassword = await PasswordHashing(payload.Password, userToAuthenticate.PasswordSalt);
            if (inputPassword != userToAuthenticate.HashedPassword)
            {
                throw new Exception("Authentication failed. Please check your username and password");
            }
            return await GenerateToken(userToAuthenticate);
        }

        public async Task<bool> ChangePassword(ChangePasswordDTO payload)
        {
            var userToChange = await _userRepository.FirstOrDefaultAsync(x => x.UserId == payload.UserId);
            if (userToChange == null) throw new Exception("Change password failded - User not found");
            if (await PasswordHashing(payload.OldPassword, userToChange.PasswordSalt) != userToChange.HashedPassword) throw new Exception("Change password failded - Authentication failed");
            var newSalt = await GenerateSalt();
            userToChange.PasswordSalt = newSalt;
            userToChange.HashedPassword = await PasswordHashing(payload.NewPassword, newSalt);
            _userRepository.Update(userToChange);
            await _userRepository.SaveChanges();
            return true;
        }

        public async Task<bool> ResetPassword(string userId)
        {
            const string samplePassword = "Canbo@1234";
            var userToChange = await _userRepository.FirstOrDefaultAsync(x => x.UserId.ToString() == userId);
            if (userToChange == null) throw new Exception("Change password failded - User not found");
            var newSalt = await GenerateSalt();
            userToChange.PasswordSalt = newSalt;
            userToChange.HashedPassword = await PasswordHashing(samplePassword, newSalt);
            _userRepository.Update(userToChange);
            await _userRepository.SaveChanges();
            return true;
        }

        private async Task<bool> CheckSignUpExisted(string username, string email, string phone)
        {
            Expression<Func<AppUser, bool>> baseFilter = f => true;
            baseFilter = baseFilter.And(x => x.UserName.ToLower() == username.ToLower() || x.Email.ToLower() == email.ToLower() || x.PhoneNumber == phone);
            var existingUsername = await _userRepository.GetSingleByCondition(baseFilter);
            if (existingUsername != null) return true;


            return false;
        }

        private async Task<string> GenerateSalt()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] saltBytes = new byte[16];
                rng.GetBytes(saltBytes);
                return Convert.ToBase64String(saltBytes);
            }
        }

        private async Task<string> PasswordHashing(string password, string salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), 10000))
            {
                byte[] hash = pbkdf2.GetBytes(20);
                return Convert.ToBase64String(hash);
            }
        }

        private async Task<ResponseTokenDTO> GenerateToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };
            var roleIds = _userInRoleRepository.GetAll().Where(x => x.UserId == user.UserId).Select(x => x.RoleId).ToList();
            var roles = _roleRepository.GetAll().Where(x => roleIds.Contains(x.RoleId)).Select(x => x.RoleName).ToList();

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenSecret.ToString()));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(TokenExpiryDay),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var tokenResponse = new ResponseTokenDTO
            {
                AccessToken = tokenHandler.WriteToken(token),
                TokenExpiration = token.ValidTo,
                UserName = user.UserName,
                DisplayName = user.UserFullName,
                UserId = user.UserId,
                Roles = roles
            };

            return tokenResponse;
        }

        public async Task<IEnumerable<UserInfoDTO>> GetAllInfoUser()
        {
            var infoUser =  _userRepository.GetAll().ToList();

            if (infoUser == null)
            {
                throw new NotImplementedException();
            }
            var result = _mapper.Map <IEnumerable<UserInfoDTO>>(infoUser);
            return result;
        }


    }
}

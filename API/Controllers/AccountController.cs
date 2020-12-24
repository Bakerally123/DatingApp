using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using System.Security.Cryptography;
using System.Text;
using API.DTO;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountController(DataContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("Register")]
       public async Task<ActionResult<UserDto>> Register(RegisterDto registerdto)
        {
            if (await UserExists(registerdto.username)) return (BadRequest("Username already taken"));

            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = registerdto.username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerdto.password)),
                PasswordSalt = hmac.Key
            };
            _context.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
            
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto logindto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x=>x.UserName==logindto.username.ToLower());
            if (user == null) return Unauthorized("Invalid Username");
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computerHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(logindto.password));

            for(int i=0;i<computerHash.Length;i++)
            {
                if(computerHash[i]!=user.PasswordHash[i])
                    return Unauthorized("Invalid Password");
            }
            return new UserDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username);
        }
    }
}

using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class UserService : IUserService
    {

        private readonly UserManager<UserApp> _userManger;

        public UserService(UserManager<UserApp> userManger)
        {
            _userManger = userManger;
        }

        public async Task<Response<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto)
        {

            var user = new UserApp
            {
                Email = createUserDto.Email,
                UserName = createUserDto.UserName,

            };

            var result = await _userManger.CreateAsync(user, createUserDto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(m => m.Description).ToList();
                return Response<UserAppDto>.Fail(new ErrorDto(errors, true), 400);

            }

            return Response<UserAppDto>.Success(ObjectMapper.Mapper.Map<UserAppDto>(user), 200);
        }

        public async Task<Response<UserAppDto>> GetUserByNameAsync(string userName)
        {
            var user = await _userManger.FindByNameAsync(userName);
            if (user == null)
                return Response<UserAppDto>.Fail("kullanıcı bulunamadı", 404, true);

            return Response<UserAppDto>.Success(ObjectMapper.Mapper.Map<UserAppDto>(user), 200);
        }
    }
}

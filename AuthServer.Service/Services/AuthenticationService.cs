using AuthServer.Core.Configuration;
using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly List<Client> _clients;
        private readonly ITokenService _tokenService;
        private readonly UserManager<UserApp> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<UserRefreshToken> _userRefreshToken;

        public AuthenticationService(IOptions<List<Client>> optClients, ITokenService tokenService, UserManager<UserApp> userManager, IUnitOfWork unitOfWork, IGenericRepository<UserRefreshToken> userRefreshToken)
        {
            _clients = optClients.Value;
            _tokenService = tokenService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _userRefreshToken = userRefreshToken;
        }

        public async Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto)
        {
            if (loginDto == null) throw new ArgumentNullException(nameof(loginDto));
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null) return Response<TokenDto>.Fail("email or password is wrong ", 400, true);

            if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
                return Response<TokenDto>.Fail("email or password is wrong ", 400, true);

            var token = _tokenService.CreateToke(user);
            var refreshUserToken = await _userRefreshToken.Where(m => m.UserId == user.Id).SingleOrDefaultAsync();
            if (refreshUserToken == null)
            {
                await _userRefreshToken.AddAsync(new UserRefreshToken { UserId = user.Id, Code = token.RefresToken, Expiration = token.RefreshTokenExpiration });
            }
            else
            {
                refreshUserToken.Code = token.RefresToken;
                refreshUserToken.Expiration = token.RefreshTokenExpiration;
            }
            await _unitOfWork.CommitAsync();
            return Response<TokenDto>.Success(token, 200);
        }

        public Response<ClientTokenDto> CreateTokenByClient(ClientLoginDto loginDto)
        {
            var client = _clients.SingleOrDefault(m => m.Id == loginDto.ClientId && m.Secret == loginDto.ClientSecret);
            if (client == null)
            {
                return Response<ClientTokenDto>.Fail("client id yada secret bulunamadı", 404, true);
            }
            var token = _tokenService.CreateTokenByClient(client);
            return Response<ClientTokenDto>.Success(token, 200);
        }


        public async Task<Response<TokenDto>> CreateTokenByRefeshToken(string refToken)
        {
            var refreshToken = await _userRefreshToken.Where(m => m.Code == refToken).SingleOrDefaultAsync();
            if (refreshToken == null)
            {
                return Response<TokenDto>.Fail("refresh Token buluanamdı", 404, true);
            }
            var user = await _userManager.FindByIdAsync(refreshToken.UserId);
            if (user == null)
            {
                return Response<TokenDto>.Fail("userId buluanamdı", 404, true);
            }
            var token = _tokenService.CreateToke(user);
            refreshToken.Code = token.RefresToken;
            refreshToken.Expiration = token.RefreshTokenExpiration;
            await _unitOfWork.CommitAsync();

            return Response<TokenDto>.Success(token, 200);
        }

        public async Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken)
        {
            var refresTok = await _userRefreshToken.Where(m => m.Code == refreshToken).SingleOrDefaultAsync();
            if (refresTok == null)
            {
                return Response<NoDataDto>.Fail("refreshtoken yok", 4040, true);
            }
            _userRefreshToken.Remove(refresTok);
            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Success(200);
        }
    }
}

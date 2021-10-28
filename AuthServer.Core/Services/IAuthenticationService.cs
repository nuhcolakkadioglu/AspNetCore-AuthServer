﻿using AuthServer.Core.Dtos;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    public interface IAuthenticationService
    {
        Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto);
        Task<Response<TokenDto>> CreateTokenByRefeshToken(string refreshToken);
        Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken);
        Task<Response<ClientLoginDto>> CreateTokenByClient(ClientLoginDto loginDto);
    }
}

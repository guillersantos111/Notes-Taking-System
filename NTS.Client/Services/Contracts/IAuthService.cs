﻿using NTS.Client.Models.DTOs;

namespace NTS.Client.Services.Contracts
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(LoginDto request);

        Task<ResponseDto> ForgotPasswordAsync(ForgotPasswordDto request);
    }
}

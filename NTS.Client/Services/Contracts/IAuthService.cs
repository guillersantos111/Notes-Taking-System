﻿using NTS.Client.Domain.DTOs;
using NTS.Client.Domain.Models;

namespace NTS.Client.Services.Contracts
{
    public interface IAuthService
    {
        Task<Response> LoginAsync(LoginDtos request);
    }
}
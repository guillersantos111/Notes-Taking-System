﻿using Microsoft.AspNetCore.Components;
using MudBlazor;
using NTS.Client.Domain.DTOs;
using NTS.Client.Domain.Models;
using NTS.Client.Services.Contracts;

namespace NTS.Client.Components
{
    public class ForgotPasswordDialogBase : ComponentBase
    {
        public readonly DialogOptions dialogOptions = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true, NoHeader = true };

        [CascadingParameter] MudDialogInstance mudDialog { get; set; }
        [Parameter] public ForgotPasswordDto forgotPasswordDto { get; set; } = new ForgotPasswordDto();
        [Inject] IAuthService authService { get; set; }

        public Response response { get; set; } = new Response();
        public string ResetToken { get; set; }

        public async Task Submit()
        {
            try
            {
                var result = await authService.ForgotPasswordAsync(forgotPasswordDto);

                if (!string.IsNullOrWhiteSpace(response?.Token))
                {
                    ResetToken = response.Token;
                    response.ErrorMessage = string.Empty;
                }
                else
                {
                    throw new Exception("Failed To Send A Password Reset Email");
                }
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
            }
        }

        public void Cancel()
        {
            mudDialog.Cancel();
        }
    }
}

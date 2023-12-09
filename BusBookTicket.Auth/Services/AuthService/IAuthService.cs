﻿using BusBookTicket.Auth.DTOs.Requests;
using BusBookTicket.Auth.DTOs.Responses;
using BusBookTicket.Core.Infrastructure.Interfaces;
using BusBookTicket.Core.Migrations;
using BusBookTicket.Core.Models.Entity;

namespace BusBookTicket.Auth.Services.AuthService
{
    public interface IAuthService : IService<AuthRequest, FormResetPass, int, AuthResponse, object, object>
    {
        Task<AuthResponse> Login(AuthRequest request);
        Task<Account> GetAccountByUsername(string username, string roleName, bool checkStatus = true);
        Task<bool> ResetPass(FormResetPass request);
        // Task<bool> ChangeStatus(AuthRequest request);
    }
}

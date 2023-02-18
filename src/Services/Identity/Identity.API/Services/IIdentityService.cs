using IdentityServer.Application.Models;

namespace Identity.API.Services;

public interface IIdentityService
{
    Task<LoginResponseModel> Login(LoginRequestModel requestModel);
}


using Identity.API.Models;

namespace Identity.API.Services;

public interface IIdentityService
{
    Task<LoginResponseModel> Login(LoginRequestModel requestModel);
}


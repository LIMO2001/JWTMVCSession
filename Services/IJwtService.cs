using System.Collections.Generic;
using System.Security.Claims;

namespace JwtSessionMvc.Services
{
    public interface IJwtService
    {
        string GenerateToken(string username, List<Claim> claims);
        ClaimsPrincipal? ValidateToken(string token);
    }
}

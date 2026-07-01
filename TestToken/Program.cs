using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;

class Program
{
    static void Main()
    {
        string token = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJzdXBlcmFkbWluIiwidW5pcXVlX25hbWUiOlsic3VwZXJhZG1pbiIsInN1cGVyYWRtaW4iXSwibmFtZSI6InN1cGVyYWRtaW4iLCJyb2xlIjpbIlN1cGVyQWRtaW4iLCJTdXBlckFkbWluIl0sIkNsdWJJZCI6IjAiLCJuYmYiOjE3ODI5MjU1NzUsImV4cCI6MTc4Mjk0MzU3NSwiaWF0IjoxNzgyOTI1NTc1fQ._V2sxLjrraWD2Ru_8eSHQmD-TSYb0kM289UPM8nPSwFPdztbItPnCrsQDZyIUoj5DP1Z1UPrv3WNUiBEXIpiXQ";
        
        var handler2 = new JsonWebTokenHandler();
        var validationParameters = new TokenValidationParameters 
        { 
            ValidateSignatureLast = true, 
            SignatureValidator = (t,p) => new JsonWebToken(t),
            RoleClaimType = "role",
            NameClaimType = "unique_name",
            ValidateAudience = false,
            ValidateIssuer = false
        };
        var result = handler2.ValidateToken(token, validationParameters);
        Console.WriteLine("IsValid: " + result.IsValid);
        Console.WriteLine("Name: " + result.ClaimsIdentity.Name);
        Console.WriteLine("HasRole: " + result.ClaimsIdentity.HasClaim(result.ClaimsIdentity.RoleClaimType, "SuperAdmin"));
    }
}

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;

class Program
{
    static void Main()
    {
        string token = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJzdXBlcmFkbWluIiwidW5pcXVlX25hbWUiOiJzdXBlcmFkbWluIiwicm9sZSI6IlN1cGVyQWRtaW4iLCJDbHViSWQiOiIwIiwibmJmIjoxNzgyOTI0NjI1LCJleHAiOjE3ODI5NDI2MjUsImlhdCI6MTc4MjkyNDYyNX0.rqC6oQ3UdNzrDXMgg7txTcsdWPtxBau2LujqqiCtGbZfiZIZ0sxmSptO2ijng5IoigKLHeGBzShh93CiW4Ln7Q";
        
        var handler1 = new JwtSecurityTokenHandler();
        var principal1 = handler1.ValidateToken(token, new TokenValidationParameters { ValidateSignatureLast = true, SignatureValidator = (t,p) => new JwtSecurityToken(t) }, out _);
        Console.WriteLine("JwtSecurityTokenHandler claims:");
        foreach(var c in principal1.Claims) Console.WriteLine(c.Type + " = " + c.Value);
        Console.WriteLine("Name: " + principal1.Identity.Name);
        Console.WriteLine("HasRole: " + principal1.IsInRole("SuperAdmin"));

        var handler2 = new JsonWebTokenHandler();
        var result = handler2.ValidateToken(token, new TokenValidationParameters { ValidateSignatureLast = true, SignatureValidator = (t,p) => new JsonWebToken(t) });
        Console.WriteLine("\nJsonWebTokenHandler claims:");
        foreach(var c in result.ClaimsIdentity.Claims) Console.WriteLine(c.Type + " = " + c.Value);
        Console.WriteLine("Name: " + result.ClaimsIdentity.Name);
        Console.WriteLine("HasRole: " + result.ClaimsIdentity.HasClaim(result.ClaimsIdentity.RoleClaimType, "SuperAdmin"));
    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Models.HrUsers;
using Shared.Models.Requests;
using UKG.HCM.Application.Services;

namespace UKG.HCM.WebApi.Services;


public interface IJwtTokenGenerator
{
    /// Generate
    string Generate(TelegramAuthRequests user, HrUserListItemDal? hrUser);
}


internal class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtOptions _options;
    private readonly IProvideCurrentDateTime _time;

    public JwtTokenGenerator(
        IOptions<JwtOptions> options,
        IProvideCurrentDateTime time)
    {
        _time = time;
        _options = options.Value;
    }

    public string Generate(TelegramAuthRequests account, HrUserListItemDal? hrUser)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, account.Id.ToString()),
            new(ClaimTypes.Name, account.Username),
            new(ClaimTypes.GivenName, account.First_name),
            new(ClaimTypes.Surname, account.Last_name),
        };

        if (hrUser != null)
        {
            claims.Add(new Claim("alias", hrUser.Alias));
            claims.Add(new Claim("position", hrUser.Position));
            claims.Add(new Claim("hrUserId", hrUser.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Role, "hr"));

        }
        
        var key = new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(_options.SigningKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: _time.DateTimeOffsetNow()
                .AddHours(24 * 7)
                .ToUniversalTime().DateTime,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DokWokApi.BLL.Infrastructure;

public class TokenValidationParametersAccessor
{
    private readonly IConfiguration _configuration;

    private readonly object _locker = new();

    private TokenValidationParameters? _regular;

    private TokenValidationParameters? _refresh;

    public TokenValidationParametersAccessor(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public TokenValidationParameters Regular
    {
        get
        {
            if (_regular is null)
            {
                lock (_locker)
                {
                    _regular ??= new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = _configuration["Jwt:Issuer"],
                        ValidAudience = _configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
                        ClockSkew = TimeSpan.Zero,
                    };
                }
            }

            return _regular;
        }
    }

    public TokenValidationParameters Refresh
    {
        get
        {
            if (_refresh is null)
            {
                lock (_locker)
                {
                    _refresh ??= new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = false,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = _configuration["Jwt:Issuer"],
                        ValidAudience = _configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
                        ClockSkew = TimeSpan.Zero,
                    };
                }
            }

            return _refresh;
        }
    }
}

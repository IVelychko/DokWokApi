using Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Domain.Helpers;

public class TokenValidationParametersAccessor
{
    private readonly IConfiguration _configuration;

    private readonly object _locker = new();

    private const string ErrorMessage = "Unable to get data from configuration";

    private TokenValidationParameters? _regular;

    private TokenValidationParameters? _refresh;

    public TokenValidationParametersAccessor(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Blocker Vulnerability", "S6781:JWT secret keys should not be disclosed", Justification = "<Pending>")]
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
                        ValidIssuer = _configuration["Jwt:Issuer"] ?? throw new ConfigurationException(ErrorMessage),
                        ValidAudience = _configuration["Jwt:Audience"] ?? throw new ConfigurationException(ErrorMessage),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]
                            ?? throw new ConfigurationException(ErrorMessage))),
                        ClockSkew = TimeSpan.Zero,
                    };
                }
            }

            return _regular;
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Blocker Vulnerability", "S6781:JWT secret keys should not be disclosed", Justification = "<Pending>")]
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
                        ValidIssuer = _configuration["Jwt:Issuer"] ?? throw new ConfigurationException(ErrorMessage),
                        ValidAudience = _configuration["Jwt:Audience"] ?? throw new ConfigurationException(ErrorMessage),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]
                            ?? throw new ConfigurationException(ErrorMessage))),
                        ClockSkew = TimeSpan.Zero,
                    };
                }
            }

            return _refresh;
        }
    }
}

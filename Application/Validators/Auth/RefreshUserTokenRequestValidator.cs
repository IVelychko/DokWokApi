using System.IdentityModel.Tokens.Jwt;
using Domain.Abstractions.Repositories;
using Domain.Abstractions.Services;
using Domain.Constants;
using Domain.DTOs.Requests.Users;
using Domain.Entities;
using Domain.Shared;
using FluentValidation;

namespace Application.Validators.Auth;

public sealed class RefreshUserTokenRequestValidator : AbstractValidator<RefreshUserTokenRequest>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ISecurityTokenService<User, JwtSecurityToken> _securityTokenService;
    private readonly TokenValidationParametersAccessor _tokenValidationParametersAccessor;
    private JwtSecurityToken? _jwtSecurityToken;
    private RefreshToken? _refreshTokenEntity;

    public RefreshUserTokenRequestValidator(
        IRefreshTokenRepository refreshTokenRepository,
        ISecurityTokenService<User, JwtSecurityToken> securityTokenService,
        TokenValidationParametersAccessor tokenValidationParametersAccessor)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _securityTokenService = securityTokenService;
        _tokenValidationParametersAccessor = tokenValidationParametersAccessor;

        RuleLevelCascadeMode = CascadeMode.Stop;
        
        RuleFor(x => x.Token)
            .NotEmpty()
            .Must(IsAlgorithmValid)
            .WithMessage("The algorithm is not valid or a token validation failed")
            .Must(IsTokenExpired)
            .WithMessage("The security token has not expired yet");

        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .Matches(RegularExpressions.Guid)
            .MustAsync(RefreshTokenExists)
            .WithMessage("The refresh token does not exist")
            .Must(x => DateTime.UtcNow < _refreshTokenEntity!.ExpiryDate)
            .WithMessage("The refresh token has expired")
            .Must(x => !_refreshTokenEntity!.Invalidated)
            .WithMessage("The refresh token has been invalidated")
            .Must(x => !_refreshTokenEntity!.Used)
            .WithMessage("The refresh token has been used")
            .Must(JwtMatches)
            .WithMessage("The refresh token does not match the JWT");
    }

    private bool IsAlgorithmValid(string securityToken)
    {
        try
        {
            _jwtSecurityToken = _securityTokenService.ValidateToken(
                securityToken, _tokenValidationParametersAccessor.Refresh);
        }
        catch
        {
            return false;
        }
        
        return _securityTokenService.IsTokenSecurityAlgorithmValid(_jwtSecurityToken);
    }
    
    private bool IsTokenExpired(string securityToken)
    {
        return _jwtSecurityToken!.ValidTo < DateTime.UtcNow;
    }

    private async Task<bool> RefreshTokenExists(string refreshToken, CancellationToken token)
    {
        _refreshTokenEntity = await _refreshTokenRepository.GetByTokenAsNoTrackingAsync(refreshToken);
        return _refreshTokenEntity is not null;
    }

    private bool JwtMatches(RefreshUserTokenRequest command, string refreshToken)
    {
        JwtSecurityToken jwtSecurityToken;
        try
        {
            jwtSecurityToken = _securityTokenService.ValidateToken(
                command.Token, _tokenValidationParametersAccessor.Refresh);
        }
        catch
        {
            return false;
        }
        
        var matches = _refreshTokenEntity!.JwtId ==
                      jwtSecurityToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value;
        return matches;
    }
}

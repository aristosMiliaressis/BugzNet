using MediatR;
using Microsoft.AspNetCore.Identity;

using Microsoft.Extensions.Logging;
using BugzNet.Infrastructure.MediatR;
using BugzNet.Infrastructure.DataEF;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using BugzNet.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BugzNet.Core.Entities.Identity;

namespace BugzNet.Application.MediatR.Requests.Identity.Commands
{
    public class GenerateTokenCommand : IRequest<CommandResponse<TokenResponse>>
    {
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }
        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }
    }

    public class GenerateTokenCommandHandler : IRequestHandler<GenerateTokenCommand, CommandResponse<TokenResponse>>
    {
        private readonly BugzNetDataContext _dbContext;
        private readonly SignInManager<BugzUser> _signInManager;
        private readonly ILogger<GenerateTokenCommandHandler> _logger;
        private readonly TokenProviderOptions _options;
        public GenerateTokenCommandHandler(BugzNetDataContext dbContext, SignInManager<BugzUser> signInManager, 
            ILogger<GenerateTokenCommandHandler> logger, AppConfig appConfig)
        {
            _dbContext = dbContext;
            _signInManager = signInManager;
            _logger = logger;
            _options = new TokenProviderOptions
            {
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appConfig.TokenSigningKey)), SecurityAlgorithms.HmacSha384)
            };
        }

        public async Task<CommandResponse<TokenResponse>> Handle(GenerateTokenCommand request, CancellationToken cancellationToken)
        {
            var user = _dbContext.Users
                            .Include(t => t.UserRoles)
                            .ThenInclude(t => t.Role)
                            .FirstOrDefault(user => user.UserName == request.Username);

            var identity = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

            if (identity == null || !identity.Succeeded)
            {
                _logger.LogError("Invalid User name or Password.");
                return CommandResponse<TokenResponse>.WithError("Invalid User name or Password.");
            }

            _logger.LogInformation("Get Token Identity validated.");

            var now = DateTime.Now;

            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, now.ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.Exp, now.Add(_options.Expiration).ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.Sub, request.Username),
                new Claim(JwtRegisteredClaimNames.Actort, user.Id),
                new Claim(ClaimTypes.Name, request.Username),
                new Claim(ClaimTypes.Role, user.UserRoles.First().Role.Name)
            };

            // Create the JWT and write it to a string
            var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(_options.Expiration),
                signingCredentials: _options.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new TokenResponse
            {
                access_token = encodedJwt,
                expires_in = (int)_options.Expiration.TotalSeconds
            };

            // Serialize and return the response

            _logger.LogInformation("Token Generated!");

            return CommandResponse<TokenResponse>.Ok(response);

        }
    }

    public class TokenProviderOptions
    {
        public string Path { get; set; } = "/api/token";
        public string Issuer { get; set; } = "BugzIssuer";
        public string Audience { get; set; } = "BugzAudience";
        public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(60);
        public SigningCredentials SigningCredentials { get; set; } 
    }

    public class TokenResponse
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
    }
}

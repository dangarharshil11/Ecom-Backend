using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace IdentityService._2.BusinessLogic.Helpers
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddAppAuthentication(this WebApplicationBuilder builder)
        {
            var settingsSection = builder.Configuration.GetSection("Jwt");

            string? secret = settingsSection.GetValue<string>("Key");
            string? issuer = settingsSection.GetValue<string>("Issuer");
            string? audience = settingsSection.GetValue<string>("Audience");

            var key = Encoding.UTF8.GetBytes(secret ?? "N+RQPbnPa4pL+ldbTeELbuBq8B6OhtnNS68O+HjKfoXyov342ntYkvtyc0aEb+5/");

            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    AuthenticationType = "Jwt",
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    ClockSkew = TimeSpan.Zero
                };
            });
            return builder;
        }
    }
}

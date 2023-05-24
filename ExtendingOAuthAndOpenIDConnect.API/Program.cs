using ApiHost;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddJwtBearer(options =>
      {
          options.Authority = "https://localhost:5001";
          options.Audience = "demoapi";
          options.TokenValidationParameters = new()
          {
              NameClaimType = "given_name",
              RoleClaimType = "role",
              ValidTypes = new[] { "at+jwt" }
          };
      });

// layers DPoP onto the "Bearer" scheme above
// builder.Services.ConfigureDPoPTokensForScheme(JwtBearerDefaults.AuthenticationScheme);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

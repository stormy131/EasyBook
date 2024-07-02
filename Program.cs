using Microsoft.EntityFrameworkCore;
using EasyBook.Models;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EasyBook.Services;
using Microsoft.OpenApi.Models;
using EasyBook.Identity;

var builder = WebApplication.CreateBuilder(args);

// Import app configuration
var bearer_config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional:false)
    .Build();

// Database context
builder.Services.AddDbContext<EasyBookContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("EasyBookContext")));

builder.Services.AddControllers().AddJsonOptions(opts =>
    opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Auth & Autorize
builder.Services.AddTransient<AuthService>();

builder.Services.AddAuthentication(options =>{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters{
        ValidIssuer = bearer_config["JwtBearer:Issuer"],
        ValidAudience = bearer_config["JwtBearer:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(bearer_config["JwtBearer:PrivateKey"]!)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true
    };
});

builder.Services.AddAuthorization(options =>{
    options.AddPolicy(IdentityData.AdminUserPolicy, p => {
        p.RequireClaim(IdentityData.AdminUserClaim, "True");
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition(
        name: JwtBearerDefaults.AuthenticationScheme,
        securityScheme: new OpenApiSecurityScheme{
            Name="Authorization",
            Description="Bearer Authorization: `Bearer JWT`",
            In=ParameterLocation.Header,
            Type=SecuritySchemeType.ApiKey,
            Scheme="Bearer"
        }
    );

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement{
            {
                new OpenApiSecurityScheme{
                    Reference = new OpenApiReference{
                        Type=ReferenceType.SecurityScheme,
                        Id=JwtBearerDefaults.AuthenticationScheme
                    }
                },
                new string[]{}
            }
        }
    );
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, config =>
	{
		config.TokenValidationParameters = new TokenValidationParameters()
		{
			ClockSkew = TimeSpan.FromSeconds(5),
			ValidateAudience = false
		};

		config.Audience = "receiverMicroservice";
		config.Authority = "https://localhost:7181";
		config.RequireHttpsMetadata = false;
		config.SaveToken = true;
	});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();
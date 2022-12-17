using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RestCommunications.IdentityServer.Configuration;
using RestCommunications.IdentityServer.DbContext;
using RestCommunications.IdentityServer.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(config =>
{
	config.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"));
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
	{
		options.Password.RequireDigit = true;
		options.Password.RequiredLength = 8;
		options.Password.RequireLowercase = true;
		options.Password.RequireUppercase = true;
		options.Password.RequiredUniqueChars = 3;
	})
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddDefaultTokenProviders();

builder.Services.AddIdentityServer(options =>
	{
		options.UserInteraction.LoginUrl = "/Account/Login";
		options.UserInteraction.LogoutUrl = "/Account/Logout";

		options.Events.RaiseSuccessEvents = true;
		options.Events.RaiseFailureEvents = true;
		options.Events.RaiseInformationEvents = true;
		options.Events.RaiseErrorEvents = true;

		options.EmitStaticAudienceClaim = true;
	})
	.AddDeveloperSigningCredential()
	.AddInMemoryClients(Configuration.GetClients())
	.AddInMemoryApiScopes(Configuration.GetApiScopes())
	.AddInMemoryApiResources(Configuration.GetApiResources())
	.AddInMemoryIdentityResources(Configuration.GetIdentityResources())
	.AddAspNetIdentity<ApplicationUser>();

var app = builder.Build();

app.UseCors(config =>
{
	config.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
});

app.UseAuthentication();
app.UseAuthorization();

app.UseIdentityServer();

app.MapDefaultControllerRoute();

app.Run();
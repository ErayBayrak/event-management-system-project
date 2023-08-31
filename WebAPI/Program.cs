using Business.Abstract;
using Business.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string tokenKey = builder.Configuration.GetSection("AppSettings:Token").Value;
string audience = builder.Configuration.GetSection("AppSettings:Audience").Value;
string issuer = builder.Configuration.GetSection("AppSettings:Issuer").Value;
SecurityKey key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(tokenKey));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidIssuer = audience,
    ValidAudience = issuer,
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = key
}
);


builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IUserDal, EfUserDal>();
builder.Services.AddScoped<IUserService, UserManager>();
builder.Services.AddScoped<IAuthService, AuthManager>();
builder.Services.AddScoped<ICategoryDal, EfCategoryDal>();
builder.Services.AddScoped<ICategoryService, CategoryManager>();
builder.Services.AddScoped<ICityDal, EfCityDal>();
builder.Services.AddScoped<ICityService, CityManager>();
builder.Services.AddScoped<IEventDal, EfEventDal>();
builder.Services.AddScoped<IEventService, EventManager>();
builder.Services.AddScoped<IAttendanceDal, EfAttendanceDal>();
builder.Services.AddScoped<IAttendanceService, AttendanceManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

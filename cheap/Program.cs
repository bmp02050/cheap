using System.Text;
using System.Threading.RateLimiting;
using cheap;
using cheap.Entities;
using cheap.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

var builder = WebApplication.CreateBuilder(args);

var jwtSettingSection = builder.Configuration.GetSection("Jwt");
builder.Services
    .Configure<Jwt>(jwtSettingSection)
    .AddScoped<Jwt>()
    .AddAutoMapper(typeof(AutoMapperProfile))
    .AddTransient<IUserService, UserService>()
    .AddTransient<IEmailService, EmailService>()
    .AddTransient<ITokenService, TokenService>()
    .AddTransient<IBaseService<Record>, RecordService>()
    .AddTransient<IBaseService<Location>,LocationService>()
    .AddTransient<IBaseService<Item>, ItemService>()
    .AddDbContext<Context>(options =>
    {
        options.UseNpgsql(builder.Configuration["ConnectionStrings:dev"]);
        options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
    })
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
    {
        options.PermitLimit = 5;
        options.Window = TimeSpan.FromSeconds(60);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 1;
    });
});

builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
);

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console());

builder.Services.AddSwaggerGen().AddReverseProxy();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.UseRateLimiter();
app.UseEndpoints(endpoints => endpoints.MapControllers());
//app.MapReverseProxy();
await app.RunAsync();
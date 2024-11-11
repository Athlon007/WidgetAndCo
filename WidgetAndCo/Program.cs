using System.Text;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WidgetAndCo.Business;
using WidgetAndCo.Core.Docs;
using WidgetAndCo.Core.Interfaces;
using WidgetAndCo.Core.Mapper;
using WidgetAndCo.Data;
using WidgetAndCo.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add DB context.
builder.Services.AddDbContext<WidgetStoreDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Setup JWT.
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(option =>
    {
        option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"] ?? throw new InvalidOperationException()))
        };
    });

// Add AutoMapper.
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Inject repositories.
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IBlobRepository, BlobRepository>();
var connectionString = builder.Configuration.GetConnectionString("AzureTableStorageConnection") ?? throw new InvalidOperationException("AzureTableStorageConnection environment variable is not set.");
var tableStorageOrderTableName = builder.Configuration.GetSection("TableStorage").GetValue<string>("OrderTableName") ?? throw new InvalidOperationException("TableStorageOrderTableName environment variable is not set.");
var tableStorageOrderProductTableName = builder.Configuration.GetSection("TableStorage").GetValue<string>("OrderProductTableName") ?? throw new InvalidOperationException("TableStorageOrderTableName environment variable is not set.");
builder.Services.AddSingleton<IOrderRepository>(sp => new OrderRepository(new TableClient(connectionString,
        tableStorageOrderTableName)));
builder.Services.AddSingleton<IOrderProductRepository>(sp => new OrderProductRepository(
    new TableClient(connectionString,
        tableStorageOrderProductTableName)));
// Inject services.
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.\n\nExample: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            Array.Empty<string>()
        }
    });

    // Add Review Function Swagger doc
    options.DocumentFilter<ReviewStoreDocFilter>();

});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<WidgetStoreDbContext>();
    dbContext.Database.Migrate();
    await dbContext.SeedDataAsync();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
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

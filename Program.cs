using Microsoft.OpenApi.Models;

namespace fotofolioAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            {
                Args = args,
                ContentRootPath = Directory.GetCurrentDirectory()
            });

            // Load configuration from appsettings.json and environment variables
            builder.Configuration
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                   .AddEnvironmentVariables();

            // Read CORS origins from environment/config (support multiple origins)
            var corsOrigins = builder.Configuration["CorsOrigin"]?.Split(",") ?? new[] { "http://localhost:5173" };

            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("fotofolio", policy =>
                {
                    policy.WithOrigins(corsOrigins)
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // Swagger setup
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Fotofolio API",
                    Version = "v1",
                    Description = "API documentation for Fotofolio application"
                });

                options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    Description = "API Key needed to access the endpoints. X-API-KEY: {apiKey}",
                    In = ParameterLocation.Header,
                    Name = "X-API-KEY",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "ApiKeyScheme"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "ApiKey"
                            },
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });

            var app = builder.Build();

            // CORS
            app.UseCors("fotofolio");

            // Swagger UI
            app.UseSwagger();
            app.UseSwaggerUI();

            // Middleware & Pipeline
            app.UseHttpsRedirection(); // will auto fallback if HTTPS isn't configured
            app.UseMiddleware<ApiKeyMiddleware>();
            app.UseAuthorization();

            app.MapControllers();

            // Optional: Debug current values (remove in prod)
            Console.WriteLine("Loaded CORS origins: " + string.Join(", ", corsOrigins));
            Console.WriteLine("Loaded API key: " + builder.Configuration["ApiKey"]);
            Console.WriteLine("DB Connection: " + builder.Configuration.GetConnectionString("DefaultConnection"));

            app.Run();
        }
    }
}

using AirQualityIndex.Interfaces;
using AirQualityIndex.Services;
using AspNetCoreRateLimit;
using DotNetEnv;
using StackExchange.Redis;

namespace AirQualityIndex;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        Env.Load();
        
        services.AddControllers();

        services.AddTransient<IAirQualityService, AirQualityService>();

        // Register Swagger
        services.AddSwaggerGen();

        // Enable CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
                builder => builder
                    .WithOrigins("https://localhost:44356/")
                    .WithMethods("GET")
                    .AllowAnyHeader());
        });
        
        //var redisConnectionString = configuration.GetSection("Redis:ConnectionString").Value;

        // if (redisConnectionString == null)
        // {
        //     throw new NullReferenceException("Redis connection string is null");
        // }
        
        //services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
        //services.AddScoped<IRedisService, RedisService>();
        services.AddHttpClient();
        services.AddMemoryCache();
        services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
        services.AddInMemoryRateLimiting();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        services.AddHealthChecks();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        //if (env.IsDevelopment())
        //{
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Air Quality API v1"));
        //}
        //else
        //{
            //app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        //}

        
        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseCors("AllowSpecificOrigin");

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
        
        app.UseIpRateLimiting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks("/health");
        });
    }
}
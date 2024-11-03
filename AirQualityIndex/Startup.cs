using AirQualityIndex.Interfaces;
using AirQualityIndex.Services;

namespace AirQualityIndex;

public class Startup // add redis database
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        services.AddTransient<IApiService, ApiService>();

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
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Air Quality API v1"));
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseCors("AllowSpecificOrigin");

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
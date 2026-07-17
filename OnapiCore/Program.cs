using Microsoft.EntityFrameworkCore;
using OnapiCore.Models;

namespace OnapiCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            builder.Services.AddDbContext<OnapiCoreContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("OnapiCoreConnection")));
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("PermitirTodo", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            });


            var app = builder.Build();
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    var errorFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
                    var error = errorFeature?.Error;
                    await context.Response.WriteAsJsonAsync(new { mensaje = "Ocurrió un error interno", detalle = error?.Message });
                });
            });
            app.Use(async (context, next) =>
            {
                await next(); // deja que la petición siga su camino normal primero

                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<OnapiCoreContext>();

                db.Auditorias.Add(new OnapiCore.Models.Auditoria
                {
                    Metodo = context.Request.Method,
                    Ruta = context.Request.Path,
                    CodigoRespuesta = context.Response.StatusCode,
                    Fecha = DateTime.Now
                });

                await db.SaveChangesAsync();
            });
            app.UseCors("PermitirTodo");
            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.MapControllers();

            var summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            app.MapGet("/weatherforecast", (HttpContext httpContext) =>
            {
                var forecast = Enumerable.Range(1, 5).Select(index =>
                    new WeatherForecast
                    {
                        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        TemperatureC = Random.Shared.Next(-20, 55),
                        Summary = summaries[Random.Shared.Next(summaries.Length)]
                    })
                    .ToArray();
                return forecast;
            });
            app.Run();
        }
    }
}

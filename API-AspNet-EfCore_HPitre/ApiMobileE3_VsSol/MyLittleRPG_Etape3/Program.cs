using Microsoft.EntityFrameworkCore;

namespace MyLittleRPG_Etape3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Connexion à la base de données
            builder.Services.AddDbContext<Data.Context.RPGContext>(options =>
            {
                string connectionString = builder.Configuration.GetConnectionString("Default");

                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllLocal", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCors("AllowAllLocal");

            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}

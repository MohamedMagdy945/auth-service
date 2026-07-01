using auth.Infrastructure;
using auth.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace auth.api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddInfrastructureServices(builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.


            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

                Console.WriteLine("Applying migrations...");

                db.Database.Migrate();

                Console.WriteLine("Migration completed");
            }


            if (app.Environment.IsDevelopment())
            {
                //app.UseSwaggerDocumentation();

            }
            using (var scope = app.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
                await seeder.SeedAsync();
            }
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

using IndexCardWebpage.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

namespace IndexCardWebpage
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			var indexCardConnectionString = builder.Configuration.GetConnectionString("IndexCardConnection") ??
				throw new InvalidOperationException("Connection string 'IndexCardConnection' not found.");

			// IndexCardContext hinzufügen (SQLite)
			builder.Services.AddDbContext<CardContext>(options =>
				options.UseSqlite(indexCardConnectionString));
            builder.Services.AddScoped<IndexCardService>();
            builder.Services.AddScoped<CardCategoriesService>();
            // Add services to the container.
            builder.Services.AddRazorComponents()
				.AddInteractiveServerComponents();

            builder.Services.AddHttpClient();

            builder.Services.AddMudServices();

			var app = builder.Build();

			using (var scope = app.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				var context = services.GetRequiredService<CardContext>();
				context.Database.EnsureCreated(); // Stellt sicher, dass die Datenbank erstellt wird
			}

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Error");
				
			}

			app.UseStaticFiles();
			app.UseAntiforgery();

			app.MapRazorComponents<App>()
				.AddInteractiveServerRenderMode();

			app.Run();
		}
	}
}
	
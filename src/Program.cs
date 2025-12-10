using BackOffice.DependencyInjections;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);


		builder.Services.AddControllers();

		builder.Services.AddOpenApi();
		

		builder.AddBackOffice();

		var app = builder.Build();

// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment())
		{
			app.MapOpenApi();
			app.UseSwaggerUi(options => { options.DocumentPath = "/openapi/v1.json"; });
		}

		app.UseHttpsRedirection();

		app.UseAuthorization();

		app.AddBackOffice();

		app.MapControllers();

		app.Run();
	}
}

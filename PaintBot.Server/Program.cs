using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Xml.XPath;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PaintBot.Server.Features.Errors;
using PaintBot.Server.Models;
using PaintBot.Server.PastGames;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options => options.AddErrorExcpetionFilter())
	.AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
	.ConfigureApiBehaviorOptions(options => options.AddValidationErrorResponseFilter());

builder.Services.AddDbContext<GamesContext>(options => options.UseInMemoryDatabase("Games"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	var documentationFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	var documentationDocument = new XPathDocument(Path.Combine(AppContext.BaseDirectory, documentationFilename));
	options.IncludeXmlComments(() => documentationDocument);
	options.OperationFilter<ErrorResponseDocumentation>(documentationDocument);
});

var app = builder.Build();

SeedDatabase(app.Services);

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();

static void SeedDatabase(IServiceProvider serviceProvider)
{
	using var scope = serviceProvider.CreateScope();
	var context = scope.ServiceProvider.GetRequiredService<GamesContext>();
	var games = Enumerable.Range(0, 3)
		.Select(i => new Game { ID = Guid.NewGuid(), DateTime = DateTime.UtcNow.AddHours(-i) })
		.ToArray();
	var bot = new Bot { Name = "TestBot", Games = games };
	foreach (var game in games)
	{
		game.Bots = new[] { bot };
	}
	context.Games.AddRange(games);
	context.Bots.Add(bot);
	context.SaveChanges();
}

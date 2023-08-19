using System.Data;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NextInLine.Server.Exceptions;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

var cnTemplate = builder.Configuration.GetConnectionString("DefaultConnection")!;
var actualPassword = Environment.GetEnvironmentVariable("POSTGRES_PWD");
var cn = cnTemplate.Replace("{ENV_POSTGRES_PWD}", actualPassword);
builder.Services.AddTransient<IDbConnection>(_ => new NpgsqlConnection(cn));

//lower case routes
builder.Services.Configure<RouteOptions>(options => { options.LowercaseUrls = true; });

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(e => e.Value is { Errors.Count: > 0 })
            .ToDictionary(e => e.Key, e => e.Value!.Errors.Select(error => error.ErrorMessage).ToArray());

        var result = new BadRequestObjectResult(errors);
        return result;
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (contextFeature != null)
        {
            context.Response.StatusCode = contextFeature.Error switch
            {
                ItemNotFoundException => StatusCodes.Status404NotFound,
                ArgumentNullException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };
            await context.Response.WriteAsync(contextFeature.Error.Message);
        }
    });
});

app.MapControllers();

app.Run();
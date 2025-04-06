using RubikCube.Services;
using RubikCube.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddOpenApiDocument();

var configuration = builder.Configuration;

builder.Services.AddCors(options =>
{
    options.AddPolicy("Policy",
        builder =>
        {
            builder.WithOrigins(configuration["AllowedCorsOrigins"].Split(','))
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

builder.Services.AddScoped<ICubeService, CubeService>();

var app = builder.Build();

if (app.Environment.IsProduction())
{
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseOpenApi();

app.UseSwagger();

app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("Policy");

app.UseAuthorization();

app.MapControllers();

app.Run();
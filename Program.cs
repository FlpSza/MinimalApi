using Microsoft.AspNetCore.SignalR;
using minimalApi.Infraestrutura.Db;
using minimalApi.Dominio.DTOs;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DbContexto>(options => {
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql")));
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (LoginDTO loginDTO) => {

if(loginDTO.Email == "adm@teste.com" && loginDTO.Senha == "123456")
    return Results.Ok("Login realizado");
else
    return Results.Unauthorized();

});



app.Run();


public class LoginDTO
{
    public string Email { get;set; } = default;
    public string Senha { get;set; } = default;
    
}
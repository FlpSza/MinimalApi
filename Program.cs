using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (minimalApi.Dominio.DTOs.LoginDTO loginDTO) => {

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
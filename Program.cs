using Microsoft.AspNetCore.SignalR;
using minimalApi.Infraestrutura.Db;
using minimalApi.Dominio.DTOs;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using minimalApi.Dominio.Interfaces;
using minimalApi.Dominio.Servicos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using minimalApi.Dominio.DTOs.ModelViews;
using minimalApi.Dominio.Entidades;

var builder = WebApplication.CreateBuilder(args);

// Adicionando o serviço IAdministradorServicos ao contêiner
builder.Services.AddScoped<IAdministradorServicos, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServicos, VeiculoServico>();

// Configurando o DbContext com MySQL
builder.Services.AddDbContext<DbContexto>(options => {
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql")));
});

// Adicionar serviços ao contêiner
builder.Services.AddControllers();

// Configurar Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Minha API",
        Description = "Exemplo de API com ASP.NET Core 8.0 e Swagger"
    });
});

var app = builder.Build();

// Configurar o middleware do Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Minha API v1");
        c.RoutePrefix = "swagger"; // Define Swagger no caminho /swagger
    });
}


app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

// Endpoint GET na raiz
app.MapGet("/", () => {
    return Results.Json(new Home()); // Retorna o objeto Home como JSON
}).WithTags("Home");

// Endpoint POST para login
app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServicos administradorServicos) => {
    if (administradorServicos.Login(loginDTO) != null)
        return Results.Ok("Login realizado");
    else
        return Results.Unauthorized();
}).WithTags("Login");

ErrosDeValidacao validaDTO(VeiculoDTO veiculoDTO){

    var validacao = new ErrosDeValidacao();

    if(string.IsNullOrEmpty(veiculoDTO.Nome)){
        validacao.Mensagens.Add("O nome nao pode ser vazio.");
    }

    if(string.IsNullOrEmpty(veiculoDTO.Marca)){
        validacao.Mensagens.Add("A marca nao pode ser vazio.");
    }

    if(veiculoDTO.Ano <= 1980){
        validacao.Mensagens.Add("Veiculo muito antigo.");
    }
    return validacao;
}; 

// Endpoint POST para Veiculos
app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServicos veiculoServicos) => {

    var validacao = new ErrosDeValidacao();

    if(string.IsNullOrEmpty(veiculoDTO.Nome)){
        validacao.Mensagens.Add("O nome nao pode ser vazio.");
    }

    if(string.IsNullOrEmpty(veiculoDTO.Marca)){
        validacao.Mensagens.Add("A marca nao pode ser vazio.");
    }

    if(veiculoDTO.Ano <= 1980){
        validacao.Mensagens.Add("Veiculo muito antigo.");
    }

    validacao = validaDTO(veiculoDTO);
    if(validacao.Mensagens.Count > 0){
        return Results.BadRequest(validacao);
    }

    var veiculo = new Veiculo{
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };
    veiculoServicos.Incluir(veiculo);

    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);  
}).WithTags("Veiculos");

app.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculoServicos veiculoServicos) => {
    var veiculos = veiculoServicos.Todos(pagina);

    return Results.Ok(veiculos);  
}).WithTags("Veiculos");

app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServicos veiculoServicos) => {
    var veiculo = veiculoServicos.BuscaPorId(id);

    if(veiculo == null)
        return Results.NotFound();

    return Results.Ok(veiculo);  
}).WithTags("Veiculos");

app.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculoServicos veiculoServicos) => {
    
    var validacao = validaDTO(veiculoDTO);
    if(validacao.Mensagens.Count > 0){
        return Results.BadRequest(validacao);
    }
    
    var veiculo = veiculoServicos.BuscaPorId(id);

    if(veiculo == null)
        return Results.NotFound();

    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = veiculoDTO.Ano;

    veiculoServicos.Atualizar(veiculo);

    return Results.Ok(veiculo);  
}).WithTags("Veiculos");

app.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoServicos veiculoServicos) => {
    var veiculo = veiculoServicos.BuscaPorId(id);

    if(veiculo == null)
        return Results.NotFound();

    veiculoServicos.Apagar(veiculo);

    return Results.NoContent();  
}).WithTags("Veiculos");


app.Run();

// DTO para Login
public class LoginDTO
{
    public string Email { get; set; } = default!;
    public string Senha { get; set; } = default!;
}

// Modelo para a página Home
public struct Home
{
    public string Doc { get => "/swagger"; }
    public string Mensagem { get => "Bem vindo a API de VEICULOS"; }
}

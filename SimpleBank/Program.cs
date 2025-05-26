using SimpleBank.Application.Comands; // Para pegar o assembly do MediatR
using SimpleBank.Application.Behaviors; // Para o ValidationBehavior
using SimpleBank.Application.Interfaces; // Para as estrat�gias
using SimpleBank.Application.Strategies; // Para as implementa��es das estrat�gias
using SimpleBank.Domain.Interfaces; // Para o IUnitOfWork e Reposit�rios
using SimpleBank.Infra.Data; // Para o UnitOfWork
using SimpleBank.Domain.Interfaces.Repositories; // Para as interfaces de reposit�rio
using FluentValidation; // Para o FluentValidation
using MediatR; // Se precisar de alguma configura��o direta, mas o Extensions j� ajuda
using Microsoft.OpenApi.Models; // Para o Swagger Security (j� deve estar l�)
using System.Reflection;
using SimpleBank.Infra.Repositories; // Para pegar o assembly do FluentValidation

var builder = WebApplication.CreateBuilder(args);

// Adicionar services ao cont�iner.

// 1. Configurar o MediatR
// Ele vai escanear o assembly onde est� o "CreateUserCommand" (ControleFinanceiro.Application)
// e registrar todos os IRequest e IRequestHandler que encontrar.
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateUserCommand).Assembly));

// 2. Configurar o FluentValidation
// Ele vai escanear o mesmo assembly para encontrar os validadores (CreateUserCommandValidator).
builder.Services.AddValidatorsFromAssembly(typeof(CreateUserCommand).Assembly);

// 3. Registrar o ValidationBehavior no pipeline do MediatR (Chain of Responsibility)
// Isso garante que todo IRequest passe pelo ValidationBehavior antes de chegar no Handler.
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// 4. Registrar Reposit�rios e UnitOfWork
// Geralmente, os reposit�rios s�o Scoped ou Transient. Como o UnitOfWork engloba eles,
// e no nosso caso com Dapper cada reposit�rio abre sua conex�o, podemos registrar como Transient.
// Para um UoW que gerencia uma �nica conex�o/transa��o, ele seria Scoped.
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IFinancialAccountRepository, FinancialAccountRepository>();
builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
builder.Services.AddTransient<ITransactionRepository, TransactionRepository>();

// 5. Registrar as Estrat�gias (Strategy Pattern)
// Registra todas as implementa��es de ITransactionFeeStrategy
builder.Services.AddTransient<ITransactionFeeStrategy, CreditCardFeeStrategy>();
builder.Services.AddTransient<ITransactionFeeStrategy, PixFeeStrategy>();
// Adicione outras estrat�gias aqui conforme for criando

// 6. Configurar a string de conex�o do PostgreSQL
// Adicione esta linha (ou verifique se ela j� existe) para carregar a string de conex�o
// do appsettings.json.
// Exemplo de appsettings.json:
// {
//   "ConnectionStrings": {
//     "DefaultConnection": "Host=localhost;Port=5432;Database=controlefinanceiro;Username=sua_user;Password=sua_senha"
//   }
// }
// Para um banco na nuvem (Render, Supabase, etc.), a string de conex�o ser� bem maior.
builder.Services.AddNpgsqlDataSource(builder.Configuration.GetConnectionString("DefaultConnection")
                                     ?? throw new InvalidOperationException("DefaultConnection string not found."));

// 7. Configura��o do Swagger (verifique o que voc� j� tinha e ajuste)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts =>
{
    opts.SwaggerDoc("v1", new OpenApiInfo { Title = "SimpleBank API", Version = "v1" });

    // Configura��o para autentica��o JWT (Bearer Token)
    opts.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Por favor, insira 'Bearer ' e o token JWT no campo",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    opts.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2", // Padr�o OAuth2
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>() // Permiss�es (vazio para qualquer escopo)
        }
    });

    // Opcional: Adicionar coment�rios XML do projeto da API no Swagger
    // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    // opts.IncludeXmlComments(xmlPath);
});


var app = builder.Build();

// Configurar o pipeline de requisi��o HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization(); // Para usar autentica��o/autoriza��o

app.MapControllers();

app.Run();
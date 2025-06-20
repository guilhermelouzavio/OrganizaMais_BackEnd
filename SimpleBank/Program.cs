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
using SimpleBank.Infra.Repositories;
using SimpleBank.Application.Comands.Users; // Para pegar o assembly do FluentValidation
using SimpleBank.Infra.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Adicionar services ao cont�iner.

// 1. Configurar o MediatR
// Ele vai escanear o assembly onde est� o "CreateUserCommand" (SimpleBank.Application)
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

// --- Configura��o do Servi�o de Token (JWT) ---
builder.Services.AddTransient<ITokenService, TokenService>();

// --- Configura��o da Autentica��o JWT ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Mudar para true em produ��o
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]!)),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true, // Valida se o token expirou
        ClockSkew = TimeSpan.Zero // N�o permite "desvio" de tempo na expira��o
    };
});

builder.Services.AddAuthorization(); // Habilita o uso de [Authorize]

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


// --- Adicionar Middleware de Autentica��o e Autoriza��o ---
app.UseAuthentication(); // DEVE VIR ANTES DE UseAuthorization()
app.UseAuthorization();

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
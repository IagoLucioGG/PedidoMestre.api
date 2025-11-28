using Microsoft.EntityFrameworkCore;
using PedidoMestre.Api.Middleware;
using PedidoMestre.Data;
using PedidoMestre.Services.Interfaces;
using PedidoMestre.Services.Implementation.Usuarios;
using PedidoMestre.Services.Implementation.Empresas;
using PedidoMestre.Services.Implementation.Clientes;
using PedidoMestre.Services.Implementation.Geral;

var builder = WebApplication.CreateBuilder(args);

// Configuração do DbContext com PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuração de Controllers
builder.Services.AddControllers();

// Configuração dos Services - Usuários
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IPerfilService, PerfilService>();
builder.Services.AddScoped<IUsuarioPerfilService, UsuarioPerfilService>();

// Configuração dos Services - Empresas
builder.Services.AddScoped<IEmpresaService, EmpresaService>();
builder.Services.AddScoped<ILojaService, LojaService>();

// Configuração dos Services - Clientes
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IAuthService, PedidoMestre.Services.Implementation.Clientes.AuthService>();

// Configuração dos Services - Pedidos
builder.Services.AddScoped<IPedidoService, PedidoMestre.Services.Implementation.Pedidos.PedidoService>();
builder.Services.AddScoped<IKdsService, PedidoMestre.Services.Implementation.Pedidos.KdsService>();

// Configuração dos Services - Empresas
builder.Services.AddScoped<IFormaPagamentoService, PedidoMestre.Services.Implementation.Empresas.FormaPagamentoService>();

// Configuração dos Services - Estoque
builder.Services.AddScoped<IEstoqueService, PedidoMestre.Services.Implementation.Estoque.EstoqueService>();

// Configuração dos Services - Caixa
builder.Services.AddScoped<ICaixaService, PedidoMestre.Services.Implementation.Caixa.CaixaService>();

// Configuração dos Services - Desempenho
builder.Services.AddScoped<IDesempenhoService, PedidoMestre.Services.Implementation.Desempenho.DesempenhoService>();

// Configuração dos Services - Geral
builder.Services.AddScoped<ITaxaEntregaService, TaxaEntregaService>();

// Configuração dos Services - Produtos
builder.Services.AddScoped<ICategoriaService, PedidoMestre.Services.Implementation.Produtos.CategoriaService>();
builder.Services.AddScoped<IProdutoService, PedidoMestre.Services.Implementation.Produtos.ProdutoService>();
builder.Services.AddScoped<IVariacaoService, PedidoMestre.Services.Implementation.Produtos.VariacaoService>();
builder.Services.AddScoped<IAdicionalService, PedidoMestre.Services.Implementation.Produtos.AdicionalService>();
builder.Services.AddScoped<IProdutoVariacaoService, PedidoMestre.Services.Implementation.Produtos.ProdutoVariacaoService>();

// Configuração do HttpClient para Geocodificação (AddHttpClient já registra o service)
builder.Services.AddHttpClient<IGeocodificacaoService, PedidoMestre.Services.Implementation.Geral.GeocodificacaoService>();

// Configuração do HttpClient para BairroService (AddHttpClient já registra o service)
builder.Services.AddHttpClient<IBairroService, PedidoMestre.Services.Implementation.Geral.BairroService>();

// Configuração do HttpClient para CnpjService (AddHttpClient já registra o service)
builder.Services.AddHttpClient<ICnpjService, PedidoMestre.Services.Implementation.External.CnpjService>();

// Configuração de Autenticação e Autorização JWT
var jwtSecretKey = builder.Configuration["Jwt:SecretKey"] ?? "PedidoMestre_SecretKey_SuperSegura_2024_Minimo32Caracteres";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "PedidoMestre";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "PedidoMestreClients";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(jwtSecretKey))
    };
});

builder.Services.AddAuthorization();

// Configuração do Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Pedido Mestre API",
        Version = "v1",
        Description = "API para gerenciamento de pedidos de delivery",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Suporte",
            Email = "suporte@pedidomestre.com.br"
        }
    });

    // Incluir comentários XML (se houver)
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Configurar para usar camelCase nos JSON
    c.CustomSchemaIds(type => type.FullName?.Replace("+", "."));

    // Configurar segurança JWT no Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer. Exemplo: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Middleware de tratamento de erros
app.UseMiddleware<ErrorHandlingMiddleware>();

// Configuração do Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pedido Mestre API v1");
        c.RoutePrefix = string.Empty; // Swagger UI na raiz (opcional: use "swagger" para /swagger)
        c.DisplayRequestDuration();
        c.EnableDeepLinking();
        c.EnableFilter();
        c.EnableValidator();
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

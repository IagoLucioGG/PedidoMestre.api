# Pedido Mestre API

Sistema de gestão de pedidos para delivery desenvolvido em .NET Core com PostgreSQL.

## 📋 Descrição

API RESTful para gerenciamento completo de pedidos de delivery, incluindo:
- Gestão de empresas e lojas
- Cadastro de clientes e usuários
- Catálogo de produtos com categorias, variações e adicionais
- Sistema de pedidos e controle de cozinha (KDS)
- Gestão de entregas e taxas por bairro
- Sistema de avaliações
- Controle de caixa e movimentações financeiras
- Autenticação JWT para clientes e usuários

## 🛠️ Tecnologias

- **.NET 9.0** - Framework principal
- **Entity Framework Core** - ORM para acesso a dados
- **PostgreSQL** - Banco de dados relacional
- **JWT** - Autenticação e autorização
- **Swagger/OpenAPI** - Documentação da API
- **Polly** - Biblioteca de resiliência para chamadas HTTP

## 📁 Estrutura do Projeto

```
PedidoMestre/
├── PedidoMestre.api/              # Camada de API (Controllers, Program.cs)
├── PedidoMestre.services/         # Camada de serviços (Business Logic)
│   ├── Interfaces/                # Interfaces dos serviços
│   └── Implementation/           # Implementações dos serviços
│       ├── Usuarios/             # Serviços de usuários
│       ├── Empresas/             # Serviços de empresas e lojas
│       ├── Clientes/             # Serviços de clientes
│       ├── Produtos/             # Serviços de produtos
│       └── Geral/                # Serviços gerais (geocodificação, etc.)
├── PedidoMestre.data/             # Camada de dados (DbContext, Migrations)
└── PedidoMestre.models/           # Camada de modelos (Entities, DTOs)
    ├── Common/                   # Modelos comuns (ResponseModel, Login)
    ├── Empresas/                 # Modelos de empresas e lojas
    ├── Usuarios/                 # Modelos de usuários e perfis
    ├── Clientes/                 # Modelos de clientes e endereços
    ├── Produtos/                 # Modelos de produtos, categorias, etc.
    ├── Pedidos/                  # Modelos de pedidos e KDS
    ├── Geral/                    # Modelos gerais (Bairros, Taxas)
    ├── Caixa/                    # Modelos de caixa
    └── Avaliacoes/               # Modelos de avaliações
```

## 🚀 Como Executar

### Pré-requisitos

- .NET 9.0 SDK
- PostgreSQL 12 ou superior
- Visual Studio 2022, VS Code ou Rider

### Configuração

1. Clone o repositório:
```bash
git clone <url-do-repositorio>
cd PedidoMestre
```

2. Configure a string de conexão no arquivo `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=PedidoMestre;Username=seu_usuario;Password=sua_senha"
  }
}
```

3. Configure as chaves JWT no `appsettings.json`:
```json
{
  "Jwt": {
    "SecretKey": "sua_chave_secreta_minimo_32_caracteres",
    "Issuer": "PedidoMestre",
    "Audience": "PedidoMestreClients",
    "ExpirationHours": 24
  }
}
```

4. Execute as migrations:
```bash
cd PedidoMestre.api
dotnet ef database update
```

5. Execute a aplicação:
```bash
dotnet run
```

6. Acesse a documentação Swagger:
```
https://localhost:5001/swagger
```

## 📚 Endpoints Principais

### Autenticação
- `POST /api/Auth/login-cliente` - Login de cliente
- `POST /api/Auth/login-usuario` - Login de usuário (gestor)

### Empresas e Lojas
- `GET /api/Empresas` - Listar empresas
- `POST /api/Empresas` - Criar empresa
- `GET /api/Lojas` - Listar lojas
- `POST /api/Lojas` - Criar loja

### Usuários
- `GET /api/Usuarios` - Listar usuários
- `POST /api/Usuarios` - Criar usuário
- `GET /api/Perfis` - Listar perfis
- `POST /api/Perfis` - Criar perfil

### Clientes
- `GET /api/Clientes` - Listar clientes
- `POST /api/Clientes` - Criar cliente

### Produtos
- `GET /api/Categorias` - Listar categorias
- `POST /api/Categorias` - Criar categoria
- `GET /api/Produtos` - Listar produtos
- `POST /api/Produtos` - Criar produto
- `GET /api/Variacoes` - Listar variações
- `POST /api/Variacoes` - Criar variação
- `GET /api/Adicionais` - Listar adicionais
- `POST /api/Adicionais` - Criar adicional

## 🔐 Autenticação

A API utiliza JWT (JSON Web Tokens) para autenticação. Para acessar endpoints protegidos:

1. Faça login usando `/api/Auth/login-cliente` ou `/api/Auth/login-usuario`
2. Copie o token retornado
3. No Swagger, clique em "Authorize" e cole o token no formato: `Bearer {seu_token}`

## 📝 DTOs (Data Transfer Objects)

Todos os endpoints POST utilizam DTOs simplificados que recebem apenas IDs para relacionamentos:

- `EmpresaCreateDto` - Criação de empresa
- `LojaCreateDto` - Criação de loja
- `UsuarioCreateDto` - Criação de usuário
- `ClienteCreateDto` - Criação de cliente
- `CategoriaCreateDto` - Criação de categoria
- `ProdutoCreateDto` - Criação de produto
- `VariacaoCreateDto` - Criação de variação
- `AdicionalCreateDto` - Criação de adicional
- `PerfilCreateDto` - Criação de perfil
- `UsuarioPerfilCreateDto` - Criação de vínculo usuário-perfil

## 🔄 Migrations

Para criar uma nova migration:
```bash
cd PedidoMestre.api
dotnet ef migrations add NomeDaMigration --project ../PedidoMestre.data
```

Para aplicar migrations:
```bash
dotnet ef database update --project ../PedidoMestre.data
```

## 🧪 Testes

(Adicionar informações sobre testes quando implementados)

## 📦 Dependências Principais

- `Microsoft.EntityFrameworkCore` - ORM
- `Npgsql.EntityFrameworkCore.PostgreSQL` - Provider PostgreSQL
- `Microsoft.AspNetCore.Authentication.JwtBearer` - Autenticação JWT
- `Swashbuckle.AspNetCore` - Swagger/OpenAPI
- `Polly` - Resiliência HTTP

## 🏗️ Arquitetura

O projeto segue uma arquitetura em camadas:

1. **API Layer** - Controllers, middleware, configuração
2. **Services Layer** - Lógica de negócio, validações
3. **Data Layer** - DbContext, migrations, acesso a dados
4. **Models Layer** - Entidades, DTOs, modelos comuns

## 🔧 Funcionalidades Especiais

### Geocodificação Automática
- Ao criar uma loja, o sistema automaticamente obtém coordenadas (latitude/longitude) do endereço
- Utiliza APIs gratuitas (Nominatim) com retry automático

### Criação Automática de Bairros
- Ao criar uma loja, o sistema busca todos os bairros da cidade
- Calcula automaticamente as taxas de entrega baseadas na distância
- Utiliza a API do IBGE para obter dados de bairros

### Validação de CNPJ
- Validação automática de CNPJ ao criar/atualizar empresa
- Utiliza APIs gratuitas (BrasilAPI, OpenCNPJ, CNPJá) com fallback
- Preenchimento automático do nome fantasia

### Cálculo de Taxa de Entrega
- Cálculo dinâmico baseado na distância entre loja e bairro
- Considera taxa por KM configurável por empresa
- Seleção automática da loja mais próxima

## 📄 Licença

(Adicionar informação de licença)

## 👥 Contribuidores

(Adicionar informações dos contribuidores)

## 📞 Suporte

(Adicionar informações de contato/suporte)


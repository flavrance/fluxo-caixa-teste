# Fluxo de Caixa API

API para gerenciamento de fluxo de caixa, permitindo o registro de créditos e débitos, além da geração de relatórios diários e por período.

## Arquitetura

Este projeto foi desenvolvido seguindo os princípios da **Clean Architecture** combinada com o padrão **Ports and Adapters (Hexagonal Architecture)**. Esta abordagem garante:

- **Testabilidade**: Camadas desacopladas facilitam testes unitários
- **Escalabilidade**: Possibilita substituição de tecnologias sem afetar a lógica de negócios
- **Baixo acoplamento**: Domínio independente de detalhes de implementação

### Diagrama da Solução

```
┌─────────────────────────────────────────────────────────────────────────┐
│                                                                         │
│  ┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐   │
│  │                 │     │                 │     │                 │   │
│  │  FluxoCaixa.API │     │ FluxoCaixa.Worker│     │  Clients/UI    │   │
│  │                 │     │                 │     │                 │   │
│  └────────┬────────┘     └────────┬────────┘     └─────────────────┘   │
│           │                       │                                     │
│           ▼                       ▼                                     │
│  ┌─────────────────────────────────────────────────────────┐           │
│  │                                                         │           │
│  │             FluxoCaixa.Infrastructure.IoC               │           │
│  │                                                         │           │
│  └─────────────────────────────────────────────────────────┘           │
│           │                       │                                     │
│           ▼                       ▼                                     │
│  ┌─────────────────────────────────────────────────────────┐           │
│  │                                                         │           │
│  │             FluxoCaixa.Application.Core                 │           │
│  │                                                         │           │
│  └─────────────────────────────────────────────────────────┘           │
│           │                       │                                     │
│           ▼                       ▼                                     │
│  ┌─────────────────────────────────────────────────────────┐           │
│  │                                                         │           │
│  │               FluxoCaixa.Domain.Core                    │           │
│  │                                                         │           │
│  └─────────────────────────────────────────────────────────┘           │
│           │                       │                                     │
│           ▼                       ▼                                     │
│  ┌─────────────────────────────────────────────────────────┐           │
│  │                                                         │           │
│  │            FluxoCaixa.Infrastructure.Data               │           │
│  │                                                         │           │
│  └─────────────────────────────────────────────────────────┘           │
│           │                       │                                     │
│           ▼                       ▼                                     │
│  ┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐   │
│  │                 │     │                 │     │                 │   │
│  │   SQL Server    │     │    MongoDB      │     │     Redis       │   │
│  │   PostgreSQL    │     │                 │     │                 │   │
│  └─────────────────┘     └─────────────────┘     └─────────────────┘   │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

### Fluxo de Processamento Assíncrono

```
┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│                 │     │                 │     │                 │
│  API Request    │────▶│    RabbitMQ     │────▶│     Worker      │
│                 │     │                 │     │                 │
└─────────────────┘     └─────────────────┘     └────────┬────────┘
                                                         │
                                                         ▼
                                               ┌─────────────────┐
                                               │                 │
                                               │  Circuit Breaker│
                                               │                 │
                                               └────────┬────────┘
                                                         │
                                                         ▼
                                               ┌─────────────────┐
                                               │                 │
                                               │  Report Service │
                                               │                 │
                                               └────────┬────────┘
                                                         │
                                                         ▼
                                               ┌─────────────────┐
                                               │                 │
                                               │  Database/Cache │
                                               │                 │
                                               └─────────────────┘
```

### Camadas da Aplicação

1. **Domain Core**: Contém as entidades, interfaces e regras de negócio
   - Entidades: Representam os objetos de domínio (CashFlow, Credit, Debit, Report)
   - Interfaces: Definem contratos para repositórios e serviços
   - Value Objects: Objetos imutáveis que representam conceitos do domínio

2. **Application Core**: Contém os serviços, DTOs e casos de uso
   - Services: Implementam a lógica de negócio
   - DTOs: Objetos de transferência de dados
   - Commands/Queries: Implementação do padrão CQRS
   - Validators: Validação de dados de entrada

3. **Infrastructure Data**: Contém a implementação dos repositórios e configurações
   - Repositories: Implementação dos repositórios
   - Context: Configuração do Entity Framework
   - Migrations: Migrações do banco de dados
   - Cache: Implementação de cache com Redis
   - Messaging: Implementação de mensageria com RabbitMQ
   - NoSQL: Implementação de armazenamento NoSQL com MongoDB

4. **Infrastructure IoC**: Contém a configuração de injeção de dependência
   - DependencyInjection: Configuração de serviços e repositórios

5. **API**: Contém os controladores e configurações da API
   - Controllers: Endpoints da API
   - Middlewares: Componentes de pipeline HTTP
   - Filters: Filtros de ação e exceção

6. **Worker**: Contém o serviço de processamento assíncrono
   - DailyConsolidationWorker: Processa o consolidado diário de forma assíncrona
   - Circuit Breaker: Implementação de resiliência com Polly

## Tecnologias Utilizadas

- **Backend**:
  - .NET 8.0
  - ASP.NET Core Web API
  - Entity Framework Core 8.0
  - MediatR (Mediator pattern)
  - FluentValidation
  - Serilog (Logging)

- **Banco de Dados**:
  - SQL Server / PostgreSQL (relacional)
  - MongoDB (NoSQL para logs)
  - Redis (Cache)

- **Mensageria**:
  - RabbitMQ

- **Resiliência**:
  - Polly (Circuit Breaker)

- **Testes**:
  - xUnit
  - Moq
  - Bogus

- **Documentação**:
  - Swagger / OpenAPI

- **Containerização**:
  - Docker
  - Docker Compose

## Funcionalidades

### Fluxo de Caixa

- Criação, atualização e exclusão de fluxos de caixa
- Adição de créditos e débitos
- Consulta de fluxos de caixa por ID e data

### Relatórios

- Relatórios diários de fluxo de caixa
- Relatório consolidado por data
- Relatório por período

### Processamento Assíncrono

- Worker Service para processamento do consolidado diário
- Implementação de Circuit Breaker para resiliência
- Reprocessamento automático em caso de falhas

### Cache

- Armazenamento em cache de relatórios e consolidados
- Estratégia de fallback para cache em caso de indisponibilidade do banco

## Como Executar

### Requisitos

- .NET 8.0 SDK
- Docker e Docker Compose (opcional)

### Executando Localmente

1. Clone o repositório:
   ```
   git clone https://github.com/seu-usuario/fluxo-caixa-api.git
   cd fluxo-caixa-api
   ```

2. Restaure os pacotes e compile o projeto:
   ```
   dotnet restore
   dotnet build
   ```

3. Execute as migrações do banco de dados:
   ```
   dotnet ef database update --project src/FluxoCaixa.Infrastructure.Data --startup-project src/FluxoCaixa.API
   ```

4. Execute a API:
   ```
   dotnet run --project src/FluxoCaixa.API
   ```

5. Execute o Worker Service (em outro terminal):
   ```
   dotnet run --project src/FluxoCaixa.Worker
   ```

### Executando com Docker

1. Execute o comando:
   ```
   docker-compose up -d
   ```

2. Acesse a API em:
   ```
   http://localhost:5222/swagger
   ```

## Testes

Para executar os testes unitários:

```
dotnet test
```

## Documentação da API

A documentação da API está disponível através do Swagger:

```
http://localhost:5222/swagger
```

## Estrutura do Projeto

```
/src
 ├── FluxoCaixa.API (Camada de API)
 │   ├── Controllers
 │   ├── Extensions
 │   ├── Middlewares
 │   ├── Filters
 │
 ├── FluxoCaixa.Application.Core (Casos de uso)
 │   ├── Commands
 │   ├── DTOs
 │   ├── Interfaces
 │   ├── Queries
 │   ├── Services
 │   ├── Validators
 │
 ├── FluxoCaixa.Domain.Core (Regras de negócio)
 │   ├── Entities
 │   ├── Exceptions
 │   ├── Interfaces
 │
 ├── FluxoCaixa.Infrastructure.Data (Persistência e outros serviços)
 │   ├── Cache
 │   ├── Context
 │   ├── EntityConfigurations
 │   ├── Messaging
 │   ├── NoSql
 │   ├── Repositories
 │
 ├── FluxoCaixa.Infrastructure.IoC (Injeção de dependência)
 │
 ├── FluxoCaixa.Worker (Processamento assíncrono)
 │
 ├── FluxoCaixa.Tests (Testes unitários e de integração)
     ├── Unit
     ├── Integration
```

## Contribuição

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/nova-feature`)
3. Faça commit das suas alterações (`git commit -m 'Adiciona nova feature'`)
4. Faça push para a branch (`git push origin feature/nova-feature`)
5. Abra um Pull Request

## Licença

Este projeto está licenciado sob a licença MIT - veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## Considerações Técnicas e Desafios

### Mapeamento de Entidades com Entity Framework Core

O projeto utiliza o Entity Framework Core como ORM para acesso a dados. Durante o desenvolvimento, enfrentamos alguns desafios específicos relacionados ao mapeamento de entidades e coleções:

#### Coleções Baseadas em Interfaces

Um dos principais desafios foi o mapeamento de coleções baseadas em interfaces, como a propriedade `Entries` na entidade `CashFlow`, que é do tipo `IReadOnlyCollection<IEntry>`. O Entity Framework Core não consegue mapear automaticamente coleções baseadas em interfaces.

**Solução implementada:**

1. A entidade `CashFlow` mantém uma coleção privada `_entries` do tipo `List<IEntry>` e expõe uma propriedade pública somente leitura `Entries` que retorna `_entries.AsReadOnly()`.

2. Além disso, a entidade possui propriedades de navegação específicas para o EF Core: `Credits` e `Debits`.

3. No repositório, após carregar um `CashFlow` com suas coleções `Credits` e `Debits`, utilizamos o método `AddEntries` para popular a coleção interna `_entries`, garantindo que a propriedade `Entries` retorne todos os dados corretamente.

```csharp
// Exemplo de como populamos a coleção _entries após carregar do banco
foreach (var cashFlow in cashFlows)
{
    cashFlow.AddEntries(cashFlow.Credits);
    cashFlow.AddEntries(cashFlow.Debits);
}
```

4. Na configuração do contexto do EF Core, ignoramos a propriedade `Entries` e configuramos explicitamente as relações entre `CashFlow`, `Credit` e `Debit`:

```csharp
// Ignorar a propriedade Entries pois é baseada em interface
entity.Ignore(e => e.Entries);

// Configurar relacionamento com Credit
entity.HasMany(e => e.Credits)
    .WithOne()
    .HasForeignKey(e => e.CashFlowId)
    .OnDelete(DeleteBehavior.Cascade);
```

#### Concorrência e Atualizações

Outro desafio foi lidar com atualizações de entidades que possuem coleções, especialmente ao adicionar novos créditos ou débitos a um fluxo de caixa existente.

**Solução implementada:**

No método `UpdateAsync` do repositório, implementamos uma lógica que:

1. Carrega a entidade existente com suas coleções
2. Atualiza as propriedades básicas
3. Identifica e adiciona novos créditos e débitos
4. Atualiza a coleção interna `_entries` para manter a consistência

```csharp
// Atualizamos a coleção _entries interna para garantir consistência
existingCashFlow.AddEntries(existingCashFlow.Credits);
existingCashFlow.AddEntries(existingCashFlow.Debits);
```

### Boas Práticas Implementadas

- **Encapsulamento**: Propriedades com setters privados e métodos específicos para modificar o estado das entidades
- **Imutabilidade**: Uso de `IReadOnlyCollection` para expor coleções sem permitir modificações externas
- **Separação de Responsabilidades**: Cada camada com responsabilidades bem definidas
- **Documentação de Código**: Comentários explicativos em classes e métodos complexos

## Solução de Problemas (Troubleshooting)

### Problemas Comuns e Soluções

#### 1. Erro ao adicionar créditos ou débitos: `DbUpdateConcurrencyException`

**Problema**: Ao tentar adicionar um crédito ou débito a um fluxo de caixa existente, ocorre uma exceção de concorrência.

**Solução**: Este erro geralmente ocorre quando há conflitos de concorrência no banco de dados. Verifique:

- Se o fluxo de caixa existe no banco de dados
- Se não há operações concorrentes modificando o mesmo fluxo de caixa
- Se o método `UpdateAsync` do repositório está sendo chamado corretamente

O repositório foi implementado para lidar com esses casos, verificando a existência das entidades antes de tentar atualizá-las.

#### 2. Dados não aparecem nas consultas GET

**Problema**: Ao consultar fluxos de caixa, créditos ou débitos através das APIs GET, os dados não são retornados corretamente.

**Solução**: Este problema pode ocorrer devido a:

- Configuração incorreta do mapeamento no Entity Framework Core
- Falta de inclusão explícita das entidades relacionadas (usando `Include`)
- Problemas com a população da coleção `Entries`

Verifique se o repositório está:
1. Incluindo as coleções relacionadas com `Include()`
2. Chamando o método `AddEntries` para popular a coleção interna `_entries`
3. Não usando `AsNoTracking()` quando precisar acessar as entidades relacionadas

#### 3. Erro ao executar em contêineres Docker

**Problema**: Erros ao executar a aplicação em contêineres Docker, especialmente relacionados à conexão com o banco de dados.

**Solução**:
- Verifique se todos os serviços estão em execução: `docker-compose ps`
- Verifique os logs dos contêineres: `docker-compose logs api` ou `docker-compose logs db`
- Certifique-se de que as variáveis de ambiente estão configuradas corretamente
- Verifique se o banco de dados está acessível dentro da rede Docker

#### 4. Problemas de desempenho em consultas

**Problema**: Consultas lentas, especialmente ao recuperar fluxos de caixa com muitos créditos e débitos.

**Solução**:
- Considere implementar paginação nas consultas
- Use `AsNoTracking()` para consultas somente leitura quando não precisar modificar as entidades
- Implemente cache para consultas frequentes
- Otimize as consultas SQL geradas pelo Entity Framework Core

## Alterações Recentes e Melhorias

### Versão 1.1.0 (Atual)

#### Correções de Bugs
- Corrigido problema com `DbUpdateConcurrencyException` ao adicionar créditos e débitos
- Corrigido problema com dados não aparecendo nas consultas GET
- Melhorada a configuração do Entity Framework Core para evitar problemas de mapeamento

#### Melhorias Técnicas
- Implementado método `AddEntries` na entidade `CashFlow` para popular corretamente a coleção `Entries`
- Melhorada a configuração de relacionamentos no contexto do Entity Framework Core
- Adicionada documentação detalhada no código para facilitar a manutenção
- Otimizado o método `UpdateAsync` do repositório para lidar melhor com entidades relacionadas

#### Melhorias de Desempenho
- Otimizadas as consultas ao banco de dados
- Implementada estratégia adequada para carregamento de entidades relacionadas

### Próximos Passos
- Implementar paginação nas consultas para melhorar o desempenho com grandes volumes de dados
- Adicionar cache para consultas frequentes
- Melhorar a documentação da API
- Implementar testes de integração adicionais
MotoMap API

Integrantes
- Vinicius Sant Anna (556841)  
- Felipe Rosa (557636)
- Pedro Souza (555533)

=====

Justificativa da Arquitetura
A MotoMap API foi desenvolvida em .NET 9 (ASP.NET Core Web API) utilizando boas práticas REST para garantir escalabilidade, desempenho e facilidade de manutenção.  

Decisões principais:
- .NET 9 Web API → moderno, rápido e com suporte a boas práticas REST.  
- Entity Framework Core (Code-First) → simplifica a persistência de dados com PostgreSQL.  
- Banco de Dados PostgreSQL → robusto, open source e confiável para alta escala.  
-Swagger/OpenAPI → documentação interativa e exemplos de uso dos endpoints.  
- Arquitetura em Camadas (Models, DTOs, Controllers, DbContext) → separação clara de responsabilidades.

Entidades principais:
- Motos → representam os veículos monitorados no pátio.  
- Readers → dispositivos leitores de RFID/BLE espalhados no pátio.  
- Yards → pátios/filiais da Mottu onde as motos ficam alocadas.  

Essa arquitetura elimina falhas humanas na localização manual de motos, dá visibilidade em tempo real e é escalável para mais de 100 filiais.

=====

Instruções de execução

1. Pré-requisitos
- .NET 9 SDK
- PostgreSQL

2. Clonar o projeto
git clone https://github.com/ViniciusSantanaa/MotoMap_.NET.git
cd MotoMap.Api

3. Configurar o banco de dados 
Edite o arquivo appsettings.json e configure sua connection string
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=motoMapDb;Username=postgres;Password=*****"
}"

Criar o banco via migrations
Executar no terminal:
dotnet ef database update

4. Executar API
   dotnet run
   
=====

Exemplos de uso

-Para criar uma moto (Post/api/motos)
{
  "plate": "ABC-1234",
  "model": "Honda CG 160",
  "yardId": 1
}

-Para listar uma moto (Get/api/motos)
[
  {
    "id": 1,
    "plate": "ABC-1234"
    "yardId": 1
  }
]

Para executar os testes
No terminal:
dotnet test

Saída esperada do teste:
Passed!  ✔  X tests successful



# Order Processing System — Event-Driven Architecture (.NET 10 & RabbitMQ)
 ![.NET CI](https://github.com/tomas-loesch/OrderProcessingSystem/actions/workflows/ci.yml/badge.svg)

Sistema distribuído para processamento assíncrono de pedidos utilizando **C# (.NET 10)**, **MassTransit**, **RabbitMQ** e **Docker**.

## Arquitetura do Projeto

O sistema aplica o padrão de desacoplamento temporal com mensageria:

`Cliente HTTP` ➔ `OrderApi (Minimal API)` ➔ `RabbitMQ (Exchange/Queue)` ➔ `OrderWorker (Background Service)`

- **`src/OrderApi`**: Recebe requisições HTTP `POST /api/orders`, publica o evento `OrderCreatedEvent` no broker e retorna `202 Accepted` imediatamente.
- **`src/OrderWorker`**: Serviço em background que consome a fila `order-created-queue` com políticas de *Retry* e tolerância a falhas (*Dead-Letter Queue*).
- **`src/SharedContracts`**: Contratos imutáveis (`record`) compartilhados entre produtor e consumidor.
- **`tests/OrderProcessing.Tests`**: Testes unitários com `xUnit` e `Moq`, além de testes de integração em memória utilizando o `MassTransit TestHarness`.

## Tecnologias Utilizadas

- **.NET 10 (C#)** — Minimal APIs & Worker Services
- **MassTransit 8** — Abstração de Service Bus e roteamento AMQP
- **RabbitMQ** — Message Broker
- **xUnit & Moq** — Testes automatizados
- **Docker & Docker Compose** — Orquestração de containers

## Como Executar via Docker Compose

1. Certifique-se de ter o **Docker** em execução.
2. Na raiz do projeto, execute:
   ```bash
   docker compose up --build -d

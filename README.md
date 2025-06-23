
# 📩 Template & Subscription Service - Subscription Platform

Este microserviço permite a criação e gerenciamento de templates de e-mail e subscriptions. Ele valida autenticação via token e envia mensagens para o SQS da AWS para envio imediato ou salva agendamentos no DynamoDB.

## 🧰 Tecnologias Utilizadas
- .NET 9
- Amazon SQS (envio imediato)
- Amazon DynamoDB (agendamentos)
- RESTful API
- Autenticação via JWT

## 📌 Funcionalidades
- CRUD de templates de e-mail
- Criação de subscriptions:
  - Envio imediato via fila SQS
  - Envio agendado salvo no DynamoDB

## 🚀 Execução

```bash
dotnet run --project SubscriptionService
```

## 🔐 Autorização
Este serviço requer token JWT válido fornecido pelo **Auth Service**.

---


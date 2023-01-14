# Report system

this is a very simple test with rabbit mq


You can make how much requests you want, simulating a purchase.
the micro service `PaymentService` will process `3` at time until finish using queues with RabbitMQ.

you can run the web app in `front` folder to see it visual, or look at the logs on payment service terminalat

# Disclaimer

this application is not for production, all the code is only for test and don't reflect on my real skills.

# Running

first of all you need to deploy docker container by:
```bash
docker compose up -d
```

it will deploy two containers:

- rabbitmq `port 5672`, `port 15672 (management)`
- sqlserver 2019 `port 1430`

after deploy the containers, you need to run the Core application. it will create migrations and all you need to run the app

```bash
cd ./ReportSystem/Core
dotnet run
```

the, run the `PaymentService` project

```bash
cd ./ReportSystem/PaymentService
dotnet run
```

you have two endpoints

- `POST /api/v1/payments`

```ts
// create new payment

{
  name: string, // payment name
  price: number, // payment price in cents
}
```

- `GET /api/v1/payments`

```ts
// list payments processing

{
  id: string,
  name: string, // payment name
  price: number, // payment price
  state: "processing" | "success" | "fail" | "retrying", // current state
  createdAt: string // date created
}
```

to run the `front` application 

```bash
cd ./front
npm i
npm start
```

this will open `http://localhost:3000`
# TestRabbitMq

# 🧩 Order Events Processing – MassTransit + RabbitMQ Architecture

## 📌 Project Objective

Build an order event processing system that guarantees:

- 🔒 **Strong ordering per `OrderId`**
- ⚡ **Concurrent processing across different orders**
- 📈 **Horizontal scalability (multiple pods)**
- ♻️ **High availability and automatic failover**

The solution is based on:

- **.NET + MassTransit**
- **RabbitMQ**
- Exchange type **`x-consistent-hash`**

---

# 🏗 Overall Architecture

## 1️⃣ Main Exchange

A single exchange is used:
* order-events-hash
* Type: x-consistent-hash

This exchange distributes messages across multiple queues based on the hash of `OrderId`.

---

## 2️⃣ Processing Queues

N queues are created (example: 4):

* order-events-0
* order-events-1
* order-events-2
* order-events-3

Each queue:
- Is bound to the `order-events-hash` exchange
- Has weight `1` in the hash ring
- Is processed by a single active consumer

---

# 🔄 Event Flow

1. An event is published (`SubmitOrderEvent`, `CancelOrderEvent`, etc.)
2. The `RoutingKey` is set to `OrderId`
3. RabbitMQ calculates:
hash(OrderId) → selects queue
4. The message always lands in the same queue for the same `OrderId`
5. The consumer processes messages sequentially

---

# 🧠 Ordering Guarantee

Each endpoint is configured with:

```csharp
e.ConcurrentMessageLimit = 1;
e.PrefetchCount = 1;
e.SingleActiveConsumer = true;

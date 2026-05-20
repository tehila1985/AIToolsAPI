# Kafka Consumer - Setup & Troubleshooting Guide

## What Was Built

A .NET Console App (`OrderConsumer`) that listens to the `order-notifications` Kafka topic and prints every new order in real time.

---

## Project Structure

```
AIToolsAPI/
├── docker-compose.yml       # Infrastructure: Kafka (KRaft), Redis, Kafka-UI
├── project1/                # Web API - Producer (publishes orders to Kafka)
└── OrderConsumer/           # Console App - Consumer (reads orders from Kafka)
    └── Program.cs
```

---

## How to Run (Every Time)

### 1. Start the infrastructure
```bash
docker compose up -d
```
Wait ~10 seconds for Kafka to be ready.

### 2. Start the Web API (Producer)
```bash
dotnet run --project project1
```
This also auto-creates the `order-notifications` topic on first run.

### 3. Start the Consumer
```bash
dotnet run --project OrderConsumer
```

You should see:
```
[Consumer] Subscribed. Waiting for partition assignment...
[ASSIGNED] order-notifications [[0]]
```

Now every order created via the API will appear instantly in the consumer console.

---

## Full Reset (Clean Slate)

If Kafka gets into a bad state, do a full reset:

```bash
docker compose down -v    # -v removes volumes (wipes all Kafka data)
docker compose up -d
# wait ~10 seconds
dotnet run --project project1    # recreates the topic
dotnet run --project OrderConsumer
```

> **Important:** After `down -v`, the topic is deleted. The Web API must run first to recreate it.

---

## Problems We Solved & Why

### Problem 1 - Consumer connected but received nothing (silent hang)

**Root cause:** Kafka's internal `__consumer_offsets` topic has a default `replication-factor=3`.
With only 1 broker, Kafka could never create it, so the group coordinator was permanently stuck.
The consumer appeared connected but could never join a consumer group.

**Fix** - added to `docker-compose.yml` under the Kafka service:
```yaml
KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR: '1'
KAFKA_TRANSACTION_STATE_LOG_REPLICATION_FACTOR: '1'
KAFKA_TRANSACTION_STATE_LOG_MIN_ISR: '1'
KAFKA_DEFAULT_REPLICATION_FACTOR: '1'
KAFKA_MIN_INSYNC_REPLICAS: '1'
```

---

### Problem 2 - Topic disappeared after reset

**Root cause:** Running `docker compose down -v` wipes all Docker volumes, including all Kafka topics.

**Fix:** Always start the Web API first after a reset — it recreates the topic automatically.
Or create it manually:
```bash
docker exec kafka kafka-topics --bootstrap-server localhost:9092 \
  --create --topic order-notifications --partitions 1 --replication-factor 1
```

---

## Consumer Config Explained

```csharp
new ConsumerConfig
{
    BootstrapServers = "localhost:9092",
    GroupId = "debug-group-" + Guid.NewGuid(), // fresh group each run
    AutoOffsetReset = AutoOffsetReset.Earliest, // read from beginning if no committed offset
    EnableAutoCommit = false,                   // manual commit for reliability
    BrokerAddressFamily = BrokerAddressFamily.V4, // force IPv4, avoids ::1 vs 127.0.0.1 issues
    SessionTimeoutMs = 45000,
    HeartbeatIntervalMs = 3000,
}
```

---

## Kafka-UI

Visual dashboard to inspect topics, messages, and consumer groups:
```
http://localhost:8080
```

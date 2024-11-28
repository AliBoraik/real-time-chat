# Chat App 

This is a chat application built using .NET Core, and React. It enables real-time messaging between users.

## Technologies Used

List the technologies and frameworks you're using in your project:

- ASP.NET Core
- React
- SignalR
- PostgreSQL
- RabbitMQ
- Docker
- Zenko (S3-compatible storage)
- Redis
## Installation

1. Clone the repository:
2. Build and run the Docker containers:
```shell
docker compose up
```
## Features

List and explain the key features of your application, including:

- Real-time chat using SignalR
- File and metadata upload
- In-memory queue using MassTransit
- Background service for queue processing
- Use of RabbitMQ for messaging
- Integration with Zenko for S3-compatible storage

## Architecture Overview

this project is built using a microservices architecture, a modern approach to designing and building scalable and maintainable applications. In a microservices architecture, the application is broken down into a collection of loosely coupled and independently deployable services. Each service is responsible for a specific business capability and can be developed, deployed, and scaled independently. This architecture promotes flexibility, modularity, and easier maintenance.

The key components and microservices in your application are:

1. **User Authentication Service:** This service handles user authentication and provides secure access to the application. It ensures user identity is managed independently, enabling single sign-on across various components.

2. **Real-time Communication Service (SignalR):** The real-time communication service enables seamless and interactive communication between users. It uses SignalR to establish persistent connections and push real-time updates to clients.

3. **File and Metadata Service:** Responsible for managing the upload, storage, and retrieval of files and associated metadata. This service integrates with Zenko for efficient S3-compatible storage.

4. **Message Processing Service (MassTransit and RabbitMQ):** The MassTransit-based message processing service processes messages from various sources, such as the real-time communication service and file service. RabbitMQ ensures reliable message delivery and communication between different microservices.

6. **Database Service:** Each microservice typically has its own database or data store optimized for its specific needs. This separation enables independent scaling and performance optimization.

7. **Frontend Microservice (React):** The frontend microservice provides the user interface for interacting with the application. It communicates with the backend microservices via APIs and integrates real-time updates using SignalR.


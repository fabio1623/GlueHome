# Delivery Solution

Delivery Solution is a .NET 7 application that provides endpoints to manage deliveries. It is following the DDD pattern. It consists therefore of 3 main projects:

- `DeliveryApi`: the API layer that exposes the endpoints.
- `DeliveryDomain`: the business logic layer that contains the domain models and services.
- `DeliveryInfrastructure`: the infrastructure layer that deals with data persistence using MySQL and message queues using RabbitMQ.

There are also two Unit Tests projects:

- `DeliveryApiTests`: tests the Delivery endpoints.
- `DeliveryDomainTests`: tests the business logic.

## Requirements

- Docker (otherwise you can run it on your IDE)

## Installation and Usage

- Clone the repository to your local machine: ```git clone https://github.com/fabio1623/GlueHome.git```
- Open a terminal and navigate to the `DeliverySolution` directory.
- Build the solution with the following command: ```docker-compose build```
- Start the application by running: ```docker-compose up -d```
- The Delivery API documentation can be accessed at `http://localhost:7145/swagger`
- To stop the application, run the following command: ```docker-compose down```

The above command will start the application as well as all required services.

The following three users are automatically generated to interact with different permissions:

- `Admin` (username: admin, password: admin): manage users and delete deliveries
- `User` (username: user, password: user): create, get, approve, and cancel deliveries
- `Partner` (username: partner, password: partner): create, get, complete, and cancel deliveries

## Available Endpoints

To interact with the API endpoints, you can use tools like Postman or cURL. It uses a Bearer Token.

- `POST /deliveries/create` - Creates a new delivery.
- `GET /deliveries/{orderNumber}` - Gets the delivery by order number.
- `PUT /deliveries/approve/{orderNumber}` - Approves the delivery.
- `PUT /deliveries/complete/{orderNumber}` - Completes the delivery.
- `PUT /deliveries/cancel/{orderNumber}` - Cancels the delivery.
- `DELETE /deliveries/{orderNumber}` - Deletes the delivery by order number.

## MySQL

The users and deliveries are stored in MySQL. You can connect to the MySQL instance using the following credentials:

- Host: localhost
- Port: 3306
- Database: delivery_db
- Username: root
- Password: mysql_root_password

## RabbitMQ

The delivery updates (Created, Approved, Completed, Cancelled, and Deleted) will be pushed to the RabbitMQ exchange.
You can interact with it through `http://localhost:15672`.
If you want to see transiting messages, you can bind a queue to `delivery-exchange` exchange with `*` as routing key to see messages.

## Logging

The Delivery Solution logs information to Elasticsearch. To view the logs, follow these steps:

- Open Kibana at `http://localhost:5601`
- Create an index pattern with `delivery-api-*`
- Explore the logs in Kibana.

## Running the Unit Tests

- Open a terminal and navigate to the `DeliverySolution` directory.
- Build the test projects with the following command: ```dotnet build```
- Run the tests with the following command: ```dotnet test```

The above command will start the test runners and run all the tests in the solution.

## Conclusion

The Delivery Solution is a containerized application that stores deliveries in MySQL and sends delivery updates to RabbitMQ. It provides a RESTful API to create, get, update, and delete deliveries. The solution also includes unit tests to ensure the business logic is working as expected.

## License

This project is licensed under the Creative Commons Attribution-NonCommercial-NoDerivatives 4.0 International License. See the [LICENSE](http://creativecommons.org/licenses/by-nc-nd/4.0/?ref=chooser-v1) file for details.

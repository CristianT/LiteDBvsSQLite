How to use this example:

Prerequisites:
- Docker

Start:
docker-compose -f "docker-compose.yml" up --build

That will start 5 containers:
- RabbitMQ
- Prometheus
- Grafana
- MetricServiceProducer
- MetricServiceHub

Usage:
- Go to http://localhost:3000
- Enter user - password: admin - admin
- Specify a new password for admin
- Click on add data source
- Select prometheus and specify http://prometheus:9090
- Click Save and test
- On the left menu put the mouse on the + icon and click on import
- Click on upload json and upload the file ./DashboardExample/DashboardExample.json
- See the image ./DashboardExample/DashboardExample.png to see how it should look like

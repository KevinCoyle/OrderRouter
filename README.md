Prerequisites
> [Docker](https://docs.docker.com/get-started/get-docker/) / [Podman](https://podman-desktop.io/)
> [PostgreSQL](https://www.postgresql.org/) is required if you are going to run the app locally (rather than in a container), 

Choices:
* Individual Zipcodes are all a 5-digit numerical value, though the ZipCode object can have a SingleValue or RangeValue (indicating a range of Zipcodes)
* There are endpoints for both single Orders to be routed as well as a List of Orders to be routed
  * If multiple orders are being routed, the response will be a List of responses, one for each provided order
  * To route multiple orders in a single call, the URL must append "/list".
* I decided to create a PostgreSQL DB and utilize Entity
  * This means that the ReferenceDocs/ directory is required, and the Dockerfile does include copying that into the build image
    * It is only the products.csv and suppliers.csv files which are 
* The connectionstring details are in the appsettings.json 

Running / Testing
* Start by ensuring your Docker/Podman environment is setup
* Run the following command to compose the system (Database and API)
  * ```docker compose up -d --build --force-recreate```
  * ```podman compose up -d --build --force-recreate```
* Once the pods are up and running, there are examples for calling the API in ```OrderRouter/OrderRouter.Api/OrderRouter.http```
* When finished, run the following command 
  * ```docker compose down -v```
  * ```podman compose down -v```
  * If you don't do so, the containers/volume/etc. will persist and the ```--force-recreate``` portion of the compose up call will cause them to be recreated, so it shouldn't be an issue of duplicating resources



Issues
* I wanted to also set up Testcontainers for Unit/Integration testing, but I was running a bit short on time and figured the manual testing as well as what's in the OrderRouter.http covered it minimally
* I didn't setup the system to handle https calls, I ran into an issue with getting it working with the containers' networking.
* I think the logic could be cleaned up a good bit to reduce on system resources as well as improve readability
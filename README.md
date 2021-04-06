# Pokedex

## Prerequisites
Please install .NET Core 3.1 from 
```bash
https://dotnet.microsoft.com/download/dotnet-core/3.1
```

## Build and execute
* Open a command prompt and navigate to the Pokedex.API folder.  
* Build the application:
```bash
dotnet build Pokedex.API.csproj -c Release
```
* Change folder:
```bash
cd bin/release/netcoreapp3.1
```
Run the service:
```bash
dotnet Pokedex.API.dll
```
The application will be listening on http://localhost:5000  
To stop the service, press Ctrl+C

## Run application using Docker
Navigate to Pokedex folder (which contains the Dockerfile) and execute the following:
```bash
docker build -t pokedex .
```
```bash
docker run -d -p 5000:80 --name pokedexapp pokedex
```
Application should now be listening on port 5000

## Send request for basic pokemon information
Send a GET message to  
**http://localhost:5000/pokemon/{pokemon-name}**  
with Accept header is set to **application/json**

## Send request for pokemon information with translated description
Send a GET message to  **http://localhost:5000/pokemon/translated/{pokemon-name}**  
with Accept header is set to **application/json**

## Improvements for Production
* Add logging 
  * Log as much information as possible around any exceptions thrown
* Use HTTPS instead of HTTP
* Use the resilience framework Polly to make the web requests to Pokeapi and Funtranslation APIs more fault tolerant. Polly can be installed using Nuget.
* Consider adding caching in the following ways:
  * cache data received from Pokeapi and Funtranslation APIs. A third party library could be used to implement this (eg: Polly).
  * set cache-control header on responses which can be used by client side caches.
  * cache outgoing responses in a shared cache.
* Consider using a single endpoint for both endpoints implemented so far. Since these endpoints are returning different representations of the same resource, the use of a vendor-specific media type may be preferable to multiple endpoints. 
* Add Swagger documentation and UI.

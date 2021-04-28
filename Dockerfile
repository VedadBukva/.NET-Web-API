FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY . .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet '.NET Web API.dll'
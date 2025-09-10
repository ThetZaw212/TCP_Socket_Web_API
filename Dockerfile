# Use official .NET 9 SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish TCP_Socket_Web_API.csproj -c Release -o /app/publish

# Use official .NET 9 ASP.NET runtime image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "TCP_Socket_Web_API.dll"]

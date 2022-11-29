FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5000

ENV ASPNETCORE_URLS=http://*:5000

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["HotelApi.csproj", "./"]
RUN dotnet restore "HotelApi.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "HotelApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HotelApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM alpine:3.10
RUN apk add --update sqlite

ENTRYPOINT ["sqlite3"]
CMD ["hotel.db"]

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HotelApi.dll"]

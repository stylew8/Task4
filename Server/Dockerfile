# Базовый образ для среды выполнения .NET
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

# Базовый образ для сборки .NET приложения
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем файл проекта
COPY ["Server.csproj", "./"]

# Восстанавливаем зависимости
RUN dotnet restore "Server.csproj"

# Копируем весь исходный код и собираем проект
COPY . .
RUN dotnet build "Server.csproj" -c Release -o /app/build

# Публикуем приложение
FROM build AS publish
RUN dotnet publish "Server.csproj" -c Release -o /app/publish

# Финальный этап: запускаем приложение
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Server.dll"]

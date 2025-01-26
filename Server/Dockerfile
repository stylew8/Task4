# Базовый образ для выполнения .NET-приложения
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

# Базовый образ для сборки приложения
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем проект и восстанавливаем зависимости
COPY ["Server.csproj", "./"]
RUN dotnet restore "Server.csproj"

# Копируем остальные файлы и собираем проект
COPY . .
RUN dotnet publish "Server.csproj" -c Release -o /app/publish

# Финальный этап: копируем опубликованные файлы
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Server.dll"]

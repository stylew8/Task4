# Базовый образ для выполнения приложения
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5111

# Образ для сборки проекта
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем файл проекта
COPY ["Server.csproj", "./"]

# Восстанавливаем зависимости
RUN dotnet restore "Server.csproj"

# Копируем все файлы
COPY . .

# Публикуем проект в папку /app/publish
RUN dotnet publish "Server.csproj" -c Release -o /app/publish

# Финальная стадия: минимальный образ
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Указываем точку входа
ENTRYPOINT ["dotnet", "Server.dll"]

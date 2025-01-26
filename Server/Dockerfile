# Используем базовый образ .NET Runtime для запуска приложения
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 7156

# Используем SDK для сборки приложения
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ./nginx/ssl /app/ssl

# Копируем файлы проекта и собираем
COPY . .
RUN dotnet restore "Server.csproj"
RUN dotnet publish "Server.csproj" -c Release -o /app/publish

# Переносим собранное приложение в runtime образ
FROM base AS final
WORKDIR /app/publish
COPY --from=build /app/publish .



# Указываем точку входа
ENTRYPOINT ["dotnet", "Server.dll"]

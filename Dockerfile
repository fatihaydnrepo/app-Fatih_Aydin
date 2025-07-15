# 1. Build aşaması
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Çözüm ve proje dosyalarını kopyala
COPY nuget.config ./
COPY Postgresql.sln ./
COPY src/Postgresql.Mvc/*.csproj ./src/Postgresql.Mvc/
COPY src/Postgresql.Mvc/ ./src/Postgresql.Mvc/

# Restore bağımlılıkları
RUN dotnet restore "src/Postgresql.Mvc/Postgresql.Mvc.csproj"

# Publish (release build)
RUN dotnet publish "src/Postgresql.Mvc/Postgresql.Mvc.csproj" -c Release -o /app/publish

# 2. Runtime aşaması
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Uygulamayı başlat
ENTRYPOINT ["dotnet", "Postgresql.Mvc.dll"]
# Uygulamanın dinleyeceği portu belirt
EXPOSE 5000
ENV ASPNETCORE_URLS=http://0.0.0.0:5000
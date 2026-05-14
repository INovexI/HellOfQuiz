# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Sadece csproj dosyasını kopyalayıp bağımlılıkları yüklüyoruz (Cache için)
COPY ["HellOfQuiz.csproj", "./"]
RUN dotnet restore "./HellOfQuiz.csproj"

# Kalan tüm dosyaları kopyalayıp projeyi derliyoruz
COPY . .
WORKDIR "/src/"
RUN dotnet build "./HellOfQuiz.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Projeyi yayına hazır hale getiriyoruz (Publish)
FROM build AS publish
RUN dotnet publish "./HellOfQuiz.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Son aşama: Çalıştırılabilir imajı hazırlıyoruz
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HellOfQuiz.dll"]

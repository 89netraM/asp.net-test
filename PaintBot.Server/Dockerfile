FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PaintBot.Server/PaintBot.Server.csproj", "PaintBot.Server/"]
RUN dotnet restore "PaintBot.Server/PaintBot.Server.csproj"
COPY . .
WORKDIR "/src/PaintBot.Server"
RUN dotnet build "PaintBot.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PaintBot.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaintBot.Server.dll"]

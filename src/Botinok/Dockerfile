FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Botinok/Botinok.fsproj", "Botinok/"]
COPY ["Botinok.Libgen/Botinok.Libgen.fsproj", "Botinok.Libgen/"]
COPY ["Botinok.Common/Botinok.Common.fsproj", "Botinok.Common/"]
RUN dotnet restore "Botinok/Botinok.fsproj"
COPY . .
WORKDIR "/src/Botinok"
RUN dotnet build "Botinok.fsproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Botinok.fsproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Botinok.dll"]

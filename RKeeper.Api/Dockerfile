#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ARG BUILD_MODE=debug
ENV BUILD_MODE=${BUILD_MODE}
RUN echo ${BUILD_MODE}

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["RKeeper/RKeeper.csproj", "RKeeper/"]
RUN dotnet restore "RKeeper/RKeeper.csproj" -v d
COPY . .
WORKDIR "/src/RKeeper"
RUN dotnet build "RKeeper.csproj" -c ${BUILD_MODE} -o /app/build

FROM build AS publish
RUN dotnet publish "RKeeper.csproj" -c ${BUILD_MODE} -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RKeeper.dll"]
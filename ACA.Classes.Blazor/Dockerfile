#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["nuget.config", ""]
COPY ["ACA.Classes.Blazor/ACA.Classes.Blazor.csproj", "ACA.Classes.Blazor/"]
RUN dotnet restore "ACA.Classes.Blazor/ACA.Classes.Blazor.csproj"
COPY . .
WORKDIR "/src/ACA.Classes.Blazor"
RUN dotnet build "ACA.Classes.Blazor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ACA.Classes.Blazor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ACA.Classes.Blazor.dll"]
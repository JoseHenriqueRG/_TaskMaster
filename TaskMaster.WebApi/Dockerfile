# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY TaskMaster.WebApi/TaskMaster.WebApi.csproj ./TaskMaster.WebApi/
COPY TaskMaster.Infra/TaskMaster.Infra.csproj ./TaskMaster.Infra/
COPY TaskMaster.Domain/TaskMaster.Domain.csproj ./TaskMaster.Domain/
COPY TaskMaster.Business/TaskMaster.Business.csproj ./TaskMaster.Business/

RUN dotnet restore TaskMaster.WebApi/TaskMaster.WebApi.csproj

# Copy the entire project and build the app
COPY . ./
RUN dotnet publish -c Release -o out

# Stage 2: Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "TaskMaster.WebApi.dll"]

FROM mcr.microsoft.com/dotnet/sdk:8.0@sha256:8ab06772f296ed5f541350334f15d9e2ce84ad4b3ce70c90f2e43db2752c30f6 AS build
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish src/Sefer.Backend.Donations.Api.csproj -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine@sha256:91106a05a57b3ef509919d0d61206317f63d27b23b666d38668b14ba8485975c
WORKDIR /App
COPY --from=build /App/out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Sefer.Backend.Donations.Api.dll"]
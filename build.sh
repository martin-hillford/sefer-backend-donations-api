#!/bin/bash
dotnet restore
dotnet build --no-restore
dotnet test --no-restore --no-build
dotnet publish src/Sefer.Backend.Donations.Api.csproj --output ./build
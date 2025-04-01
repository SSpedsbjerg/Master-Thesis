FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /app
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /publish


FROM mcr.microsoft.com/dotnet/runtime:9.0 as runtime
WORKDIR /publish
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "REPS.dll", "452455", "30", "localhost:8080"]

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY SmGenPar.Wasm.csproj .
RUN dotnet restore SmGenPar.Wasm.csproj
COPY . .
RUN dotnet build SmGenPar.Wasm.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish SmGenPar.Wasm.csproj -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY nginx.conf /etc/nginx/nginx.conf

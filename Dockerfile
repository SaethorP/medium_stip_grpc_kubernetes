FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY DebitCardApi/DebitCardApi.csproj DebitCardApi/
RUN dotnet restore DebitCardApi/DebitCardApi.csproj

COPY . .
RUN dotnet publish DebitCardApi/DebitCardApi.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=https://+:50051
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/tls.crt
ENV ASPNETCORE_Kestrel__Certificates__Default__KeyPath=/https/tls.key

EXPOSE 50051

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DebitCardApi.dll"]

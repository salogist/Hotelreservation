FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY HotelReservation.sln ./
COPY src/HotelReservation.Domain/*.csproj src/HotelReservation.Domain/
COPY src/HotelReservation.Application/*.csproj src/HotelReservation.Application/
COPY src/HotelReservation.Infrastructure/*.csproj src/HotelReservation.Infrastructure/
COPY src/HotelReservation.Web/*.csproj src/HotelReservation.Web/
RUN dotnet restore

COPY . .
RUN dotnet publish src/HotelReservation.Web/HotelReservation.Web.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "HotelReservation.Web.dll"]

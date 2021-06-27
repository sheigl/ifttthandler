FROM mcr.microsoft.com/dotnet/sdk:5.0 as build
WORKDIR /build

COPY ./ifttthandler.csproj ./

RUN dotnet restore

COPY . .

RUN dotnet build
RUN dotnet publish -o /dist

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime

WORKDIR /app

COPY --from=build /dist ./

ENV RabbitMqOptions_HostName="ifttt_mq"
ENV RabbitMqOptions_UserName="mqadmin"
ENV RabbitMqOptions_VirtualHost="ifttt"

CMD [ "dotnet", "ifttthandler.dll" ]

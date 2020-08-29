#docker run --log-opt max-size=50m --log-opt max-file=4  -d -it --restart unless-stopped --net=host -p 5009:5009 mustafasalih/fortnite:website

FROM mcr.microsoft.com/dotnet/core/aspnet:5.0-focal AS base
RUN apt-get update && apt-get install -y apt-utils libgdiplus libc6-dev
WORKDIR /app
EXPOSE 5009


FROM mcr.microsoft.com/dotnet/core/sdk:5.0-focal AS build
WORKDIR /src
COPY ["Websites/FTNPower.Website/FTNPower.Website.csproj", "Websites/FTNPower.Website/"]
COPY ["Websites/FTNPower.Image.Api.Service/FTNPower.Image.Api.Service.csproj", "Websites/FTNPower.Image.Api.Service/"]
COPY ["FTNPower.Redis/FTNPower.Redis.csproj", "FTNPower.Redis/"]
COPY ["GlobalLibs/MyLogger/Global.csproj", "GlobalLibs/MyLogger/"]
COPY ["FTNPowerApplication/FTNPower.Core/FTNPower.Core.csproj", "FTNPowerApplication/FTNPower.Core/"]
COPY ["Fortnite/Fortnite.Localization/Fortnite.Localization.csproj", "Fortnite/Fortnite.Localization/"]
COPY ["Fortnite/Fortnite.StaticData/Fortnite.Static.csproj", "Fortnite/Fortnite.StaticData/"]
COPY ["Fortnite/Fortnite.Model/Fortnite.Model.csproj", "Fortnite/Fortnite.Model/"]
COPY ["Fortnite/Fortnite.Core/Fortnite.Core.csproj", "Fortnite/Fortnite.Core/"]
COPY ["Fortnite/Fortnite.Api/Fortnite.Api.csproj", "Fortnite/Fortnite.Api/"]
COPY ["Discord.Net-dev/src/Discord.Net.Commands/Discord.Net.Commands.csproj", "Discord.Net-dev/src/Discord.Net.Commands/"]
COPY ["Discord.Net-dev/src/Discord.Net.Core/Discord.Net.Core.csproj", "Discord.Net-dev/src/Discord.Net.Core/"]
COPY ["Discord.Net-dev/src/Discord.Net.WebSocket/Discord.Net.WebSocket.csproj", "Discord.Net-dev/src/Discord.Net.WebSocket/"]
COPY ["Discord.Net-dev/src/Discord.Net.Rest/Discord.Net.Rest.csproj", "Discord.Net-dev/src/Discord.Net.Rest/"]
COPY ["FTNPowerApplication/FTNPower.Model/FTNPower.Model.csproj", "FTNPowerApplication/FTNPower.Model/"]
COPY ["Fortnite/Fortnite.ExternalApi/Fortnite.External.csproj", "Fortnite/Fortnite.ExternalApi/"]
COPY ["FTNPowerApplication/FTNPower.Data/FTNPower.Data.csproj", "FTNPowerApplication/FTNPower.Data/"]
COPY ["FTNPowerApplication/FTNPower.Static/FTNPower.Static.csproj", "FTNPowerApplication/FTNPower.Static/"]
COPY ["Fortnite/Fortnite/Fortnite.csproj", "Fortnite/Fortnite/"]
RUN dotnet restore -nowarn:CS0162,CS0219,CS1998,CS0168,CS8632 "Websites/FTNPower.Website/FTNPower.Website.csproj"
COPY . .
WORKDIR "/src/Websites/FTNPower.Website"
RUN dotnet build -nowarn:CS0162,CS0219,CS1998,CS0168,CS8632 "FTNPower.Website.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FTNPower.Website.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN echo "Etc/UTC" > /etc/timezone
RUN echo "127.0.0.1 linuxserver" >> /etc/hosts
ENTRYPOINT ["dotnet", "FTNPower.Website.dll"]
#docker run --log-opt max-size=50m --log-opt max-file=4 -d -it --restart unless-stopped --net=host -p 5010:5010 -v fortnite-volume:/app/Storage mustafasalih/fortnite:imageservice

FROM mcr.microsoft.com/dotnet/core/aspnet:5.0-focal AS base
RUN apt-get update \
        && echo 'msttcorefonts msttcorefonts/accepted-mscorefonts-eula select true' | debconf-set-selections \
        && apt-get install -y --allow-unauthenticated \
        libc6-dev \
        libgdiplus \
        curl \
        && curl -o ttf-mscorefonts-installer_3.7_all.deb http://ftp.br.debian.org/debian/pool/contrib/m/msttcorefonts/ttf-mscorefonts-installer_3.7_all.deb \
        && apt-get install -y ./ttf-mscorefonts-installer_3.7_all.deb \
        && rm -rf /var/lib/apt/lists/*

WORKDIR /app
EXPOSE 5010

FROM mcr.microsoft.com/dotnet/core/sdk:5.0-focal AS build
WORKDIR /src
COPY ["Fortnite/Fortnite.Model/Fortnite.Model.csproj", "Fortnite/Fortnite.Model/"]
COPY ["Websites/FTNPower.Image.Api/FTNPower.Image.Api.csproj", "Websites/FTNPower.Image.Api/"]
COPY ["Websites/FTNPower.Image.Api.Service/FTNPower.Image.Api.Service.csproj", "Websites/FTNPower.Image.Api.Service/"]
COPY ["Websites/FTNPower.Image.Processing/FTNPower.Image.Processing.csproj", "Websites/FTNPower.Image.Processing/"]
RUN dotnet restore -nowarn:CS0162,CS0219,CS1998,CS0168,CS8632 "Websites/FTNPower.Image.Api/FTNPower.Image.Api.csproj"
COPY . .
WORKDIR "/src/Websites/FTNPower.Image.Api"
RUN dotnet build -nowarn:CS0162,CS0219,CS1998,CS0168,CS8632 "FTNPower.Image.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FTNPower.Image.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN echo "Etc/UTC" > /etc/timezone
RUN echo "127.0.0.1 linuxserver" >> /etc/hosts
ENTRYPOINT ["dotnet", "FTNPower.Image.Api.dll"]
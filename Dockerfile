FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /app

COPY Blog-CSharp.sln ./
COPY Blog-CSharp.csproj ./

COPY appsettings.* ./

RUN dotnet restore

COPY . .

RUN dotnet build

RUN dotnet tool install --global dotnet-ef

ENV PATH="$PATH:/root/.dotnet/tools"

EXPOSE 5000

CMD ["dotnet", "run"]
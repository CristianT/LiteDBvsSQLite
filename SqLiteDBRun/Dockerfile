FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as build-env
WORKDIR /app

COPY ./nuget.config  ./temp_csproj/
COPY ./*/*.csproj ./temp_csproj/
RUN for file in $(ls ./temp_csproj/*.csproj); do dotnet restore ${file}; done

COPY . ./
RUN dotnet publish SqLiteDBRun -c Release -o out

FROM mcr.microsoft.com/dotnet/core/runtime:3.1
WORKDIR /app

COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "SqLiteDBRun.dll"]
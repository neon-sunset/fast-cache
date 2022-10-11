FROM mcr.microsoft.com/dotnet/sdk:7.0
WORKDIR /app
COPY src .
RUN dotnet build FastCache.Benchmarks/FastCache.Benchmarks.csproj -c release -f net7.0

ENV DOTNET_ReadyToRun=0
ENV DOTNET_TieredPGO=1
ENV DOTNET_JitVTableProfiling=1
ENV DOTNET_JitProfileCasts=1

CMD dotnet run --project FastCache.Benchmarks/FastCache.Benchmarks.csproj -c release -f net7.0
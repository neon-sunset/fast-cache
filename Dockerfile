FROM mcr.microsoft.com/dotnet/sdk:8.0.100-preview.5
WORKDIR /app

COPY src .
COPY Directory.Build.props .
RUN dotnet publish FastCache.Benchmarks/FastCache.Benchmarks.csproj \
    -c release \
    -o publish \
    -f net8.0

# Must be run interactively
CMD ./publish/FastCache.Benchmarks
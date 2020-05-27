FROM mcr.microsoft.com/dotnet/core/runtime:3.1
ARG source=.
WORKDIR /app
EXPOSE 50051
COPY $source .
ENTRYPOINT ["dotnet", "KT.MessageFileProcessor.Job.dll"]

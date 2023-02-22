FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /app
COPY src/BugzNet.Core/BugzNet.Core.csproj src/BugzNet.Core/
COPY src/BugzNet.Infrastructure/BugzNet.Infrastructure.csproj src/BugzNet.Infrastructure/
COPY src/BugzNet.Application/BugzNet.Application.csproj src/BugzNet.Application/
COPY src/BugzNet.Web/BugzNet.Web.csproj src/BugzNet.Web/
RUN dotnet restore "src/BugzNet.Web/BugzNet.Web.csproj"

# copy git folder to add tag & commit info to build
COPY .git/ .
COPY src/ src/
RUN dotnet publish "src/BugzNet.Web/BugzNet.Web.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS release
EXPOSE 80
RUN apt-get update \
	&& apt-get install -y libgdiplus

# Install necessary apt + pip packages
RUN apt-get update && apt-get install firefox-esr sudo postgresql python3-pip cron wget -y
RUN pip3 install selenium==3.141.0

# Setup emulated user
COPY ./.emulator/geckodriver /usr/bin/
COPY ./.emulator/emulate_admin.py /app/emulate_admin.py
COPY ./.emulator/emulate_user.py /app/emulate_user.py
RUN chmod +x /usr/bin/geckodriver \
	&& chmod +x /app/emulate_admin.py \
	&& chmod +x /app/emulate_user.py

WORKDIR /app
COPY --from=build /app/publish .

RUN chmod +x /app/entrypoint.sh
ENTRYPOINT /app/entrypoint.sh

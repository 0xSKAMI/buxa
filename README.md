# Buxa

Buxa is discord bot that embeds instagram videos for friend group to watch without leaving discord

> [!IMPORTANT]  
> Buxa is running on cheap server right now and could be slow at moments.

# Features

I want to make universal bot that can embed videos of instagram, tiktok and youtube shorts.

Buxa is build on discordgo library and for it to work proparly it needs permissions: "Send Messages" and "Read Message History".
Buxa does not store any messages in any kind of database for security purposes. it saves video in buffer and and simply sends it.

For this moment buxa only supports insgagram reels (only public ones).

# How do I run it

## Build Prerequisites

Go programminglanguage (at leats version 1.24.3), yt-dlp, git, ffmpeg are required.

> [!NOTE]  
> Buxa without docker at this moment is not included in this README except Debian/Ubuntu.

## Debian/Ubuntu
Run this only if you do not prerequisites installed (does not include golang).
```
curl -L https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp -o ~/.local/bin/yt-dlp
chmod a+rx ~/.local/bin/yt-dlp  # Make executable
sudo apt update
sudo apt install ffmpeg
sudo apt install git
```
One of the ways to run this application is to clone the repo
```
git clone https://github.com/0xSKAMI/buxa.git
cd buxa
touch .env
```
> [!NOTE]  
> To successfully run application you will need valid discord bot token which you have to put in .env file in the root of the project.
#### example of .env
```
TOKEN="{your token}"
```
After that you can build buxa and run it.
```
cd src/
go build main.go
./main
```
## Docker
To run this application in docker you will need docker version 28.3.3 or newer.

```
docker pull 0xskami/buxa:0.1.2
docker run -e TOKEN='{input your bot's token}' 0xskami/buxa
```

# Get in touch and contributing

For now only way to get in touch with me is throu github issues ;)
But I am working on discord server at this moment.

Any contrubion is welcome and Contrubions.md is under development at this moment

# License
Buxa is licensed under MIT license.

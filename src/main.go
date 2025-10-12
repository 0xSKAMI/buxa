package main

import (
	"fmt"
	"os"
	"os/signal"
	"syscall"
	"strings"
	"os/exec"
	"bytes"

	"github.com/bwmarrin/discordgo"
	"github.com/joho/godotenv"
)

//declare map to store messages id and links they send
var linking = map[string]string{};

func main() {
	//importing .env variables
	godotenv.Load("../.env");
	var token string = os.Getenv("TOKEN");

	//craeting discord client
	discord, err := discordgo.New("Bot " + token);
	if err != nil {
		fmt.Println("error creating Discord session,", err)
		return
	}

	//adding handler for indstagram reels
	discord.AddHandler(messageCreate);

	// Open a websocket connection to Discord and begin listening.
	err = discord.Open();

	if err != nil {
		fmt.Println("error opening connection,", err)
		return
	}
	
	//consoling starting of bot
	fmt.Println("bot is now listening");

	//make sure program does not quit
	quitChannel := make(chan os.Signal, 1);
	signal.Notify(quitChannel, syscall.SIGINT, syscall.SIGTERM);
	<-quitChannel;

	//closing connection so threshhold and stuff
	err = discord.Close();
}

func messageCreate(s *discordgo.Session, m *discordgo.MessageCreate) {
	if strings.HasPrefix(m.Content, "https://www.instagram.com/reel/") && m.Author.ID != s.State.User.ID {
		//do some cool operations
		_, ok := linking[m.ID];
		if ok == true {
			fmt.Println("value was in the map already");
			return;
		}
		linking[m.ID] = m.Content;

		cmd := exec.Command("yt-dlp", "-q", "-o", "-", "-t", "mp4", m.Content);

		var out bytes.Buffer;
		cmd.Stdout = &out;
		var err error;
		if err := cmd.Run();  err != nil {
			fmt.Println("error running command", err);
			s.ChannelMessageSend(m.ChannelID, "some error happened");
			return;
		};

		fmt.Println(err);

		s.ChannelFileSendWithMessage(m.ChannelID, "here is your video", "test.mp4", &out);
	}
}

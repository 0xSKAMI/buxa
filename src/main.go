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
	//check if user send link from instagram
	if strings.Contains(m.Content, "https://www.instagram.com/") && m.Author.ID != s.State.User.ID {
		//create array of worlds in the message (if message is like wow https://www.instagram.com/)
		words := strings.Fields(m.Content);
		//this variable will hold link from words
		var link string;

		//this is for getting and setting reel link in link variable
		for _, element := range words {
			if (strings.HasPrefix(element, "https://www.instagram.com/")) {
				link = element;
				break;
			}
		}
		
		//do some cool operations
		_, ok := linking[m.ID];
		if ok == true {
			fmt.Println("value was in the map already");
			return;
		}
		linking[m.ID] = m.Content;

		//creaet command to download video in mp4 format
		cmd := exec.Command("yt-dlp", "-q", "-o", "-", "--cookies", "cookies.txt", "-t", "mp4", link);

		//declare buffer that will hold video
		var out bytes.Buffer;
		cmd.Stdout = &out;
		var err error;
		//error handling (normal)
		if err := cmd.Run();  err != nil {
			fmt.Println("error running command", err);
			s.ChannelMessageSend(m.ChannelID, "some error happened");
			return;
		};

		//developer purposes
		fmt.Println(err);

		//sednig video (.mp4 after the name can be changed to some other more compressed format)
		if uploaded, err := s.ChannelFileSendWithMessage(m.ChannelID, "here is your video", "test.mp4", &out); err != nil {
			s.ChannelMessageSend(m.ChannelID, "file is too large");
			fmt.Println("file is too large:", err);
			fmt.Println(uploaded);
			return;
		}
	}
}

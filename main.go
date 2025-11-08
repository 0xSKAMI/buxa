package main

import (
	"fmt"
	"os"
	"os/signal"
	"syscall"
	"strings"
	"os/exec"
	"bytes"
	"log"
	
	"github.com/bwmarrin/discordgo"
	"github.com/joho/godotenv"
)

func main() {
	//importing .env variables
	godotenv.Load("./.env");
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

		//creaet command to download video in mp4 format
		cmd := exec.Command("yt-dlp", "-q", "-o", "-", "--cookies", "cookies.txt", "-t", "mp4", link);

		//declare buffer that will hold video
		var out bytes.Buffer;
		var stderr bytes.Buffer;
		cmd.Stdout = &out;
		cmd.Stderr = &stderr;
		var err error;
		//error handling (normal)
		if err := cmd.Run();  err != nil {
			fmt.Println(fmt.Sprint(err) + ": " + stderr.String())
			log.Fatal(err);
			s.ChannelMessageSend(m.ChannelID, "some error happened");
			return;
		};
		
		//error checking (i know there is one above but for some reason without this code won't run)
		if err != nil {
			log.Fatal(err);
			return;
		}

		//sednig video (.mp4 after the name can be changed to some other more compressed format)
		if uploaded, err := s.ChannelFileSendWithMessage(m.ChannelID, "here is your video", "test.mp4", &out); err != nil {
			s.ChannelMessageSend(m.ChannelID, "file is too large");
			log.Fatal(err);
			fmt.Println(uploaded);
			return;
		}
	}
}

package main

import (
	"fmt"
	"os"
	"os/signal"
	"syscall"
	"strings"
	"os/exec"

	"github.com/bwmarrin/discordgo"
	"github.com/joho/godotenv"
)

//declare map to store messages id and links they send
var linking = map[string]string{};

func main() {
	cmd := exec.Command("ls", "./");
	if err := cmd.Run(); err != nil {
		fmt.Println("hi");
	}

	out, err := cmd.Output();

	fmt.Println(string(out));
	fmt.Println(err);

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
	//check if reel is send
	if strings.HasPrefix(m.Content, "https://www.instagram.com/reel/") && m.Author.ID != s.State.User.ID {
		//do some cool operations
		_, ok := linking[m.ID];
		if ok == true {
			fmt.Println("value was in the map already");
			return;
		}
		linking[m.ID] = m.Content;
		s.ChannelMessageDelete(m.ChannelID, m.ID);
	}
}

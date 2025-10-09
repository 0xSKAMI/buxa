package main

import (
	"fmt"
	"os"
	"os/signal"
	"syscall"

	"github.com/bwmarrin/discordgo"
	"github.com/joho/godotenv"
)

func main() {
	//importing .env variables
	godotenv.Load("../.env");
	var token string = os.Getenv("TOKEN");
	fmt.Println(token);

	discord, err := discordgo.New("Bot " + token);
	if err != nil {
		fmt.Println("error creating Discord session,", err)
		return
	}

	// Open a websocket connection to Discord and begin listening.
	err = discord.Open();

	if err != nil {
		fmt.Println("error opening connection,", err)
		return
	}
	
	fmt.Println("bot is now listening");

	quitChannel := make(chan os.Signal, 1)
	signal.Notify(quitChannel, syscall.SIGINT, syscall.SIGTERM)
	<-quitChannel

	err = discord.Close();
}

type testHandler interface {
	Type() string
}

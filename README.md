<div align="center">
  <h1>BUXA</h1>
</div>

Buxa is a Discord bot in development. It is always watching you and recording how much time someone has played a game, which you can see with just commands. It was created for my personal friend group server because I couldn't find any bot that would do the same, but anyone can use it in their servers as well.

Bot uses **Discord.Net NuGet**.

<div align="center">
  <h2>STARTING THE BOT</h2>
</div>

To get started, make sure to have **.NET 8** installed.

### Setup Instructions:

1. Clone the repository.
2. Create a `.env` file in the root directory.
3. In this `.env` file, you should define the following variables:
    - `TOKEN` (Discord bot token)
    - `DB` (PostgreSQL connection string)

**Note**: The bot uses PostgreSQL as the database.

4. Run the following command in the bot's folder:

    ```bash
    dotnet run
    ```

That’s it! Your bot should now be up and running.

---

<div align="center">
  <h2>DATABASE SETUP</h2>
</div>

Before running the bot, you need to create the necessary tables in your PostgreSQL database. Use the following SQL script:

```sql
CREATE TABLE players (
    playerid CHAR(255) PRIMARY KEY
);

CREATE TABLE games (
    gameid SERIAL PRIMARY KEY,
    name CHAR(255) NOT NULL
);

CREATE TABLE playergames (
    gameid INTEGER REFERENCES games(gameid),
    playerid CHAR(255) REFERENCES players(playerid),
    played_time INTERVAL,
    creation_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

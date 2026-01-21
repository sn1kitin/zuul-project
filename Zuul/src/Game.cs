using System;

class Game
{
	// Private fields
	private Parser parser;
	private Player player;
	private Room winRoom;

	// Constructor
	public Game()
	{
		parser = new Parser();
		player = new Player();
		CreateRooms();
	}

	// Initialise the Rooms (and the Items)
	private void CreateRooms()
	{
		// Create the rooms
		Room outside = new Room("outside the main entrance of the university");
		Room theatre = new Room("in a lecture theatre");
		Room pub = new Room("in the campus pub");
		Room lab = new Room("in a computing lab");
		Room office = new Room("in the computing admin office");
		winRoom = office;

		// Initialise room exits
		outside.AddExit("east", theatre);
		outside.AddExit("south", lab);
		outside.AddExit("west", pub);

		theatre.AddExit("west", outside);

		pub.AddExit("east", outside);

		lab.AddExit("north", outside);
		lab.AddExit("east", office);

		office.AddExit("west", lab);

		//up down exits
		lab.AddExit("down", office);
		office.AddExit("up", lab);

		// Create your Items here
		Item stone = new Item(1, "A small stone");
		Item key = new Item(1, "A rusty key");
		Item bandage = new Item(1, "Bandage");
		Item table = new Item (1111, "table");

		// And add them to the Rooms
		outside.Chest.Put("stone", stone);
		office.Chest.Put("key", key);
		lab.Chest.Put("bandage", bandage);
		lab.Chest.Put("table", table);

		// Start game outside
		player.CurrentRoom = outside;
	}

	//  Main play routine. Loops until end of play.
	public void Play()
	{
		PrintWelcome();

		// Enter the main command loop. Here we repeatedly read commands and
		// execute them until the player wants to quit.
		bool finished = false;
		while (!finished)
		{
			if (!player.IsAlive())
			{
				Console.WriteLine ("You are dead");
				return;
			}
			if (player.CurrentRoom == winRoom)
			{
				Console.WriteLine("You won!");
				return;
			}
			Command command = parser.GetCommand();
			finished = ProcessCommand(command);
		}
		Console.WriteLine("Thank you for playing.");
		Console.WriteLine("Press [Enter] to continue.");
		Console.ReadLine();
	}

	// Print out the opening message for the player.
	private void PrintWelcome()
	{
		Console.WriteLine();
		Console.WriteLine("Welcome to Zuul!");
		Console.WriteLine("Zuul is a new, incredibly boring adventure game.");
		Console.WriteLine("Type 'help' if you need help.");
		Console.WriteLine();
		Console.WriteLine(player.CurrentRoom.GetLongDescription());
	}

	// Given a command, process (that is: execute) the command.
	// If this command ends the game, it returns true.
	// Otherwise false is returned.
	private bool ProcessCommand(Command command)
	{
		bool wantToQuit = false;

		if(command.IsUnknown())
		{
			Console.WriteLine("I don't know what you mean...");
			return wantToQuit; // false
		}

		switch (command.CommandWord)
		{
			case "help":
				PrintHelp();
				break;
			case "go":
				GoRoom(command);
				break;
			case "look":
				Look();
				break;
			case "take":
				Take(command);
				break;
			case "drop":
				Drop(command);
				break;
			case "status":
				Status();
				break;
			case "quit":
				wantToQuit = true;
				break;
		}

		return wantToQuit;
	}

	// ######################################
	// implementations of user commands:
	// ######################################
	
	// Print out some help information.
	// Here we print the mission and a list of the command words.
	private void PrintHelp()
	{
		Console.WriteLine("You are lost. You are alone.");
		Console.WriteLine("You wander around at the university.");
		Console.WriteLine();
		// let the parser print the commands
		parser.PrintValidCommands();
	}

	private void Look()
	{
		Console.WriteLine(player.CurrentRoom.GetLongDescription());	
	}


	private void Take(Command command)
	{
		if (command.SecondWord == null)
		{
			Console.WriteLine("Take what?");
			return;
		}

		player.TakeFromChest(command.SecondWord);
	}

	private void Drop(Command command)
	{
		if (command.SecondWord == null)
		{
			Console.WriteLine("Drop what?");
			return;
		}

		player.DropToChest(command.SecondWord);
	}

	private void Status()
{
    Console.WriteLine("Health: " + player.health);
	Console.WriteLine("Items: " + player.ShowBackpack() );
}


	// Try to go to one direction. If there is an exit, enter the new
	// room, otherwise print an error message.
	private void GoRoom(Command command)
	{
		if(!command.HasSecondWord())
		{
			// if there is no second word, we don't know where to go...
			Console.WriteLine("Go where?");
			return;
		}

		string direction = command.SecondWord;

		// Try to go to the next room.
		Room nextRoom = player.CurrentRoom.GetExit(direction);
		if (nextRoom == null)
		{
			Console.WriteLine("There is no door to "+direction+"!");
			return;
		}

		player.CurrentRoom = nextRoom;
		player.Damage(10);
		Console.WriteLine("You lost 10 health. Current health: " + player.health);
		Console.WriteLine(player.CurrentRoom.GetLongDescription());
	}
}

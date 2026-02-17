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
		Room entrance = new Room("in the grand entrance hall of Blackmoor Manor.\nCrumbling portraits line the walls. Cold drafts seep through broken stained glass.\nTwo torches still burn weakly in iron sconces by the door.");
		Room corridor = new Room("in a long stone corridor between the manor wings.\nMoth-eaten tapestries hang from the walls, showing the Blackmoor family crest.");
		Room cellar = new Room("at the top of the cellar stairs. The air turns cold and damp below.\nAn oppressive darkness swallows everything past the first step.");
		Room kitchen = new Room("in the manor's old kitchen. A massive stone hearth dominates one wall.\nCopper pots hang from ceiling hooks. The smell of rot lingers in the cold air.");
		Room armory = new Room("in the manor armory. Most weapon racks are stripped bare,\nbut a few pieces remain. A coat of arms hangs above the door.");
		Room crypt = new Room("in the Blackmoor family crypt. Stone sarcophagi line the walls.\nAt the center lies the tomb of Lord Aldric Blackmoor himself.\nA golden reliquary rests atop the stone lid. Something stirs in the shadows...");
		winRoom = crypt;

		// Initialise room exits
		entrance.AddExit("corridor", corridor);
		entrance.AddExit("cellar", cellar);

		corridor.AddExit("entrance", entrance);
		corridor.AddExit("kitchen", kitchen);
		corridor.AddExit("armory", armory);

		cellar.AddExit("entrance", entrance);
		cellar.AddExit("crypt", crypt, "key"); // locked — needs crypt_key

		kitchen.AddExit("corridor", corridor);

		armory.AddExit("corridor", corridor);

		crypt.AddExit("cellar", cellar);

		// Cellar and crypt are dark without a torch
		cellar.IsDark = true;
		crypt.IsDark = true;

		// Create Items here
		Item torch = new Item(1, "A half-burnt torch. It will light your way through the darkness.");
		Item cryptKey = new Item(1, "An old iron key with a skull engraved on it. Opens something below.");
		Item herbBundle = new Item(2, "Dried herbs. Can be used to treat wounds.", 0, 50);
		Item oldBrandy = new Item(1, "A sealed bottle of old brandy. Numbs the pain.", 0, 40);
		Item rustySword = new Item(4, "A rusted longsword. Heavy but still sharp.", 25, 0);
		Item silverDagger = new Item(2, "A silver-hilted dagger. Light and fast.", 35, 0);
		Item relic = new Item(1, "The Blackmoor Reliquary. A golden case said to hold a fragment of the founder's soul.");

		// Add them to the Rooms
		entrance.Chest.Put("torch", torch);
		cellar.Chest.Put("key", cryptKey); // key is in the cellar
		kitchen.Chest.Put("herb_bundle", herbBundle);
		kitchen.Chest.Put("old_brandy", oldBrandy);
		armory.Chest.Put("rusty_sword", rustySword);
		armory.Chest.Put("silver_dagger", silverDagger);
		crypt.Chest.Put("relic", relic);

		// Create enemies
		Enemy rat = new Enemy("Giant Cellar Rat", 40, 15);
		Enemy specter = new Enemy("Specter of Lord Blackmoor", 120, 35);

		// Add enemies to rooms
		cellar.Enemy = rat;   // rat blocks the cellar
		crypt.Enemy = specter;

		// Start game at entrance
		player.CurrentRoom = entrance;
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
				Console.WriteLine ("You have perished within the walls of Blackmoor Manor.");
				return;
			}
			if (player.CurrentRoom == winRoom && !player.CurrentRoom.HasLivingEnemy() && player.HasItem("relic"))
			{
				Console.WriteLine("You clutch the Blackmoor Reliquary and run.");
				Console.WriteLine("The specter's wail fades as you burst into the cold night air.");
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
		Console.WriteLine("Welcome to Blackmoor Manor!");
		Console.WriteLine("You are a relic hunter. The Blackmoor Reliquary is buried in the crypt below.");
		Console.WriteLine("Find it — but Lord Blackmoor does not rest easy.");
		Console.WriteLine("Type 'help' if you need help.");
		Console.WriteLine();
		Console.WriteLine(player.CurrentRoom.GetEnterDescription(player.HasItem("torch")));
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
			case "use":
    			Use(command);
    			break;
			case "drop":
				Drop(command);
				break;
			case "inspect":
				Inspect(command);
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
		Console.WriteLine("You are searching for the Blackmoor Reliquary in the crypt below the manor.");
		Console.WriteLine("Take the torch — the cellar and crypt are pitch dark without it.");
		Console.WriteLine();
		// let the parser print the commands
		parser.PrintValidCommands();
	}

	private void Look()
	{
		Console.WriteLine(player.CurrentRoom.GetLongDescription(player.HasItem("torch")));	
	}

	private void Inspect(Command command)
	{
		if (command.SecondWord == null)
		{
			Console.WriteLine("Inspect what?");
			return;
		}

		// Can't inspect in the dark
		if (player.CurrentRoom.IsDark && !player.HasItem("torch"))
		{
			Console.WriteLine("It is pitch dark. You can't see anything.");
			return;
		}

		string itemName = command.SecondWord;

		// Check room first, then backpack
		var roomItems = player.CurrentRoom.Chest.GetAllItems();
		if (roomItems.ContainsKey(itemName))
		{
			Console.WriteLine(player.CurrentRoom.InspectItem(itemName));
		}
		else
		{
			Console.WriteLine(player.InspectFromBackpack(itemName));
		}
	}

	private void Take(Command command)
	{
		if (command.SecondWord == null)
		{
			Console.WriteLine("Take what?");
			return;
		}

		// Can't take items in the dark
		if (player.CurrentRoom.IsDark && !player.HasItem("torch"))
		{
			Console.WriteLine("It is pitch dark. You can't see anything.");
			return;
		}

		// Can't take items while enemy is alive
		if (player.CurrentRoom.HasLivingEnemy())
		{
			Console.WriteLine("You cannot pick up items while there is a dangerous enemy here!");
			return;
		}

		player.TakeFromChest(command.SecondWord);
	}

	private void Use(Command command)
	{
		if (!command.HasSecondWord())
		{
			Console.WriteLine("Use what?");
			return;
		}

		string itemName = (command.SecondWord);
		string target = command.ThirdWord; // can be null

		// Handle using a key on a locked exit
		if (itemName.Contains("key") && target != null)
		{
			if (player.CurrentRoom.IsExitLocked(target))
			{
				if (player.HasItem(itemName) && player.CurrentRoom.UnlockExit(target, itemName))
				{
					Console.WriteLine("You use the " + itemName.Replace("_", " ") + ". The door to the " + target + " unlocks with a heavy clunk.");
					return;
				}
				Console.WriteLine("That key doesn't fit this door.");
				return;
			}
			Console.WriteLine("That door is not locked.");
			return;
		}

		string result = player.Use(itemName, target);
		Console.WriteLine(result);
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

		// Check if there's a living enemy blocking the way
		if (player.CurrentRoom.HasLivingEnemy())
		{
			Room nextRoom = player.CurrentRoom.GetExit(direction);
			if (nextRoom == null)
			{
				Console.WriteLine("There is no door to "+direction+"!");
				return;
			}

			// Can only retreat to previous room
			if (nextRoom != player.PreviousRoom)
			{
				Console.WriteLine("You cannot leave! Defeat the enemy or retreat the way you came.");
				return;
			}
			Console.WriteLine("You carefully retreat from the enemy...");
		}
		else
		{
			// No enemy, check normal exit
			Room nextRoom = player.CurrentRoom.GetExit(direction);
			if (nextRoom == null)
			{
				Console.WriteLine("There is no door to "+direction+"!");
				return;
			}

			// Check if exit is locked
			if (player.CurrentRoom.IsExitLocked(direction))
			{
				string requiredKey = player.CurrentRoom.GetRequiredKey(direction);
				Console.WriteLine("The door to the " + direction + " is locked.");
				Console.WriteLine("You need the " + requiredKey.Replace("_", " ") + " to open it.");
				return;
			}
		}

		// Try to go to the next room.
		Room targetRoom = player.CurrentRoom.GetExit(direction);
		player.PreviousRoom = player.CurrentRoom;
		player.CurrentRoom = targetRoom;
		
		player.Damage(5);
		Console.WriteLine("You lost 5 health. Current health: " + player.health);
		Console.WriteLine(player.CurrentRoom.GetEnterDescription(player.HasItem("torch")));
	}
}

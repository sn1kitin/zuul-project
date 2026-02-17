using System.Collections.Generic;

class Room
{
	// Private fields
	private string description;
	private Dictionary<string, Room> exits; // stores exits of this room.
	private Dictionary<string, string> lockedExits; // exit -> required key name, null if unlocked
	private Inventory chest;
	private Enemy enemy;
	public bool IsDark { get; set; }

	// Property (getter only)
	public Inventory Chest 
	{
		get { return chest; }
	}

	public Enemy Enemy
	{
		get { return enemy; }
		set { enemy = value; }
	}

	// Create a room described "description". Initially, it has no exits.
	// "description" is something like "in a kitchen" or "in a court yard".
	public Room(string desc)
	{
		description = desc;
		exits = new Dictionary<string, Room>();
		lockedExits = new Dictionary<string, string>();
		// een room kan veel items bevatten
		chest = new Inventory(9999999);
		enemy = null;
		IsDark = false;
	}

	// Define an exit for this room. Optionally locked with a required key.
	public void AddExit(string direction, Room neighbor, string requiredKey = null)
	{
		exits.Add(direction, neighbor);
		lockedExits.Add(direction, requiredKey); // null means unlocked
	}

	// Check if room has a living enemy
	public bool HasLivingEnemy()
	{
		return enemy != null && enemy.IsAlive;
	}

	// Check if an exit is locked
	public bool IsExitLocked(string direction)
	{
		if (lockedExits.ContainsKey(direction))
			return lockedExits[direction] != null;
		return false;
	}

	// Try to unlock an exit with a key. Returns true if successful.
	public bool UnlockExit(string direction, string keyName)
	{
		if (!lockedExits.ContainsKey(direction))
			return false;
		if (lockedExits[direction] == keyName)
		{
			lockedExits[direction] = null; // unlocked
			return true;
		}
		return false;
	}

	// Get the required key name for a locked exit
	public string GetRequiredKey(string direction)
	{
		if (lockedExits.ContainsKey(direction))
			return lockedExits[direction];
		return null;
	}

	// Return the description of the room.
	public string GetShortDescription()
	{
		return description;
	}

	// Return a long description of this room, in the form:
	//     You are in the kitchen.
	//     Exits: north, west
	// Shows full item descriptions. Used by "look" command.
public string GetLongDescription(bool hasTorch)
	{
		if (IsDark && !hasTorch)
			return "It is pitch dark. You can't see anything.";

		string str = "You are ";
		str += description;
		str += ".\n";
		str += GetExitString();

		// Show items with full descriptions
		string items = GetItemsStringFull();
		if (items != "")
		{
			str += "\n" + items;
		}

		// Show enemy
		if (enemy != null && enemy.IsAlive)
		{
			str += "\nThere is a " + enemy.GetDescription() + " here!";
		}
		else if (enemy != null && !enemy.IsAlive)
		{
			str += "\nThere is a dead " + enemy.Name + " here.";
		}

		return str;
	}

	// Return a short description shown when entering a room.
	// Shows item names only, without descriptions.
	public string GetEnterDescription(bool hasTorch)
	{
		if (IsDark && !hasTorch)
			return "It is pitch dark. You can't see anything.";

		string str = "You are ";
		str += description;
		str += ".\n";
		str += GetExitString();

		// Show items names only (no descriptions)
		string items = GetItemsStringShort();
		if (items != "")
		{
			str += "\n" + items;
		}

		// Show enemy
		if (enemy != null && enemy.IsAlive)
		{
			str += "\nThere is a " + enemy.GetDescription() + " here!";
		}
		else if (enemy != null && !enemy.IsAlive)
		{
			str += "\nThere is a dead " + enemy.Name + " here.";
		}

		return str;
	}

	// Return a description of a single item in the room with damage/heal info.
	// Used by "inspect" command.
	public string InspectItem(string itemName)
	{
		var allItems = chest.GetAllItems();
		if (!allItems.ContainsKey(itemName))
			return "There is no " + itemName + " here.";

		Item item = allItems[itemName];
		string str = itemName + ": " + item.Description;
		if (item.Damage > 0)
			str += "\n  Damage: " + item.Damage;
		if (item.HealAmount > 0)
			str += "\n  Heals: " + item.HealAmount + " HP";
		return str;
	}

	// Return the room that is reached if we go from this room in direction
	// "direction". If there is no room in that direction, return null.
	public Room GetExit(string direction)
	{
		if (exits.ContainsKey(direction))
		{
			return exits[direction];
		}
		return null;
	}

	// Return a string describing the room's exits, for example
	// "Exits: north, west".
	private string GetExitString()
	{
		string str = "Exits: ";
		str += String.Join(", ", exits.Keys);

		return str;
	}

	// Return items with names only (no descriptions)
	private string GetItemsStringShort()
	{
		var allItems = chest.GetAllItems();
		if (allItems.Count == 0)
			return "";

		string str = "Items here: ";
		str += String.Join(", ", allItems.Keys);
		return str;
	}

	// Return items with full descriptions
		private string GetItemsStringFull()
	{
		var allItems = chest.GetAllItems();
		if (allItems.Count == 0)
			return "";

		string str = "Items here:\n";
		foreach (var item in allItems)
		{
			str += "  " + item.Key + ": " + item.Value.Description;
			if (item.Value.Damage > 0)
				str += " [Damage: " + item.Value.Damage + "]";
			if (item.Value.HealAmount > 0)
				str += " [Heals: " + item.Value.HealAmount + " HP]";
			str += "\n";
		}
		return str.TrimEnd();
	}
}

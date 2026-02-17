class Player
{
    //auto property
    public Room CurrentRoom { get; set; }
    public Room PreviousRoom { get; set; }
    //fields
    public int health;
    private Inventory backpack;
    //constructor
    public Player()
    {
        CurrentRoom = null;
        PreviousRoom = null;
        health = 100;
        backpack = new Inventory(25);
    }
    //methods
    public void Damage(int amount)
    {
        health -= amount;
        if (health < 0)
        health = 0;
    }

    public void Heal(int amount)
    {
        health += amount;
        if (health > 100)
        health = 100;
    }

    public bool IsAlive()
    {
        return health > 0;
    }

    public bool TakeFromChest(string itemName)
    {
        Item item = CurrentRoom.Chest.Get(itemName);

        if (item == null)
        {
            Console.WriteLine ("Item is not in the room.");
            return false;
        }

        if (!backpack.Put(itemName, item))
        {
            Console.WriteLine("Item doesnt fit in your inventory.");
            CurrentRoom.Chest.Put(itemName, item);
            return false;
        }

        Console.WriteLine("You picked up " + itemName);
        return true;
    }

    public bool DropToChest(string itemName)
    {
        Item item = backpack.Get(itemName);

        if (item == null)
        {
            Console.WriteLine("You dont have that item.");
            return false;
        }

        CurrentRoom.Chest.Put(itemName, item);
        Console.WriteLine("You dropped " + itemName);
        return true;
    }

    public string ShowBackpack()        
    {
        return backpack.Show();
    }

    public bool HasItem(string itemName)
    {
        return backpack.HasItem(itemName);
    }

    // Inspect an item from the backpack
    public string InspectFromBackpack(string itemName)
    {
        var allItems = backpack.GetAllItems();
        if (!allItems.ContainsKey(itemName))
            return "You dont have that item.";

        Item item = allItems[itemName];
        string str = itemName + " (in backpack): " + item.Description;
        if (item.Damage > 0)
            str += "\n  Damage: " + item.Damage;
        if (item.HealAmount > 0)
            str += "\n  Heals: " + item.HealAmount + " HP";
        return str;
    }

    public string Use(string itemName, string target)
    {
        if (!backpack.HasItem(itemName))
        {
            return "You dont have that item"; 
        }

        var allItems = backpack.GetAllItems();
        Item item = allItems[itemName];

        // Healing items
        if (item.HealAmount > 0)
        {
            backpack.Get(itemName); // consume
            Heal(item.HealAmount);
            return "You used the " + itemName.Replace("_", " ") + ". Health: " + health;
        }

        // Weapons
        if (item.Damage > 0)
        {
            if (target == null)
                return "Use " + itemName.Replace("_", " ") + " on what? Try: use " + itemName + " <enemy>";

            Enemy enemy = CurrentRoom.Enemy;
            if (enemy == null || !enemy.IsAlive)
                return "There is nothing to attack here.";

            if (!enemy.Name.ToLower().Contains(target.ToLower()))
                return "There is no " + target + " here.";

            enemy.TakeDamage(item.Damage);
            string result = "You hit the " + enemy.Name + " with your " + itemName.Replace("_", " ") + " for " + item.Damage + " damage!";

            if (enemy.IsAlive)
            {
                result += "\n" + enemy.Name + " has " + enemy.Health + " health remaining.";
                Damage(enemy.Damage);
                result += "\nThe " + enemy.Name + " hits you for " + enemy.Damage + " damage! Your health: " + health;
            }
            else
            {
                result += "\nYou defeated the " + enemy.Name + "!";
            }

            return result;
        }

        // Torch
        if (itemName == "torch")
            return "The torch flickers in your hand.";

        return "You cant use that item";
    }
   
}

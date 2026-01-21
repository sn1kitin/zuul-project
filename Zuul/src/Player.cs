class Player
{
    //auto property
    public Room CurrentRoom { get; set; }
    //fields
    public int health;
    private Inventory backpack;
    //constructor
    public Player()
    {
        CurrentRoom = null;
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

}


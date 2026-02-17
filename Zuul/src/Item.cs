class Item
{
    //fields
    public int Weight {get;}
    public string Description {get;}
    public int Damage {get;}
    public int HealAmount {get;}

    //consrtuctor
    public Item(int weight, string description, int damage = 0, int healAmount = 0)
    {
        Weight = weight;
        Description = description;
        Damage = damage;
        HealAmount = healAmount;
    }
}

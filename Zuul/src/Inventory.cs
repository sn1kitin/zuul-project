using System.Collections.Generic;

class Inventory
{
    // fields
    private int maxWeight;
    private Dictionary<string, Item> items;

    // constructor
    public Inventory(int maxWeight)
    {
        this.maxWeight = maxWeight;
        this.items = new Dictionary<string, Item>();
    }

    // methods
    public bool Put(string itemName, Item item)
    {
        // Check weight
        if (item.Weight > FreeWeight())
            return false;

        items[itemName] = item;
        return true;
    }

    public Item Get(string itemName)
    {
        if (items.ContainsKey(itemName))
        {
            Item item = items[itemName];
            items.Remove(itemName);
            return item;
        }
        return null;
    }

    public int TotalWeight()
    {
        int total = 0;
        foreach (var pair in items)
            total += pair.Value.Weight;

        return total;
    }

    public int FreeWeight()
    {
        return maxWeight - TotalWeight();
    }

    public string Show()
    {
        if (items.Count == 0)
            return "(empty)";

        string result = "";
        foreach (var pair in items)
            result += $"{pair.Key} ({pair.Value.Weight}kg)\n";

        return result;
    }
}

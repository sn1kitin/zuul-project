class Enemy
{
	// Private fields
	private string name;
	private int health;
	private int damage;

	// Properties
	public string Name { get { return name; } }
	public int Health { get { return health; } }
	public int Damage { get { return damage; } }
	public bool IsAlive { get { return health > 0; } }

	// Constructor
	public Enemy(string name, int health, int damage)
	{
		this.name = name;
		this.health = health;
		this.damage = damage;
	}

	// methods
	public void TakeDamage(int amount)
	{
		health -= amount;
		if (health < 0)
			health = 0;
	}

	public string GetDescription()
	{
		return name + " (Health: " + health + ")";
	}
}

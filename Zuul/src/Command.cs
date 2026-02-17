class Command
{
	public string CommandWord { get; init; }
	public string SecondWord { get; init; }
	public string ThirdWord { get; init; }
	
	// Create a command object. First, second and third word must be supplied, but
	// any of them can be null. See Parser.GetCommand()
	public Command(string first, string second, string third)
	{
		CommandWord = first;
		SecondWord = second;
		ThirdWord = third;
	}

	
	// Return true if this command was not understood.
	public bool IsUnknown()
	{
		return CommandWord == null;
	}

	
	// Return true if the command has a second word.
	public bool HasSecondWord()
	{
		return SecondWord != null;
	}
}

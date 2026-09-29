public struct AtomContainer
{
    private Dictionary<string, Atom> atomDictionary;

    public AtomContainer()
    {
        atomDictionary = new Dictionary<string, Atom>();
    }

    public void addBlock(string name, Atom atom)
    {
        atomDictionary.Add(name, atom);
    }

    public void removeBlock(string name)
    {
        atomDictionary.Remove(name);
    }

    public Atom getBlockByName(string name)
    {
        return atomDictionary[name];
    }

    public Dictionary<string, Atom> getContainer()
    {
        return atomDictionary;
    }
}

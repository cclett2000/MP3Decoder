public struct AtomContainer
{
    private Dictionary<string, Atom> atomDictionary;

    public AtomContainer()
    {
        atomDictionary = new Dictionary<string, Atom>();
    }

    public void addAtom(string name, Atom atom)
    {
        atomDictionary.Add(name, atom);
    }

    public void removeAtom(string name)
    {
        atomDictionary.Remove(name);
    }

    public Atom getAtomByName(string name)
    {
        return atomDictionary[name];
    }

    public Dictionary<string, Atom> getContainer()
    {
        return atomDictionary;
    }

    public bool doesExist(string atomName)
    {
        return atomDictionary.ContainsKey(atomName);
    }
}

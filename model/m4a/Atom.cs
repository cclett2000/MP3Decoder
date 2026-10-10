public struct Atom
{
    private static readonly HashSet<string> CONTAINER_ATOMS = new HashSet<string> {
        "moov", "trak", "mdia", "minf", "dinf", "stbl", "avc1", "edts"
    };

    private static readonly HashSet<string> IGNORED_ATOMS = new HashSet<string>
    {
        "free"
    };

    private bool _isContainer;
    public bool IsContainer {
        get { return _isContainer; }
        set 
        {
            if (value == true)
            {
                Container = new AtomContainer();
            }

            _isContainer = value;
        }
        
    }

    public int Size { get; set; }
    public string Header { get; set; }
    public byte[] Payload { get; set; }
    public AtomContainer Container { get; set; }

    public static bool isContainerAtom(string atomType)
    {
        return CONTAINER_ATOMS.Contains(atomType);
    }

    public static bool isAtomIgnored(string atomType)
    {
        return IGNORED_ATOMS.Contains(atomType);
    }
}
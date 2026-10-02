public struct Atom
{
    private bool _isContainer;
    public bool IsContainer {
        get { return _isContainer; }
        set 
        {
            if (value == true)
            {
                Children = new AtomContainer();
            }

            _isContainer = value;
        }
        
    }

    public int Size { get; set; }
    public string Header { get; set; }
    public byte[] Payload { get; set; }
    public AtomContainer Children { get; set; }
}
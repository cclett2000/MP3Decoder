public struct Atom
{
    // flag(s)
    internal bool isContainer {  get; set; }

    // var(s)
    internal int size { get; set; }
    internal string header { get; set; }
    internal byte[] payload { get; set; }
    internal AtomContainer children { get; set; }
}
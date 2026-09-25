public struct Block
{
    // TODO: may not need constructor, remove if true
    public Block () {

    }

    /// <summary>
    /// First 4 bytes of the block; indicates the size (in bytes) of the block
    /// as a whole.
    /// </summary>
    public byte[] size;

    /// <summary>
    /// Next 4 bytes that indicate what kind of data this block contains 
    /// </summary>
    public byte[] header;

    /// <summary>
    /// Next 'n' number of bytes, this is the actual data of the block
    /// </summary>
    public byte[] payload;
}
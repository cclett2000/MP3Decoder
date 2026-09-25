using MP3Decoder.decode.m4a.file;
using MP3Decoder.playback;

class Program
{
    public static void Main(string[] args)
    {
        BlockContainer blockContainer = new BlockContainer();

        AudioPlayer audioPlayer = new AudioPlayer();
        M4AFileHandler m4AFileHandler = new M4AFileHandler("C:\\Users\\Charles\\source\\repos\\MP3Decoder\\data\\01. Angel City - Love Me Right (Oh Sheila).m4a");

        m4AFileHandler.setBlockContainer(blockContainer);
        m4AFileHandler.LoadInMemory();
    }
}
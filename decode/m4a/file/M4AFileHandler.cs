using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace MP3Decoder.decode.m4a.file
{
    internal class M4AFileHandler(string m4aFilePath)
    {
        static int BLOCK_SIZE_LENGTH = 4;
        static int BLOCK_HEAD_LENGTH = 4;

        private byte[] data = [];
        private int bytePosition = 0;
        private string m4aFilePath = m4aFilePath;

        BlockContainer blockContainer;

        public void LoadInMemory()
        {
            // load file into mem using stream
            data = File.ReadAllBytes(m4aFilePath);

            ValidateFileType();
            ParseBlocks();

            // GC, do your thing
            data = [];
        }

        private void ParseBlocks()
        {
            while (bytePosition < data.Length)
            {
                Block tempBlock = new Block();
                int[] blockRange = CalculateBlockSize();
                //TODO: store block range using...well blockRange
                

            }
        }

        /// <summary>
        /// Calculates the size of the next block in the M4A byte array.
        /// </summary>
        /// <returns>the range, start and end position, of the next block</returns>
        private int[] CalculateBlockSize()
        {
            int size = 0;
            foreach (uint byteItem in data[bytePosition..BLOCK_SIZE_LENGTH])
            {
                size += (int)byteItem;
            }

            // ensure our position is updated properly
            return [(bytePosition + 4), size];
        }

        /// <param name="data"></param>
        /// <returns></returns>
        private int ValidateFileType()
        {            
            if (Encoding.UTF8.GetString(data[BLOCK_SIZE_LENGTH..(BLOCK_SIZE_LENGTH + (BLOCK_HEAD_LENGTH * 2))]) != "ftypM4A ")
            {
                // do something else
                Environment.Exit(500);
            }

            return bytePosition = CalculateBlockSize()[1];
        }

        public void setBlockContainer(BlockContainer blockContainer)
        {
            this.blockContainer = blockContainer;
        }
    }
}

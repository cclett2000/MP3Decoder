using System;
using System.Collections.Generic;
using System.Text;

namespace MP3Decoder.decode.m4a.file
{
    internal class M4AFileHandler
    {
        static int BLOCK_SIZE_LENGTH = 4;

        private int pos = 0;
        private string m4aFilePath = "";

        public M4AFileHandler(string m4aFilePath)
        {
            this.m4aFilePath = m4aFilePath;
        }

        // TODO: Look into a data structure (stack?) that allows quicker pruning of
        //       already touched bytes
        public void loadInMemory()
        {
            // load file into mem using stream
            byte[] data = File.ReadAllBytes(m4aFilePath);

            // ensure file is .m4a
            validateFileType(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// position in byte stream where the next box begins
        /// </returns>
        private int validateFileType(byte[] data)
        {
            int size = 0;
            while (pos < BLOCK_SIZE_LENGTH)
            {
                size += Convert.ToInt32(data[pos]);
                pos++;
            }
            

            return size;
        }


    }
}

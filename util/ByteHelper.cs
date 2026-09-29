using System.Text;

namespace MP3Decoder.util
{
    internal class ByteHelper
    {
        public static int calculateIntegerFromByteArray(byte[] bytesToCalculate, bool isBigEndian = false)
        {
            if (isBigEndian)
            {
                Array.Reverse(bytesToCalculate);
            }

            return (int)BitConverter.ToUInt32(bytesToCalculate, 0);
        }

        public static string getStringValueFromByteArray(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
    }
}

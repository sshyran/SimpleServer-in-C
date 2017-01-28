using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DMP9Labs.IO
{
    public class DlfEncoder
    {
        public DlfEncoder() { }

        internal static List<char> table = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/".ToList();
        public string EncodeBytes(byte[] bytes)
        {
            char[] result;
            try
            {
                result = new char[bytes.Length * 2];
                int i = 0;
                foreach (byte b in bytes)
                {
                    byte[] buffer = new byte[4];
                    byte temp;
                    buffer[0] = (byte)((b & 252) >> 2);
                    temp = (byte)((b & 3) << 4);
                    buffer[1] = ((0 & 240) >> 4);
                    buffer[1] += temp;
                    temp = ((0 & 15) << 2);
                    buffer[2] = ((0 & 192) >> 6);
                    buffer[2] += temp;
                    buffer[3] = (0 & 63);
                    for (int x = 0; x < 2; x++)
                    {
                        result[i] = table[buffer[x]];
                        i++;
                    }
                }
            }
            catch (Exception e)
            {
                result = new char[0];
                throw new Exception("Failed to encode to DLF format", e);
            }
            return "dlf" + new string(result);
        }
        public byte[] DecodeString(string s)
        {
            if (!s.StartsWith("dlf"))
            {
                throw new Exception("Failed to decode from DLF format",new Exception("Not valid DLF, didn't have 'dlf' at start of string"));
            }
            string s2 = s.Substring(3);
            byte[] result;
            try
            {
                result = new byte[s2.Length / 2];
                int i = 0;
                foreach (char[] c in s2.Select((c, j) => new { letter = c, group = j / 2 }).GroupBy(l => l.group, l => l.letter).Select(g => string.Join("", g)).Select(l => l.ToCharArray()))
                {
                    byte[] buffer = new byte[2];
                    byte[] buffer2 = new byte[3];
                    for (int x = 0; x < 2; x++)
                    {
                        buffer[x] = (byte)table.IndexOf(table.First(y => y == c[x]));
                    }
                    byte b, b1, b2;
                    byte temp1, temp2;
                    for (int x = 0; x < 1; x++)
                    {
                        temp1 = buffer[0];
                        temp2 = buffer[1];
                        b = (byte)(buffer[0] << 2);
                        b1 = (byte)((buffer[1] & 48) >> 4);
                        b1 += b;

                        b = (byte)((buffer[1] & 15) << 4);
                        b2 = ((0 & 60) >> 2);
                        b2 += b;
                        buffer2[0] = b1;
                        buffer2[1] = b2;
                    }
                    result[i] = buffer2[0];
                    i++;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Failed to decode from DLF format", e);
            }
            return result;
        }
    }
}


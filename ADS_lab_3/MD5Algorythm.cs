using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADS_lab_2
{
    public static class MD5Algorythm
    {
        private readonly static uint[] T = new uint[64] {
                0xd76aa478, 0xe8c7b756, 0x242070db, 0xc1bdceee,
                0xf57c0faf, 0x4787c62a, 0xa8304613, 0xfd469501,
                0x698098d8, 0x8b44f7af, 0xffff5bb1, 0x895cd7be,
                0x6b901122, 0xfd987193, 0xa679438e, 0x49b40821,
                0xf61e2562, 0xc040b340, 0x265e5a51, 0xe9b6c7aa,
                0xd62f105d, 0x02441453, 0xd8a1e681, 0xe7d3fbc8,
                0x21e1cde6, 0xc33707d6, 0xf4d50d87, 0x455a14ed,
                0xa9e3e905, 0xfcefa3f8, 0x676f02d9, 0x8d2a4c8a,
                0xfffa3942, 0x8771f681, 0x6d9d6122, 0xfde5380c,
                0xa4beea44, 0x4bdecfa9, 0xf6bb4b60, 0xbebfbc70,
                0x289b7ec6, 0xeaa127fa, 0xd4ef3085, 0x04881d05,
                0xd9d4d039, 0xe6db99e5, 0x1fa27cf8, 0xc4ac5665,
                0xf4292244, 0x432aff97, 0xab9423a7, 0xfc93a039,
                0x655b59c3, 0x8f0ccc92, 0xffeff47d, 0x85845dd1,
                0x6fa87e4f, 0xfe2ce6e0, 0xa3014314, 0x4e0811a1,
                0xf7537e82, 0xbd3af235, 0x2ad7d2bb, 0xeb86d391
        };

        private readonly static int[] shift = new int[64]
        {
            7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,  7, 12, 17, 22,
            5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,  5,  9, 14, 20,
            4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,  4, 11, 16, 23,
            6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21,  6, 10, 15, 21
        };



        public static uint[] Algorithm(byte[] massege, bool isBigFile = false)
        {
            uint A = 0x67452301;
            uint B = 0xEFCDAB89;
            uint C = 0x98BADCFE;
            uint D = 0x10325476;

            if (!isBigFile)
            {
                massege = PaddingText(massege);
            }

            uint counter;
            if (isBigFile)
            {
                counter = (uint) Math.Ceiling((massege.Length * (uint)8 * 1.0) / 512);
            }
            else
            {
                counter = (uint)(massege.Length * (uint)8 / 512);
            }
            for (uint j = 0; j < counter; j++)
            {
                byte[] tmpMsg = new byte[64];
                Array.Copy(massege, 64 * j, tmpMsg, 0, 64);

                uint[] tmpRes = MessageProcessing(A, B, C, D, tmpMsg);

                A = tmpRes[0];
                B = tmpRes[1];
                C = tmpRes[2];
                D = tmpRes[3];
            }
            return new uint[] { A, B, C, D };
        }
        public static uint[] Algorithm(byte[] massege, uint[] previousResult, bool isBigFile = false)
        {
            uint A = previousResult[0];
            uint B = previousResult[1];
            uint C = previousResult[2];
            uint D = previousResult[3];

            if (!isBigFile)
            {
                massege = PaddingText(massege);
            }

            uint counter;
            if (isBigFile)
            {
                counter = (uint)Math.Ceiling((massege.Length * (uint)8 * 1.0) / 512);
            }
            else
            {
                counter = (uint)(massege.Length * (uint)8 / 512);
            }
            for (long j = 0; j < counter; j++)
            {
                byte[] tmpMsg = new byte[64];
                Array.Copy(massege, 64 * j, tmpMsg, 0, 64);

                uint[] tmpRes = MessageProcessing(A, B, C, D, tmpMsg);

                A = tmpRes[0];
                B = tmpRes[1];
                C = tmpRes[2];
                D = tmpRes[3];
            }
            return new uint[] { A, B, C, D };
        }

        //Функція виконує доповнення до 512 бітів
        private static byte[] PaddingText(byte[] arrayMessage)
        {
            int originalLength = arrayMessage.Length;
            int paddingLength = 64 - ((originalLength + 8) % 64);  // 64 біти (8 байтів) для зберігання довжини повідомлення
            if (paddingLength < 1)
                paddingLength += 64;

            int newLength = originalLength + paddingLength + 8;  // Додаємо 8 байтів для зберігання довжини повідомлення

            // Створюємо новий масив з належними розмірами
            byte[] paddedMessage = new byte[newLength];

            // Копіюємо оригінальне повідомлення в новий масив
            Array.Copy(arrayMessage, 0, paddedMessage, 0, originalLength);

            // Додаємо 1 (1000_0000) у кінці оригінального повідомлення
            paddedMessage[originalLength] = 0x80;

            // Додаємо останні 8 байтів (64 біти) - довжину оригінального повідомлення
            long arrayMessageLengthBits = (long)originalLength * 8;
            byte[] lengthBytes = BitConverter.GetBytes(arrayMessageLengthBits);

            Array.Copy(lengthBytes, 0, paddedMessage, newLength - 8, 8);

            return paddedMessage;
        }

        // Обробка повідомлення 
        private static uint[] MessageProcessing(uint A, uint B, uint C, uint D, byte[] block)
        {
            uint a = A, b = B, c = C, d = D, f = 0, p = 0;

            // Необхідно розділити кожен блок (512 бітів) на 16 блоків по 32 біти
            uint[] miniBlock = new uint[16];
            for (int i = 0; i < miniBlock.Length; i++)
            {
                miniBlock[i] = BitConverter.ToUInt32(block, i * 4);
            }

            for (uint i = 0; i < T.Length; i++)
            {
                if (i < 16)
                {
                    f = (b & c) | (~b & d);
                    p = i;
                }
                else if (i >= 16 && i < 32)
                {
                    f = (b & d) | (c & ~d);
                    p = (1 + (5 * i)) % 16;
                }
                else if (i >= 32 && i < 48)
                {
                    f = b ^ c ^ d;
                    p = (5 + (3 * i)) % 16;
                }
                else
                {
                    f = c ^ (b | ~d);
                    p = (7 * i) % 16;
                }

                var tmpD = d;
                d = c;
                c = b;
                b = b + CyclicShiftLeft(a + f + miniBlock[p] + T[i], shift[i]);
                a = tmpD;
            }

            A = A + a;
            B = B + b;
            C = C + c;
            D = D + d;

            return new uint[] { A, B, C, D };
        }

        private static uint CyclicShiftLeft(uint value, int count)
        {
            // Потрібно використовувати також зсув вправо на зворотню кількість бітів
            return (value << count) | (value >> (32 - count));
        }

        public static string MakeStringFromArrUInt(uint[] array)
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < array.Length; i++)
            {
                stringBuilder.Append(MakeStringFromInt(array[i]));
            }

            return stringBuilder.ToString();
        }
        private static string MakeStringFromInt(uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            StringBuilder hexString = new StringBuilder();

            foreach (byte b in bytes)
            {
                hexString.AppendFormat("{0:x2}", b);
            }

            return hexString.ToString();
        }
    }
}

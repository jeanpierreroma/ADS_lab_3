using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADS_lab_3
{
    public class RC5Algorrithm
    {
        // Довжина слова
        private readonly int W;

        // Кількість раундів
        private readonly int R;

        // Довжина ключа
        private readonly int B;

        //64-бітні константи
        const ulong PW_64 = 0xB7E151628AED2A6B;
        const ulong QW_64 = 0x9E3779B97F4A7C15;

        //32-бітні константи
        const uint PW_32 = 0xB7E15163;
        const uint QW_32 = 0x9E3779B9;

        //16-бітні константи
        const ushort PW_16 = 0xB7E1;
        const ushort QW_16 = 0x9E37;

        ulong[] S_long;
        ushort[] S_short;
        
        public RC5Algorrithm(int w, int r, byte[] b)
        {
            W = w;
            R = r;
            B = b.Length;

            if (w != 16)
            {
                S_long = CreateSubkyes(b);
            }
            else
            {
                S_short = CreateSubkyesShort(b);
            }
        }

        private ulong[] CreateSubkyes(byte[] key)
        {
            // Этап 1. Розбивання ключа на слова

            // кількість байтів у слові
            int u = W / 8;

            int keyLength = key.Length;

            // розмір масива слів L
            int c = keyLength / u;
            ulong[] L = new ulong[c];


            for (int i = keyLength - 1; i >= 0; i--)
            {
                L[i / u] = CyclicShiftLeft(L[i / u], 8) + key[i];
            }


            // Етап 2. Ініціалізація масиву S (підключів)

            ulong[] subkeysArray = new ulong[2 * R + 2];
            if (W == 64)
            {
                subkeysArray[0] = PW_64;
            }
            else if (W == 32)
            {
                subkeysArray[0] = PW_32;
            }
            else
            {
                subkeysArray[0] = PW_16;
            }

            for (int i = 1; i < 2 * R + 2; i++)
            {
                if (W == 64)
                {
                    subkeysArray[i] = subkeysArray[i - 1] + QW_64;
                }
                else if (W == 32)
                {
                    subkeysArray[i] = (subkeysArray[i - 1] + QW_32);
                }
                else
                {
                    subkeysArray[i] = (subkeysArray[i - 1] + QW_16);
                }
            }

            // Етап 3. Змішування ініціалізованого масиву S з масивом ключів L

            int ii = 0, jj = 0;
            ulong a = 0, b = 0;
            int t = Math.Max(c, (2 * R) + 2);

            for (int s = 0; s < 3 * t; s++)
            {
                subkeysArray[ii] = CyclicShiftLeft((subkeysArray[ii] + a + b), 3);
                a = subkeysArray[ii];
                L[jj] = CyclicShiftLeft((L[jj] + a + b), (int)(a + b));
                b = L[jj];
                ii = (ii + 1) % (2 * R + 2);
                jj = (jj + 1) % c;
            }

            return subkeysArray;
        }

        private ushort[] CreateSubkyesShort(byte[] key)
        {
            // Этап 1. Розбивання ключа на слова

            // кількість байтів у слові
            int u = W / 8;

            int keyLength = key.Length;

            // розмір масива слів L
            int c = keyLength / u;
            ushort[] L = new ushort[c];


            for (int i = keyLength - 1; i >= 0; i--)
            {
                L[i / u] = (ushort)(CyclicShiftLeft(L[i / u], 8) + key[i]);
            }


            // Етап 2. Ініціалізація масиву S (підключів)

            ushort[] subkeysArray = new ushort[2 * R + 2];
            subkeysArray[0] = PW_16;


            for (int i = 1; i < 2 * R + 2; i++)
            {
                subkeysArray[i] = (ushort)(subkeysArray[i - 1] + QW_16);
            }

            // Етап 3. Змішування ініціалізованого масиву S з масивом ключів L

            int ii = 0, jj = 0;
            ushort a = 0, b = 0;
            int t = Math.Max(c, (2 * R) + 2);

            for (int s = 0; s < 3 * t; s++)
            {
                subkeysArray[ii] = CyclicShiftLeft((ushort)(subkeysArray[ii] + a + b), 3);
                a = subkeysArray[ii];
                L[jj] = CyclicShiftLeft((ushort)(L[jj] + a + b), (a + b));
                b = L[jj];
                ii = (ii + 1) % (2 * R + 2);
                jj = (jj + 1) % c;
            }

            return subkeysArray;
        }

        //Циклічний зсув вліво
        private ulong CyclicShiftLeft(ulong a, int offset)
        {
            return (a << offset | a >> (W - offset));
        }
        private uint CyclicShiftLeft(uint a, int offset)
        {
            return ((uint)(a << offset | a >> (W - offset)));
        }
        private ushort CyclicShiftLeft(ushort x, int y) => (ushort)((x << (y % W)) | (x >> (W - (y % W))));
        private ushort CyclicShiftRight(ushort x, int y) => (ushort)((x >> (y % W)) | (x << (W - (y % W))));
        //private ushort CyclicShiftLeft(ushort a, int offset)
        //{
        //    return ((ushort)(a << offset | a >> (W - offset)));
        //}

        //Циклічний зсув вправо
        private ulong CyclicShiftRight(ulong a, int offset)
        {
            return (a >> offset | a << (W - offset));
        }
        private uint CyclicShiftRight(uint a, int offset)
        {
            return (a >> offset | a << (W - offset));
        }
        //private ushort CyclicShiftRight(ushort a, int offset)
        //{
        //    return ((ushort)(a >> offset | a << (W - offset)));
        //}

        // Перетворення вхідних даних на беззнакову змінну
        private ulong BytesToUlong(byte[] b, int pos)
        {
            ulong r = 0;
            for (int i = pos + (W / 8 - 1); i > pos; i--)
            {
                r |= (ulong)b[i];
                r <<= 8;
            }
            r |= (ulong)b[pos];
            return r;
        }
        private uint BytesToUint(byte[] b, int pos)
        {
            uint r = 0;
            for (int i = pos + (W / 8 - 1); i > pos; i--)
            {
                r |= (uint)b[i];
                r <<= 8;
            }
            r |= (uint)b[pos];
            return r;
        }
        private ushort BytesToUshort(byte[] b, int pos)
        {
            ushort r = 0;
            for (int i = pos + 1; i > pos; i--)
            {
                r |= (ushort)b[i];
                r <<= 8;
            }
            r |= (ushort)b[pos];
            return r;
        }

        // Перетворення беззнакових змін в послідовність байтів
        private void UlongToBytes(ulong a, byte[] b, int p)
        {
            b[p] = (byte)(a & 0xFF);
            for (int i = 1; i < W / 8; i++)
            {
                a >>= 8;
                b[p + i] = (byte)(a & 0xFF);
            }
        }
        private void UintToBytes(uint a, byte[] b, int p)
        {
            b[p] = (byte)(a & 0xFF);
            for (int i = 1; i < W / 8; i++)
            {
                a >>= 8;
                b[p + i] = (byte)(a & 0xFF);
            }
        }
        private void UshortToBytes(ushort a, byte[] b, int p)
        {
            b[p] = (byte)(a & 0xFF);
            for (int i = 1; i < 2; i++)
            {
                a >>= 8;
                b[p + i] = (byte)(a & 0xFF);
            }
        }

        // Шифрування
        public void Encryption(byte[] inBuf, byte[] outBuf)
        {
            if (W == 64)
            {
                ulong a = BytesToUlong(inBuf, 0);
                ulong b = BytesToUlong(inBuf, 8);

                a = a + S_long[0];
                b = b + S_long[1];

                for (int i = 1; i < R + 1; i++)
                {
                    a = CyclicShiftLeft((a ^ b), (int)b) + S_long[2 * i];
                    b = CyclicShiftLeft((b ^ a), (int)a) + S_long[2 * i + 1];
                }

                UlongToBytes(a, outBuf, 0);
                UlongToBytes(b, outBuf, 8);
            } else if (W == 32)
            {
                uint a = BytesToUint(inBuf, 0);
                uint b = BytesToUint(inBuf, 4);

                a = a + (uint)S_long[0];
                b = b + (uint)S_long[1];

                for (int i = 1; i < R + 1; i++)
                {
                    a = (uint)(CyclicShiftLeft((a ^ b), (int)b) + S_long[2 * i]);
                    b = (uint)(CyclicShiftLeft((b ^ a), (int)a) + S_long[2 * i + 1]);
                }

                UintToBytes(a, outBuf, 0);
                UintToBytes(b, outBuf, 4);
            }
            else
            {
                ushort a = BytesToUshort(inBuf, 0);
                ushort b = BytesToUshort(inBuf, 2);


                a = (ushort)(a + S_short[0]);
                b = (ushort)(b + S_short[1]);

                //ushort tmp_a = (ushort)(a ^ b);
                //Console.WriteLine($"Після XOR {(ushort)tmp_a}");
                ////tmp_a = (ushort)tmp_a;

                //tmp_a = CyclicShiftLeft(tmp_a, (int)b);
                //Console.WriteLine($"Після зсуву вправо {(ushort)tmp_a}");
                ////tmp_a = (ushort)tmp_a;

                //tmp_a = (ushort)(tmp_a + S_short[2]);
                //Console.WriteLine($"Після додавання {(ushort)tmp_a}");
                ////tmp_a = (ushort)tmp_a;

                //ushort tmp_b = (ushort)(b ^ tmp_a);
                //Console.WriteLine($"Після XOR {(ushort)tmp_b}");
                ////tmp_b = (ushort)tmp_b;

                //tmp_b = CyclicShiftLeft(tmp_b, (int)tmp_a);
                //Console.WriteLine($"Після зсуву вправо {(ushort)tmp_b}");
                ////tmp_b = (ushort)tmp_b;

                //tmp_b = (ushort)(tmp_b + S_short[3]);
                //Console.WriteLine($"Після додавання {(ushort)tmp_b}");
                ////tmp_b = (ushort)tmp_b;

                for (int i = 1; i < R + 1; i++)
                {
                    //ushort tmp_a = (ushort)(a ^ b);
                    //Console.WriteLine($"Після XOR {(ushort)tmp_a}");
                    ////tmp_a = (ushort)tmp_a;

                    //tmp_a = CyclicShiftLeft(tmp_a, (int)b);
                    //Console.WriteLine($"Після зсуву вправо {(ushort)tmp_a}");

                    //tmp_a = (ushort)(tmp_a + S_short[2]);
                    //Console.WriteLine($"Після додавання {(ushort)tmp_a}");

                    //ushort tmp_b = (ushort)(b ^ tmp_a);
                    //Console.WriteLine($"Після XOR {(ushort)tmp_b}");

                    //tmp_b = CyclicShiftLeft(tmp_b, (int)tmp_a);
                    //Console.WriteLine($"Після зсуву вправо {(ushort)tmp_b}");

                    //tmp_b = (ushort)(tmp_b + S_short[3]);
                    //Console.WriteLine($"Після додавання {(ushort)tmp_b}");
                    a = (ushort)(CyclicShiftLeft((ushort)(a ^ b), (int)b) + S_short[2 * i]);

                    b = (ushort)(CyclicShiftLeft((ushort)(b ^ a), (int)a) + S_short[2 * i + 1]);
                }

                UshortToBytes((ushort)a, outBuf, 0);
                UshortToBytes((ushort)b, outBuf, 2);
            }
        }

        // Розшифрування
        public void Dencryption(byte[] inBuf, byte[] outBuf)
        {
            if (W == 64)
            {
                ulong a = BytesToUlong(inBuf, 0);
                ulong b = BytesToUlong(inBuf, W / 8);

                for (int i = R; i > 0; i--)
                {
                    b = CyclicShiftRight((b - S_long[2 * i + 1]), (int)a) ^ a;
                    a = CyclicShiftRight((a - S_long[2 * i]), (int)b) ^ b;
                }

                b = b - S_long[1];
                a = a - S_long[0];

                UlongToBytes(a, outBuf, 0);
                UlongToBytes(b, outBuf, W / 8);
            }
            else if (W == 32)
            {
                uint a = BytesToUint(inBuf, 0);
                uint b = BytesToUint(inBuf, 4);

                for (int i = R; i > 0; i--)
                {
                    b = (uint)(CyclicShiftRight((uint)(b - S_long[2 * i + 1]), (int)a) ^ a);
                    a = (uint)(CyclicShiftRight((uint)(a - S_long[2 * i]), (int)b) ^ b);
                }

                b = b - (uint)S_long[1];
                a = a - (uint)S_long[0];

                UintToBytes(a, outBuf, 0);
                UintToBytes(b, outBuf, 4);
            }
            else
            {
                ushort a = BytesToUshort(inBuf, 0);
                ushort b = BytesToUshort(inBuf, 2);

                //Console.WriteLine($"Initial B: {b}");

                //ushort tmp_b = (ushort)(b - S_short[3]);
                //Console.WriteLine($"Після віднімання {(ushort)tmp_b}");
                ////tmp_b = (ushort)tmp_b;

                //tmp_b = CyclicShiftRight(tmp_b, (int)a);
                //Console.WriteLine($"Після зсуву вправо {(ushort)tmp_b}");
                ////tmp_b = (ushort)tmp_b;

                //tmp_b = (ushort)(tmp_b ^ a);
                //Console.WriteLine($"Після XOR {(ushort)tmp_b}");
                ////tmp_b = (ushort)tmp_b;

                //Console.WriteLine($"Initial a {a}");

                //ushort tmp_a = (ushort)(a - S_short[2]);
                //Console.WriteLine($"Після віднімання {(ushort)tmp_a}");
                ////tmp_a = (ushort)tmp_a;

                //tmp_a = CyclicShiftRight(tmp_a, (int)tmp_b);
                //Console.WriteLine($"Після зсуву вправо {(ushort)tmp_a}");
                ////tmp_a = (ushort)tmp_a;

                //tmp_a = (ushort)(tmp_a ^ tmp_b);
                //Console.WriteLine($"Після XOR {(ushort)tmp_a}");
                ////tmp_a = (ushort)tmp_a;

                for (int i = R; i > 0; i--)
                {
                    b = (ushort)(CyclicShiftRight((ushort)(b - S_short[2 * i + 1]), (int)a) ^ a);

                    a = (ushort)(CyclicShiftRight((ushort)(a - S_short[2 * i]), (int)b) ^ b);
                }

                b = (ushort)(b - S_short[1]);
                a = (ushort)(a - S_short[0]);

                UshortToBytes(a, outBuf, 0);
                UshortToBytes(b, outBuf, 2);
            }
        }
    }
}

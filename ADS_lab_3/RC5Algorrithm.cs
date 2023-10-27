using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private const ulong PW_64 = 0xB7E151628AED2A6B;
        private const ulong QW_64 = 0x9E3779B97F4A7C15;

        //32-бітні константи
        private const uint PW_32 = 0xB7E15163;
        private const uint QW_32 = 0x9E3779B9;

        //16-бітні константи
        private const ushort PW_16 = 0xB7E1;
        private const ushort QW_16 = 0x9E37;

        private readonly ulong[] S;        
        public RC5Algorrithm(int w, int r, byte[] b)
        {
            W = w;
            R = r;
            B = b.Length;

            S = CreateSubkyes(b);
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

        //Циклічний зсув вліво
        private ulong CyclicShiftLeft(ulong a, int offset)
        {
            return (a << (offset % W)) | (a >> (W - (offset % W)));
        }
        private uint CyclicShiftLeft(uint a, int offset)
        {
            return (a << (offset % W)) | (a >> (W - (offset % W)));
        }
        private ushort CyclicShiftLeft(ushort a, int offset) 
        {
            return (ushort)((a << (offset % W)) | (a >> (W - (offset % W))));
        }

        //Циклічний зсув вправо
        private ulong CyclicShiftRight(ulong a, int offset)
        {
            return (a >> (offset % W)) | (a << (W - (offset % W)));
        }
        private uint CyclicShiftRight(uint a, int offset)
        {
            return (a >> (offset % W)) | (a << (W - (offset % W)));
        }
        private ushort CyclicShiftRight(ushort a, int offset)
        {
            return (ushort)((a >> (offset % W)) | (a << (W - (offset % W))));
        }


        // Перетворення вхідних даних на беззнакову змінну
        private ulong BytesToUlong(byte[] inputData, int position)
        {
            ulong x = (ulong)(inputData[position] & 0xFF);
            int offset = 8;
            for (int i = 1; i < 8; i++)
            {
                x += (ulong)(inputData[position + i] & 0xFF) << offset;
                offset += 8;
            }

            return x;
        }
        private uint BytesToUint(byte[] inputData, int position)
        {
            uint x = (uint)(inputData[position] & 0xFF);
            int offset = 8;
            for (int i = 1; i < 4; i++)
            {
                x += (uint)(inputData[position + i] & 0xFF) << offset;
                offset += 8;
            }

            return x;
        }
        private ushort BytesToUshort(byte[] inputData, int position)
        {
            ushort x = (ushort)(inputData[position] & 0xFF);
            int offset = 8;
            for (int i = 1; i < 2; i++)
            {
                x += (ushort)((ushort)(inputData[position + i] & 0xFF) << offset);
                offset += 8;
            }

            return x;
        }

        // Перетворення беззнакових змінних в масив байтів
        private void UlongToBytes(ulong x, byte[] outputData, int position)
        {
            int offset = 0;
            for (int i = 0; i < 8; i++)
            {
                outputData[i + position] = (byte)((x >> offset) & 0xFF);
                offset += 8;
            }
        }
        private void UintToBytes(uint x, byte[] outputData, int position)
        {
            int offset = 0;
            for (int i = 0; i < 4; i++)
            {
                outputData[i + position] = (byte)((x >> offset) & 0xFF);
                offset += 8;
            }
        }
        private void UshortToBytes(ushort x, byte[] outputData, int position)
        {
            int offset = 0;
            for (int i = 0; i < 2; i++)
            {
                outputData[i + position] = (byte)((x >> offset) & 0xFF);
                offset += 8;
            }
        }

        // Шифрування
        public void Encryption(byte[] inputData, byte[] outputData)
        {
            if (W == 64)
            {
                ulong a = BytesToUlong(inputData, 0);
                ulong b = BytesToUlong(inputData, 8);

                a = a + S[0];
                b = b + S[1];

                for (int i = 1; i < R + 1; i++)
                {
                    a = CyclicShiftLeft((a ^ b), (int)b) + S[2 * i];
                    b = CyclicShiftLeft((b ^ a), (int)a) + S[2 * i + 1];
                }

                UlongToBytes(a, outputData, 0);
                UlongToBytes(b, outputData, 8);
            } else if (W == 32)
            {
                uint a = BytesToUint(inputData, 0);
                uint b = BytesToUint(inputData, 4);

                a = a + (uint)S[0];
                b = b + (uint)S[1];

                for (int i = 1; i < R + 1; i++)
                {
                    a = (uint)(CyclicShiftLeft((a ^ b), (int)b) + S[2 * i]);
                    b = (uint)(CyclicShiftLeft((b ^ a), (int)a) + S[2 * i + 1]);
                }

                UintToBytes(a, outputData, 0);
                UintToBytes(b, outputData, 4);
            }
            else
            {
                ushort a = BytesToUshort(inputData, 0);
                ushort b = BytesToUshort(inputData, 2);

                a = (ushort)(a + S[0]);
                b = (ushort)(b + S[1]);

                for (int i = 1; i < R + 1; i++)
                {
                    a = (ushort)(CyclicShiftLeft((ushort)(a ^ b), (int)b) + S[2 * i]);
                    b = (ushort)(CyclicShiftLeft((ushort)(b ^ a), (int)a) + S[2 * i + 1]);
                }

                UshortToBytes(a, outputData, 0);
                UshortToBytes(b, outputData, 2);
            }
        }


        // Розшифрування
        public void Dencryption(byte[] inputData, byte[] outputData)
        {
            if (W == 64)
            {
                ulong a = BytesToUlong(inputData, 0);
                ulong b = BytesToUlong(inputData, 8);

                for (int i = R; i > 0; i--)
                {
                    b = CyclicShiftRight((b - S[2 * i + 1]), (int)a) ^ a;
                    a = CyclicShiftRight((a - S[2 * i]), (int)b) ^ b;
                }

                b = b - S[1];
                a = a - S[0];

                UlongToBytes(a, outputData, 0);
                UlongToBytes(b, outputData, 8);
            }
            else if (W == 32)
            {
                uint a = BytesToUint(inputData, 0);
                uint b = BytesToUint(inputData, 4);

                for (int i = R; i > 0; i--)
                {
                    b = (CyclicShiftRight((uint)(b - S[2 * i + 1]), (int)a) ^ a);
                    a = (CyclicShiftRight((uint)(a - S[2 * i]), (int)b) ^ b);
                }

                a = a - (uint)S[0];
                b = b - (uint)S[1];

                UintToBytes(a, outputData, 0);
                UintToBytes(b, outputData, 4);
            }
            else
            {
                ushort a = BytesToUshort(inputData, 0);
                ushort b = BytesToUshort(inputData, 2);

                for (int i = R; i > 0; i--)
                {
                    b = (ushort)(CyclicShiftRight((ushort)(b - S[2 * i + 1]), (int)a) ^ a);

                    a = (ushort)(CyclicShiftRight((ushort)(a - S[2 * i]), (int)b) ^ b);
                }

                b = (ushort)(b - S[1]);
                a = (ushort)(a - S[0]);

                UshortToBytes(a, outputData, 0);
                UshortToBytes(b, outputData, 2);
            }
        }
    }
}

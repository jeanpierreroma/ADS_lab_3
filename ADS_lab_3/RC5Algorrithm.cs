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
        const int W = 64;

        // Кількість раундів
        const int R = 16;

        const UInt64 PW = 0xB7E151628AED2A6B;        // 64-бітна константа
        const UInt64 QW = 0x9E3779B97F4A7C15;        // 64-бітна константа

        UInt64[] L;                                  // масив слів для ключа користувача
        UInt64[] S;                                  // таблица розширених ключів
        int t;                                       // розмір таблиці
        int b;                                       // довжина ключа в байтах

        public RC5Algorrithm(byte[] key)
        {
            /*
             *  Перед непосредственно шифрованием или расшифровкой данных выполняется процедура расширения ключа.
             *  Процедура генерации ключа состоит из четырех этапов:
             *      1. Генерация констант
             *      2. Разбиение ключа на слова
             *      3. Построение таблицы расширенных ключей
             *      4. Перемешивание
             */

            // основные переменные
            UInt64 x, y;
            int i, j, n;

            /*
             * Этап 1. Генерация констант
             * Для заданного параметра W генерируются две псевдослучайные величины,
             * используя две математические константы: e (экспонента) и f (Golden ratio).
             * Qw = Odd((e - 2) * 2^W;
             * Pw = Odd((f - 1) * 2^W;
             * где Odd() - это округление до ближайшего нечетного целого.
             *
             * Для оптимизации алгоритмы эти 2 величины определены заранее (см. константы выше).
             */

            /*
             * Этап 2. Разбиение ключа на слова
             * На этом этапе происходит копирование ключа K[0]..K[255] в массив слов L[0]..L[c-1], где
             * c = b/u, а u = W/8. Если b не кратен W/8, то L[i] дополняется нулевыми битами до ближайшего
             * большего размера c, при котором длина ключа b будет кратна W/8.
             */

            // кількість байтів у слові
            int u = W / 8;

            b = key.Length;

            // розмір масива слів L
            int c = b % u > 0 ? b / u + 1 : b / u;
            L = new UInt64[c];

            for (i = b - 1; i >= 0; i--)
            {
                L[i / u] = CyclicShiftLeft(L[i / u], 8) + key[i];
            }

            /* Этап 3. Построение таблицы расширенных ключей
             * На этом этапе происходит построение таблицы расширенных ключей S[0]..S[2(R + 1)],
             * которая выполняется следующим образом:
             */

            t = 2 * (R + 1);
            S = new UInt64[t];
            S[0] = PW;
            for (i = 1; i < t; i++)
            {
                S[i] = S[i - 1] + QW;
            }

            /* Этап 4. Перемешивание
             * Циклически выполняются следующие действия:
             */

            x = y = 0;
            i = j = 0;
            n = 3 * Math.Max(t, c);

            for (int k = 0; k < n; k++)
            {
                x = S[i] = CyclicShiftLeft((S[i] + x + y), 3);
                y = L[j] = CyclicShiftLeft((L[j] + x + y), (int)(x + y));
                i = (i + 1) % t;
                j = (j + 1) % c;
            }
        }

        /// <summary>
        /// Циклический сдвиг битов слова влево
        /// </summary>
        /// <param name="a">машинное слово: 64 бита</param>
        /// <param name="offset">смещение</param>
        /// <returns>машинное слово: 64 бита</returns>
        private UInt64 CyclicShiftLeft(UInt64 a, int offset)
        {
            return (a << offset | a >> (W - offset));
        }

        /// <summary>
        /// Циклический сдвиг битов слова вправо
        /// </summary>
        /// <param name="a">машинное слово: 64 бита</param>
        /// <param name="offset">смещение</param>
        /// <returns>машинное слово: 64 бита</returns>
        private UInt64 ROR(UInt64 a, int offset)
        {
            UInt64 r1, r2;
            r1 = a >> offset;
            r2 = a << (W - offset);
            return (r1 | r2);

        }

        /// <summary>
        /// Свертка слова (64 бит) по 8-ми байтам
        /// </summary>
        /// <param name="b">массив байтов</param>
        /// <param name="p">позиция</param>
        /// <returns></returns>
        private static UInt64 BytesToUInt64(byte[] b, int p)
        {
            UInt64 r = 0;
            for (int i = p + 7; i > p; i--)
            {
                r |= (UInt64)b[i];
                r <<= 8;
            }
            r |= (UInt64)b[p];
            return r;
        }

        /// <summary>
        /// Развертка слова (64 бит) по 8-ми байтам
        /// </summary>
        /// <param name="a">64-битное слово</param>
        /// <param name="b">массив байтов</param>
        /// <param name="p">позиция</param>
        private static void UInt64ToBytes(UInt64 a, byte[] b, int p)
        {
            for (int i = 0; i < 7; i++)
            {
                b[p + i] = (byte)(a & 0xFF);
                a >>= 8;
            }
            b[p + 7] = (byte)(a & 0xFF);
        }

        /// <summary>
        /// Операция шифрования
        /// </summary>
        /// <param name="inBuf">входной буфер для шифруемых данных (64 бита)</param>
        /// <param name="outBuf">выходной буфер (64 бита)</param>
        public void Cipher(byte[] inBuf, byte[] outBuf)
        {
            UInt64 a = BytesToUInt64(inBuf, 0);
            UInt64 b = BytesToUInt64(inBuf, 8);

            a = a + S[0];
            b = b + S[1];

            for (int i = 1; i < R + 1; i++)
            {
                a = CyclicShiftLeft((a ^ b), (int)b) + S[2 * i];
                b = CyclicShiftLeft((b ^ a), (int)a) + S[2 * i + 1];
            }

            UInt64ToBytes(a, outBuf, 0);
            UInt64ToBytes(b, outBuf, 8);
        }

        /// <summary>
        /// Операция расшифрования
        /// </summary>
        /// <param name="inBuf">входной буфер для шифруемых данных (64 бита)</param>
        /// <param name="outBuf">выходной буфер (64 бита)</param>
        public void Decipher(byte[] inBuf, byte[] outBuf)
        {
            UInt64 a = BytesToUInt64(inBuf, 0);
            UInt64 b = BytesToUInt64(inBuf, 8);

            for (int i = R; i > 0; i--)
            {
                b = ROR((b - S[2 * i + 1]), (int)a) ^ a;
                a = ROR((a - S[2 * i]), (int)b) ^ b;
            }

            b = b - S[1];
            a = a - S[0];

            UInt64ToBytes(a, outBuf, 0);
            UInt64ToBytes(b, outBuf, 8);
        }
    }
}

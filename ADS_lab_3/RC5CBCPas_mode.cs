using ADS_lab_1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADS_lab_3
{
    public class RC5CBCPas_mode
    {
        private readonly int blockSize;
        private RC5Algorrithm rc5;
        private LinearCongruentialGenerator generator;

        public RC5CBCPas_mode(int w, int r, byte[] key)
        {
            blockSize = 2 * (w / 8);
            rc5 = new RC5Algorrithm(w, r, key);
            generator = new LinearCongruentialGenerator();
        }

        public byte[] Encrypt(byte[] plainText, out byte[] cryptText)
        {
            // Зробити доповнення (масив рандому на початку і вкінці заокруглення)
            var appendedText = AppendText(plainText);

            // Шифрування
            cryptText = new byte[appendedText.Length];

            // Перерший блок шифрується ECB
            byte[] inputData_randomArray = new byte[blockSize];
            byte[] outputData_randomArray = new byte[blockSize];

            Buffer.BlockCopy(appendedText, 0, inputData_randomArray, 0, blockSize);

            rc5.Encryption(inputData_randomArray, outputData_randomArray);

            Buffer.BlockCopy(outputData_randomArray, 0, cryptText, 0, blockSize);

            // Шифрування в режимі CBC
            int iterationCount = appendedText.Length / blockSize - 1;

            for (int i = 0; i < iterationCount; i++)
            {
                byte[] previousBlock = new byte[blockSize];
                Buffer.BlockCopy(cryptText, i * blockSize, previousBlock, 0, blockSize);

                byte[] nextPlainBlock = new byte[blockSize];
                Buffer.BlockCopy(appendedText, (i + 1) * blockSize, nextPlainBlock, 0, blockSize);

                // Операція XOR попереднього результату з поточних
                byte[] currentBlock = new byte[blockSize];
                for (int j = 0; j < currentBlock.Length; j++)
                {
                    currentBlock[j] = (byte)(previousBlock[j] ^ nextPlainBlock[j]);
                }

                byte[] output = new byte[blockSize];
                rc5.Encryption(currentBlock, output);

                Buffer.BlockCopy(output, 0, cryptText, (i + 1) * blockSize, blockSize);
            }

            return cryptText;
        }



        public void Decrypt(string cryptedText)
        {

        }

        private byte[] AppendText(byte[] inputText)
        {
            int lengthRandomArray = blockSize;
            int lenghtAppendedText = inputText.Length % blockSize == 0 ? blockSize : inputText.Length % blockSize;

            int lengthFullAppendedText = lengthRandomArray + inputText.Length + lenghtAppendedText;
            byte[] appendedText = new byte[lengthFullAppendedText];

            var iv = RandomArray(lengthRandomArray);

            byte[] appendedTextEnd = new byte[lenghtAppendedText];
            for (int i = 0; i < appendedTextEnd.Length; i++)
            {
                appendedTextEnd[i] = (byte)lenghtAppendedText;
            }

            // Копіювання рандомних значень
            Buffer.BlockCopy(iv, 0, appendedText, 0, iv.Length);

            // Копівання звичайного тексту
            Buffer.BlockCopy(inputText, 0, appendedText, iv.Length, inputText.Length);

            // Копіювання доповнювльного тексту
            Buffer.BlockCopy(appendedTextEnd, 0, appendedText, inputText.Length + iv.Length, appendedTextEnd.Length);

            return appendedText;
        }

        private byte[] RandomArray(int length)
        {
            byte[] random = new byte[length];
            var sequence = generator.GenerateSequence(length);

            
            int i = 0;
            foreach (var value in sequence)
            {
                random[i] = (byte)value;
                i++;
            }

            return random;

        }
    }
}

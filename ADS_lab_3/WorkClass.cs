using ADS_lab_2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADS_lab_3
{
    public class WorkClass
    {
        private const string DefaultRoutePlainTextFile = "D:\\University\\7_term\\Application and data security\\Labs\\Labs\\Lab_3\\TestFolder\\PlainText\\";
        private const string DefaultRouteCtyptedTextFile = "D:\\University\\7_term\\Application and data security\\Labs\\Labs\\Lab_3\\TestFolder\\CryptedText\\";
        private const string DefaultRouteDecryptedTextFile = "D:\\University\\7_term\\Application and data security\\Labs\\Labs\\Lab_3\\TestFolder\\DecryptedText\\";


        private readonly int[] wArray = { 16, 32, 64, 16, 32, 64, 16, 32, 64, 16, 32, 64, 16, 32, 64, 16, 32, 64, 16, 32, 64, 16, 32, 64, 16 };
        private readonly int[] rArray = { 8, 12, 16, 20, 8, 12, 16, 20, 8, 12, 16, 20, 8, 12, 16, 20, 8, 12, 16, 20, 8, 12, 16, 20, 8 };
        private readonly int[] bArray = { 16, 16, 32, 16, 32, 8, 8, 16, 32, 16, 8, 16, 32, 32, 16, 8, 8, 16, 32, 32, 16, 8, 32, 8, 8 };

        private int variant;

        public void MyMenu ()
        {

            Console.WriteLine("Lab 3 ASD: RC5 implementation\n");

            int initialChoice = InitialMenu();

            if (initialChoice == 1)
            {
                Console.WriteLine("\n\nAvailable variants:\n");
                ShowAvailableVariants();
                Console.WriteLine("\nChoose variant:");

                variant = VariantMenu();
                variant = variant - 1;
            }
            else
            {
                Environment.Exit(0);
            }

            do
            {
                int secondChoice = SecondMenu();

                if (secondChoice == 1)
                {
                    Console.WriteLine("Enter password key");
                    string password = Console.ReadLine();

                    Console.WriteLine("Enter file name");
                    string fileName = Console.ReadLine();

                    EnctyptFile(password, fileName);

                    Console.WriteLine("File was successful encrypted");
                }
                else if (secondChoice == 2)
                {
                    Console.WriteLine("Enter password key");
                    string password = Console.ReadLine();

                    Console.WriteLine("Enter file name");
                    string fileName = Console.ReadLine();


                    Console.WriteLine("File was successful decrypted");
                }
                else
                {
                    Environment.Exit(0);
                }

            } while (true);
            
        }
        private int InitialMenu()
        {
            bool firstChoiceResult = false;
            bool isNotExit = true;
            int choice = 0;

            do
            {
                Console.WriteLine("Choose actions:");
                Console.WriteLine("1. Choose variant");
                Console.WriteLine("2. Exit");

                firstChoiceResult = int.TryParse(Console.ReadLine(), out choice);

                if (!firstChoiceResult)
                {
                    Console.WriteLine("You entered not an integer number!");
                }
                else if (choice != 1 && choice != 2)
                {
                    Console.WriteLine("You entered number out of range!");
                }
                else
                {
                    isNotExit = false;
                }
            } while (isNotExit);

            return choice;
        }
        private int VariantMenu()
        {
            bool varianChoiceResult = false;
            bool isNotExit = true;
            int variant;
            do
            {
                varianChoiceResult = int.TryParse(Console.ReadLine(), out variant);

                if (!varianChoiceResult)
                {
                    Console.WriteLine("You entered not an integer number!");
                }
                else if (variant < 1 && variant > 25)
                {
                    Console.WriteLine("You entered number out of range!");
                }
                else
                {
                    isNotExit = false;
                }
            } while (isNotExit);

            return variant;
        }
        private int SecondMenu()
        {
            bool secondChoiceResult = false;
            bool isNotExit = true;
            int choice = 0;

            do
            {
                Console.WriteLine("Choose actions:");
                Console.WriteLine("1. Encrypt the file");
                Console.WriteLine("2. Decrypt the file");
                Console.WriteLine("3. Exit");

                secondChoiceResult = int.TryParse(Console.ReadLine(), out choice);

                if (!secondChoiceResult)
                {
                    Console.WriteLine("You entered not an integer number!");
                }
                else if (choice != 1 && choice != 2 && choice != 3)
                {
                    Console.WriteLine("You entered number out of range!");
                }
                else
                {
                    isNotExit = false;
                }
            } while (isNotExit);

            return choice;
        }
        private void ShowAvailableVariants()
        {
            Console.WriteLine("{0,-15}{1,-20}{2,-15}{3,-15}", "№ variant", "Word length (w)", "Rounds (r)", "Key length (b)");
            Console.WriteLine(new string('-', 70));

            for (int i = 0; i < rArray.Length; i++)
            {
                Console.WriteLine("{0,-15}{1,-20}{2,-15}{3,-15}", i + 1, wArray[i], rArray[i], bArray[i]);
                Console.WriteLine(new string('-', 70));
            }
        }
    
        private void EnctyptFile(string password, string fileName)
        {
            byte[] key = KeyAccordingToVariant(password);

            // Зчитуємо дані з файлу
            byte[] plainText = ReadFromFile(DefaultRoutePlainTextFile + fileName);

            RC5CBCPas_mode rc5Mode = new RC5CBCPas_mode(wArray[variant], rArray[variant], key);

            rc5Mode.Encrypt(plainText, out byte[] cryptText);

            // Записуємо дані в файл
            WriteIntoFile(DefaultRouteCtyptedTextFile + fileName, cryptText);
        }

        private void DenctyptFile(string password, string fileName)
        {
            byte[] key = KeyAccordingToVariant(password);

            // Зчитуємо дані з файлу
            byte[] cryptedText = ReadFromFile(DefaultRouteCtyptedTextFile + fileName);

            RC5CBCPas_mode rc5Mode = new RC5CBCPas_mode(wArray[variant], rArray[variant], key);

            rc5Mode.Dencrypt(cryptedText, out byte[] decryptText);

            // Записуємо дані в файл
            WriteIntoFile(DefaultRouteDecryptedTextFile + fileName, decryptText);
        }

        private byte[] KeyAccordingToVariant(string password)
        {
            var hash = HashPassword(Encoding.UTF8.GetBytes(password));

            byte[] key;
            if (bArray[variant] == 8)
            {
                key = hash.Skip(8).ToArray();
            }
            else if (bArray[variant] == 16)
            {
                key = hash;
            }
            else
            {
                // Обчислюємо MD5 хеш від старших 128 бітів
                byte[] newHash = HashPassword(hash);

                // Об'єднуємо два MD5 хеші
                byte[] combinedHash = new byte[32];
                Array.Copy(hash, combinedHash, 16);
                Array.Copy(newHash, 0, combinedHash, 16, 16);

                key = combinedHash;
            }

            return key;
        }

        private string DecryptUTF8Bytes(byte[] encryptedBytes)
        {
            try
            {
                string decryptedText = Encoding.UTF8.GetString(encryptedBytes);
                return decryptedText;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Помилка розшифровки: " + ex.Message);
                return null;
            }
        }

        private byte[] GetArrayBytesFromMD5(uint[] md5Result)
        {
            byte[] resultBytes = new byte[md5Result.Length * 4]; // 4 байти в кожному uint

            for (int i = 0; i < md5Result.Length; i++)
            {
                byte[] uintBytes = BitConverter.GetBytes(md5Result[i]);
                Array.Copy(uintBytes, 0, resultBytes, i * 4, 4);
            }

            return resultBytes;
        }

        private byte[] HashPassword(byte[] password)
        {
            var md5Result = MD5Algorythm.Algorithm(password);

            var result = GetArrayBytesFromMD5(md5Result);

            return result;
        }

        private byte[] ReadFromFile(string fileName)
        {
            // Написати виключення
            var fileContent = File.ReadAllText(fileName);

            return Encoding.UTF8.GetBytes(fileContent);
        }

        private void WriteIntoFile(string fileName, byte[] content)
        {
            string stringContent = DecryptUTF8Bytes(content);
            File.WriteAllText(fileName, stringContent);
        }
    }

}

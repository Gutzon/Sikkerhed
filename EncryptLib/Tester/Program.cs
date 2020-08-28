using System;
using EncryptLib;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            string v = "";
            Encryption crypt = new Encryption("keyeee");
            Encryption crypt2 = new Encryption("keyeee");
            Keyexchange keye = new Keyexchange();
            Console.WriteLine("Hello World!");
            while (true)
            {
            v = Console.ReadLine();
                string text = "";
                foreach (byte b in crypt.AESencrypted(v))
                {
                    text += b.ToString();
                }
                
            Console.WriteLine(crypt.AESencrypted(v));
            Console.WriteLine(crypt2.AESdecrypted(crypt.AESencrypted(v)));
                text = "";
                foreach (byte b in crypt.myAes.Key)
                {
                    text += b.ToString();
                }
                Console.WriteLine("cryp:  "+text);
                text = "";
                foreach (byte b in crypt2.myAes.Key)
                {
                    text += b.ToString();
                }
                Console.WriteLine("cryp2: "+text);
                text = "";
                foreach (byte b in crypt.myAes.IV)
                {
                    text += b.ToString();
                }
                Console.WriteLine("iv:  " + text);
                text = "";
                foreach (byte b in crypt2.myAes.IV)
                {
                    text += b.ToString();
                }
                Console.WriteLine("iv2: " + text);
                Console.WriteLine("iv2: " + text);
                Console.WriteLine(keye.generatekey(2).ToString());
            }
        }
    }
}

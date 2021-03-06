﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;

namespace EncryptLib
{
    public class Keyexchange
    {
        public double p = 1729;         //public number, should have been a prime number
        public double publickey;        //public key
        int priNum;                     //private key
        Random ran = new Random();

        public Keyexchange()
        {
            generatekeys();
        }

        public void generatekeys()              //generates private and one side of the public key
        {
            priNum = ran.Next(1, 10);
            publickey = ran.Next(0, 20);
        }
        public double generatepublickey(double publickey)       //combine both public keys to give the real public key
        {
            this.publickey += publickey;
            return this.publickey;
        }
        public void CheckP(double prime)                    //checking for which public number (p) it should use
        {
            if (prime > this.p)
            {
                this.p = prime;
            }
        }
        public double Sendkey()                         //Generates it's private key compined with the public key and number to give the other end
        {
            return (double)((decimal)(Math.Pow(publickey, priNum)) % (decimal)p);
        }
        public double generatekey(double otherkey)         //uses the public combined private key of the other to get the real key
        {
            return (double)((decimal)(Math.Pow(otherkey, priNum)) % (decimal)p);
        }


    }
    public class Encryption
    {
        List<char> abc = new List<char>("keya78bcdfjlmnopXSqrsxz0123RTGBN69½§!\"@#£¤ $%&/{(tuvw[])}=+?´`|¨^~'*-_.:;,åøæ<>\\QAZWEDC45VFHYUJMKIOLÆPÅØöÖäghiÄôÔâÂíÍìÌóÓ");
        List<char> playfairCipher = new List<char>();               //My Playfair Ciphe char list
        public Aes myAes = Aes.Create();

        private void generate(string keyword)           //Generate my version of Playfair Ciphe 
        {
            List<char> abc = new List<char>(this.abc);
            int shift = shiftsum(keyword);
            foreach (char c in keyword)
            {
                if (!playfairCipher.Contains(c))
                {
                    abc.Remove(c);
                    playfairCipher.Add(c);
                }
            }
            foreach (char c in abc)
            {
                playfairCipher.Add(c);
            }                                       //Done with Playfair Ciphe
            byte[] mynewkey = new byte[32];         //Starts generating AES.Key and AES.IV using my keyword and my Playfair Ciphe char list
            byte[] mynewiv = new byte[16];
            int i = shift;
            if (keyword.Length >= 32)
            {
                string v = "";
                while (v.Length < 32)
                {
                    v += keyword.ToArray()[v.Length];
                }
            }
            while (keyword.Length < 32)
            {
                keyword += playfairCipher[i].ToString();
                i += shift;
                if (i >= playfairCipher.Count)
                {
                    i -= playfairCipher.Count;
                }
            }
            i = 0;
            int j = 0;
            Boolean boo = false;
            while (i < 32)
            {
                mynewkey[i] = Encoding.ASCII.GetBytes(keyword)[i];
                if (boo && j < 16)
                {
                    mynewiv[j] = Encoding.ASCII.GetBytes(keyword)[i];
                    boo = false;
                }
                else
                {
                    j += 1;
                    boo = true;
                }
                i += 1;
            }

            myAes.Key = mynewkey;
            myAes.IV = mynewiv;                         //Done generating the Key and IV

        }
        public string AESencrypted(string text)
        {
            byte[] bytes = EncryptStringToBytes_Aes(text, myAes.Key, myAes.IV);        //encryts it into bytes
            string v = "";                                                             //Write bytes into a string with extra char to split it up
            foreach (byte b in bytes)
            {
                v += "[" + b.ToString() + "]";
            }                                                                          //Done and gives the encrypted back
            return v;
        }
        public string AESdecrypted(string message)
        {
            List<byte> lbytes = new List<byte>();                                                                   //Splits the string back up into the bytes it was
            while (message.Length > 0)
            {
                lbytes.Add(Byte.Parse(message.Substring(message.IndexOf("[") + 1, message.IndexOf("]") - 1)));
                message = message.Substring(message.IndexOf("]") + 1, message.Length - message.IndexOf("]") - 1);
            }
            byte[] bytes = new byte[lbytes.Count];
            int i = 0;
            while (i < lbytes.Count)
            {
                bytes[i] = lbytes[i];
                i += 1;
            }                                                                                                       //Done


            return DecryptStringFromBytes_Aes(bytes, myAes.Key, myAes.IV);                                          //Decrypts the bytes and sends back the unencrypted string
        }
        private (char, char) Playenen(char c, char j)               // My Playfair Ciphe Encryptions rules for two chars
        {
            int cre = 0;
            int jre = 0;

            int cinX = playfairCipher.IndexOf(c);
            int jinX = playfairCipher.IndexOf(j);
            int cinY = 0;
            int jinY = 0;
            while (cinX > 10)
            {
                cinX -= 11;
                cinY += 1;
            }
            while (jinX > 10)
            {
                jinX -= 11;
                jinY += 1;
            }
            if (cinX == jinX)
            {
                cre = cinX + cinY * 11 + 11;
                jre = jinX + jinY * 11 + 11;
            }
            else if (cinY == jinY)
            {
                cre = cinX + cinY * 11 + 1;
                jre = jinX + jinY * 11 + 1;
            }
            else if (cinX != jinX && cinY != jinY)
            {
                cre = jinX + (cinY * 11);
                jre = cinX + (jinY * 11);
            }


            if (cre >= playfairCipher.Count)
            {
                cre -= playfairCipher.Count;
            }
            if (jre >= playfairCipher.Count)
            {
                jre -= playfairCipher.Count;
            }

            return (playfairCipher[cre], playfairCipher[jre]);
        }
        private (char, char) Playende(char c, char j)           // My Playfair Ciphe Decryptions rules for two chars
        {
            int cre = 0;
            int jre = 0;

            int cinX = playfairCipher.IndexOf(c);
            int jinX = playfairCipher.IndexOf(j);
            int cinY = 0;
            int jinY = 0;
            while (cinX > 10)
            {
                cinX -= 11;
                cinY += 1;
            }
            while (jinX > 10)
            {
                jinX -= 11;
                jinY += 1;
            }
            if (cinX == jinX)
            {
                cre = cinX + cinY * 11 - 11;
                jre = jinX + jinY * 11 - 11;
            }
            else if (cinY == jinY)
            {
                cre = cinX + cinY * 11 - 1;
                jre = jinX + jinY * 11 - 1;
            }
            else if (cinX != jinX && cinY != jinY)
            {
                cre = jinX + (cinY * 11);
                jre = cinX + (jinY * 11);
            }


            if (cre < 0)
            {
                cre += playfairCipher.Count;
            }
            if (jre < 0)
            {
                jre += playfairCipher.Count;
            }

            return (playfairCipher[cre], playfairCipher[jre]);
        }
        public string encrypt(string text)
        {
            int shift = shiftsum(text);
            List<char> cry = new List<char>();
            foreach (char c in text)                //Validates chars for and shifts charectors.
            {
                if (!abc.Contains(c))
                {
                    cry.Add(c);
                }
                else if (playfairCipher.IndexOf(c) + shift >= abc.Count)
                {
                    cry.Add(playfairCipher[playfairCipher.IndexOf(c) + shift - abc.Count]);
                }
                else
                {
                    cry.Add(playfairCipher[playfairCipher.IndexOf(c) + shift]);
                }
            }                                       //done
            int i = 0;
            while (i < cry.Count && true)           //Finds the two chars to run through My Playfair Ciphe
            {
                if ((i + 1) >= cry.Count)
                {
                    i = cry.Count;
                }
                else
                {
                    while (!abc.Contains(cry[i]))
                    {
                        i += 1;
                    }
                    int j = i + 1;
                    while (!abc.Contains(cry[j]))
                    {
                        j += 1;
                    }
                    (char, char) p = Playenen(cry[i], cry[j]);
                    cry[i] = p.Item1;
                    cry[j] = p.Item2;
                    i = j + 1;
                }
            }                                                   //done
            string v = "";
            foreach (char c in cry)                             //Places it into a string and returns it
            {
                v += c;
            }

            return v;
        }

        public string decrypt(string text)                  //Doing almost the same as encrypt but in reverse
        {
            int shift = shiftsum(text);
            List<char> cry = new List<char>(text);
            int doub = 0;
            while (doub == 0)
            {
                int i = doub;
                while (i < cry.Count)
                {
                    if ((i + 1) >= cry.Count)
                    {
                        i = cry.Count;
                    }
                    else
                    {
                        while (!abc.Contains(cry[i]))
                        {
                            i += 1;
                        }
                        int j = i + 1;
                        while (!abc.Contains(cry[j]))
                        {
                            j += 1;
                        }
                        (char, char) p = Playende(cry[i], cry[j]);
                        cry[i] = p.Item1;
                        cry[j] = p.Item2;
                        i = j + 1;
                    }
                }
                doub -= 1;
            }
            string v = "";
            foreach (char c in cry)
            {
                if (!abc.Contains(c))
                {
                    v += c;
                }
                else if (playfairCipher.IndexOf(c) - shift < 0)
                {
                    v += playfairCipher[playfairCipher.IndexOf(c) - shift + abc.Count];
                }
                else
                {
                    v += playfairCipher[playfairCipher.IndexOf(c) - shift];
                }
            }

            return v;
        }
        int shiftsum(string text)               //Finds out the Total Digit Sum of length of the string. Exemple   123 = 6.
        {
            int shift = text.Length;
            while (shift >= 10)
            {
                var v = 0;
                foreach (char c in shift.ToString())
                {
                    v += Int32.Parse(c.ToString());
                }
                shift = v;
            }
            return shift;
        }
        public Encryption(string keyword)
        {
            generate(keyword);
        }



        static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }
        static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            try
                            {
                                plaintext = srDecrypt.ReadToEnd();
                            }
                            catch
                            {
                                return "ERROR Decrypting";
                            }
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.

                        }
                    }
                }
            }

            return plaintext;
        }
    }
}

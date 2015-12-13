using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace org.github.fredjeck.SafeStrings
{
    /// <summary>
    /// SafeStrings is a collection of extensions to the String class allowing to simply and effectively encrypt/decrypt a string.
    /// A typical usage of SafeStrings it to encrypt passwords in your configuration files to avoid storing them as plaintext.
    ///
    /// SafeStrings encrypted strings are prefixed with <code>Prefix</code> (usually "x-enc:"). This allows to safely call the <code>DecryptWithPassword</code> on a string even if it's not encrypted.
    /// 
    /// The resulting encrypted string, stores the Initialization Vector(IV) and key Salt along the encrypted data.
    /// This is a safe practice as those two components are randomly generated each type the <code>EncryptWithPassword</code> method.
    /// You can find more details about this in the following SO post <a href = "http://stackoverflow.com/questions/13901529/symmetric-encryption-aes-is-saving-the-iv-and-salt-alongside-the-encrypted-da" > the
    /// following post.</a>
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Prefix that will be used on encrypted strings.
        /// </summary>
        private const string Prefix = "x-enc:";
        /// <summary>
        /// Size of the salt used to derive a key from a password.
        /// </summary>
        private const int SaltSize = 8;
        /// <summary>
        /// Size of the blocks used by AES
        /// </summary>
        private const int BlockSize = 128;
        /// <summary>
        /// Size of the Initialization Vector.
        /// IV size must always be BlockSize / 8.
        /// </summary>
        private const int IvSize = BlockSize / 8;
        /// <summary>
        /// Size in bytes of the key used to encode strings.
        /// i.e 32 bytes = 256 bits key.
        /// </summary>
        private const int KeySize = 32;
        /// <summary>
        /// Number of iterations used to generate the key.
        /// </summary>
        private const int KeyIterations = 4096;

        /// <summary>
        /// Generates a Salt (random sequence of bytes) using a cryptographically strong number generator
        /// </summary>
        /// <param name="size">The size of the generated salt</param>
        /// <returns>A byte array containing random bytes</returns>
        private static byte[] GenerateRandomBytes(int size)
        {
            using (var generator = new RNGCryptoServiceProvider())
            {
                var salt = new byte[size];
                generator.GetBytes(salt);
                return salt;
            }
        }

        /// <summary>
        /// Generates a key by derivng a password (PBKDF2, by using a pseudo-random number generator based on HMACSHA1)
        /// </summary>
        /// <param name="password">the password used to generate the key</param>
        /// <param name="salt">the salt</param>
        /// <param name="size">the requested key size</param>
        /// <returns>A pseudo random key derived from the given <code>password</code> and <code>salt</code></returns>
        private static byte[] GenerateKey(string password, byte[] salt, int size)
        {
            var generator = new Rfc2898DeriveBytes(password, salt, KeyIterations);
            return generator.GetBytes(size);
        }

        /// <summary>
        /// Encrypts a string using the provided password.
        /// The resulting encrypted string will start with the "x-enc" prefix followed by the encrypted bytes encoded using the Base64Scheme for better readability.
        /// i.e : "x-enc:fzN2O8y7UraR6zk03XSYLZE9A4rSDWHsYNFFlik8+A3sU4Fu4E7QVWgIFZohclexamyPQhX7bRQ="
        /// The encryption is performed with AES using a securely derived key.
        /// The resulting encrypted string is self sufficient which means that it contains all the data needed to decrypt it, provided that the correct password is given.
        /// 
        /// </summary>
        /// <param name="str">The string to encode</param>
        /// <param name="password">The password that will be used to encrypt the string</param>
        /// <returns>An autonomous encrypted string. If any error is raised during the encryption process, this method will return <Code>string.Empty</Code></returns>
        public static string EncryptUsingPassword(this string str, string password)
        {
            if (string.IsNullOrWhiteSpace(str) || string.IsNullOrWhiteSpace(password))
            {
                return string.Empty;
            }

            try
            {
                var salt = GenerateRandomBytes(SaltSize);
                var key = GenerateKey(password, salt, KeySize);
                var iv = GenerateRandomBytes(IvSize);

                using (var aes = new RijndaelManaged())
                {
                    aes.Key = key;
                    aes.IV = iv;
                    aes.BlockSize = BlockSize;

                    var encryptor = aes.CreateEncryptor();
                    using (var memory = new MemoryStream())
                    {
                        using (var crypto = new CryptoStream(memory, encryptor, CryptoStreamMode.Write))
                        using (var writer = new StreamWriter(crypto))
                        {

                            writer.Write(str);
                        }
                        var data = memory.ToArray();

                        var encrypted = new byte[data.Length + IvSize + SaltSize];
                        Array.Copy(iv, 0, encrypted, 0, IvSize);
                        Array.Copy(salt, 0, encrypted, IvSize, SaltSize);
                        Array.Copy(data, 0, encrypted, IvSize + SaltSize, data.Length);
                        return Prefix + Convert.ToBase64String(encrypted);
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// Decodes the string using the given password.
        /// This method can be safely used even on unencrypted strings.
        /// Calling this method on unencrypted strings will return the original string.
        /// </summary>
        /// <param name="str">An x-enc encrypted string</param>
        /// <param name="password">The password that will be used to decrypt the string</param>
        /// <returns>The decrypted string or the original one if any error is raised during the decryption process</returns>
        public static string DecryptUsingPassword(this string str, string password)
        {
            if (string.IsNullOrWhiteSpace(str) || !str.StartsWith(Prefix) || string.IsNullOrWhiteSpace(password))
            {
                return str;
            }
            try
            {
                var bytes = Convert.FromBase64String(str.Replace(Prefix, ""));
                var iv = new byte[IvSize];
                var salt = new byte[SaltSize];
                var data = new byte[bytes.Length - SaltSize - IvSize];
                Array.Copy(bytes, 0, iv, 0, IvSize);
                Array.Copy(bytes, IvSize, salt, 0, SaltSize);
                Array.Copy(bytes, SaltSize + IvSize, data, 0, data.Length);

                var key = GenerateKey(password, salt, KeySize);

                using (var aes = new RijndaelManaged())
                {
                    aes.Key = key;
                    aes.IV = iv;
                    aes.BlockSize = BlockSize;

                    var decryptor = aes.CreateDecryptor();
                    using (var memory = new MemoryStream(data))
                    using (var crypto = new CryptoStream(memory, decryptor, CryptoStreamMode.Read))
                    using (var reader = new StreamReader(crypto))
                    {
                        return reader.ReadToEnd();
                    }

                }
            }
            catch
            {
                return str;
            }
        }
    }
}

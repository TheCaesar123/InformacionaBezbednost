using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace Common
{   
    [DataContract]
    public class _3DES_Algorithm
    {
        private string encrypted = null;
        private string decrypted = null;
        [DataMember]
        public string Encrypted { get => encrypted; set => encrypted = value; }
        [DataMember]
        public  string Decrypted { get => decrypted; set => decrypted = value; }

        public static byte[] Encrypt(string secretKey, CipherMode mode, string message = "Message")
        {

            byte[] body = ASCIIEncoding.ASCII.GetBytes(message);
            byte[] encryptedBody = null;
            TripleDESCryptoServiceProvider tripleDesCryptoProvider = new TripleDESCryptoServiceProvider
            {
                Key = ASCIIEncoding.ASCII.GetBytes(secretKey),
                Mode = mode,
                Padding = PaddingMode.PKCS7
            };


            ICryptoTransform tripleDesEncryptTransform = tripleDesCryptoProvider.CreateEncryptor();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, tripleDesEncryptTransform, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(body, 0, body.Length);
                    encryptedBody = memoryStream.ToArray();
                }
            }
            System.Console.WriteLine("-----------------");
            
            string retVal = ASCIIEncoding.ASCII.GetString(encryptedBody);
            System.Console.WriteLine(retVal);
            System.Console.WriteLine("-----------------");
            return encryptedBody;
        }


        /// <summary>
        /// Function that decrypts the cipher text from inFile and stores as plaintext to outFile
        /// </summary>
        /// <param name="secretKey"> symmetric encryption key </param>
        public static byte[] Decrypt(string secretKey, CipherMode mode, byte[] encryptedBody)
        {
            byte[] body = encryptedBody;         
            byte[] decryptedBody = null;

            TripleDESCryptoServiceProvider tripleDesCryptoProvider = new TripleDESCryptoServiceProvider
            {
                Key = ASCIIEncoding.ASCII.GetBytes(secretKey),
                Mode = mode,
                Padding = PaddingMode.None
            };

            ICryptoTransform tripleDesDecryptTransform = tripleDesCryptoProvider.CreateDecryptor();
            using (MemoryStream memoryStream = new MemoryStream(body))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, tripleDesDecryptTransform, CryptoStreamMode.Read))
                {
                    decryptedBody = new byte[body.Length];       //decrypted image body - the same lenght as encrypted part
                    cryptoStream.Read(decryptedBody, 0, decryptedBody.Length);
                }
            }
            System.Console.WriteLine("-----------------");
            string retVal = ASCIIEncoding.ASCII.GetString(decryptedBody);
            System.Console.WriteLine(retVal);
            System.Console.WriteLine("-----------------");
            return decryptedBody;
        }
    }
}

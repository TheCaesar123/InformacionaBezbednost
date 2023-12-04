﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Common
{

    public class DigitalSignature
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"> a message/text to be digitally signed </param>
        /// <param name="hashAlgorithm"> an arbitrary hash algorithm </param>
        /// <param name="certificate"> certificate of a user who creates a signature </param>
        /// <returns> byte array representing a digital signature for the given message </returns>
        public static byte[] Create(string message, string hashAlgorithm, X509Certificate2 certificate)
        {
            /// Looks for the certificate's private key to sign a message
            /// 
            RSACryptoServiceProvider csp = (RSACryptoServiceProvider)certificate.PrivateKey;

            if (csp == null)
            {
                throw new Exception("Valid certificate was not found.");
            }
            UnicodeEncoding encoding = new UnicodeEncoding();
            byte[] data = encoding.GetBytes(message);
            byte[] hash = null;


            SHA1Managed sha1 = new SHA1Managed();
            hash = sha1.ComputeHash(data);



            /// Use RSACryptoServiceProvider support to create a signature using a previously created hash value
            byte[] signature = csp.SignHash(hash, CryptoConfig.MapNameToOID(hashAlgorithm));
            return signature;
        }


        public static bool Verify(string message, string hashAlgorithm, byte[] signature, X509Certificate2 certificate)
        {
            /// Looks for the certificate's public key to verify a message
            ///

            RSACryptoServiceProvider csp = (RSACryptoServiceProvider)certificate.PublicKey.Key;

            UnicodeEncoding encoding = new UnicodeEncoding();
            byte[] data = encoding.GetBytes(message);
            byte[] hash = null;


            SHA1Managed sha1 = new SHA1Managed();
            hash = sha1.ComputeHash(data);



            /// Use RSACryptoServiceProvider support to compare two - hash value from signature and newly created hash value
            return csp.VerifyHash(hash, CryptoConfig.MapNameToOID(hashAlgorithm), signature);
        }
    }

}

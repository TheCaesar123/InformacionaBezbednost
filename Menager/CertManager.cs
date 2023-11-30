using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Security;
using System.Diagnostics;
using System.Security.Principal;
using System.Threading;

namespace Manager
{
    public class CertManager
    {
        /// <summary>
        /// Get a certificate with the specified subject name from the predefined certificate storage
        /// Only valid certificates should be considered
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="storeLocation"></param>
        /// <param name="subjectName"></param>
        /// <returns> The requested certificate. If no valid certificate is found, returns null. </returns>
        /// 
       
        public static X509Certificate2 GetCertificateFromStorage(StoreName storeName, StoreLocation storeLocation, string subjectName)
        {
            X509Store store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);

            X509Certificate2Collection certCollection = store.Certificates.Find(X509FindType.FindBySubjectName, subjectName, true);

            /// Check whether the subjectName of the certificate is exactly the same as the given "subjectName"
            foreach (X509Certificate2 c in certCollection)
            {
                if (c.SubjectName.Name.Contains(string.Format("CN={0}", subjectName)))
                {
                    //Console.WriteLine(c.SubjectName.Name);
                    return c;
                }

            }

            return null;
        }
        public static string GetGroup(StoreName storeName, StoreLocation storeLocation, string subjectName)
        {

            string group = "";
            //subjectName = "wcfsupervisor";
            X509Store store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);

         

            X509Certificate2Collection certCollection = store.Certificates.Find(X509FindType.FindBySubjectName, subjectName, true);

            foreach (X509Certificate2 c in certCollection)
            {
                if (c.SubjectName.Name.Contains(string.Format("CN={0}", subjectName)))
                {
                    Console.WriteLine(c.SubjectName.Name);
                    if (c.SubjectName.Name.Contains("OU="))
                    {
                        string[] pom = c.SubjectName.Name.Split(','); // CN=Wcf..., OU=group...
                        string[] pom1 = pom[0].Split('=');
                        string[] pom2 = pom[1].Split('=');
                        Console.WriteLine($"Client [{pom1[1].ToUpper()}] is member of the group : {pom2[1].ToUpper()}");
                        group = pom[1];
                        
                        return group;
                    }
                }
            }
            return group;
        }
       
        /// <summary>
        /// Get a certificate from file.		
        /// </summary>
        /// <param name="fileName"> .cer file name </param>
        /// <returns> The requested certificate. If no valid certificate is found, returns null. </returns>
        public static X509Certificate2 GetCertificateFromFile(string fileName)
        {
            X509Certificate2 certificate = null;


            return certificate;
        }

        /// <summary>
        /// Get a certificate from file.
        /// </summary>
        /// <param name="fileName">.pfx file name</param>
        /// <param name="pwd"> password for .pfx file</param>
        /// <returns>The requested certificate. If no valid certificate is found, returns null.</returns>
        public static X509Certificate2 GetCertificateFromFile(string fileName, SecureString pwd)
        {
            X509Certificate2 certificate = null;


            return certificate;
        }
    }
}

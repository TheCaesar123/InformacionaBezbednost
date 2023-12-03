using Client;
using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class EntitetServer : IEntitet
    {
        
        
        public void Modify(string Korisnik, Entitet entitet)
        {
            if (Manager.CertManager.GetGroup(StoreName.My, StoreLocation.LocalMachine, Korisnik).Contains("OU=modify"))
                Console.WriteLine("MODIFY");
            else
            {
                string name = Thread.CurrentPrincipal.Identity.Name;
                DateTime time = DateTime.Now;
                string message = String.Format($"Access is denied. User [{Korisnik.ToUpper()}] tried to call Modify method (time: { time.TimeOfDay}). " +
                    "For this method user needs to be member of group Modify.");
                throw new FaultException<SecurityException>(new SecurityException(message));
            }
        }
        
        public void Read(string Korisnik, Entitet entitet)
        {
            if (Manager.CertManager.GetGroup(StoreName.My, StoreLocation.LocalMachine, Korisnik).Contains("OU=read"))
            {
                Console.WriteLine("READ");
                //string key = SecretKey.LoadKey("D:/Program Files (x86)/TEST/clientKey.txt");
                //_3DES_Algorithm.Decrypt(key, CipherMode.ECB, sdf);
                // Console.WriteLine("{0}", _3DES_Algorithm.Decrypted);
                string cipher = File.ReadAllText("C:/Bezbednost/encripted.txt");
                entitet.Name = "radi";
             
                //Console.WriteLine(_3DES_Algorithm.Decrypt(SecretKey.LoadKey("D:/Program Files (x86)/TEST/clientKey.txt"), CipherMode.ECB, cipher));
            }
            else
            {
                string name = Manager.Formatter.ParseName(Thread.CurrentPrincipal.Identity.Name);
                DateTime time = DateTime.Now;
                string message = String.Format($"Access is denied. User [{Korisnik.ToUpper()}] tried to call Read method (time: {time.TimeOfDay}). " +
                    "For this method user needs to be member of group Read.");
                throw new FaultException<SecurityException>(new SecurityException(message));
            }
        }

       
       

        public void Supervise(string Korisnik, Entitet entitet)
        {
            //Console.WriteLine("SUPERVISE");
            if (Manager.CertManager.GetGroup(StoreName.My, StoreLocation.LocalMachine, Korisnik).Contains("OU=supervise"))
                Console.WriteLine("SUPERVISE");
            else
            {
                string name = Thread.CurrentPrincipal.Identity.Name;
                DateTime time = DateTime.Now;
                string message = String.Format($"Access is denied. User [{Korisnik.ToUpper()}] tried to call Supervise method (time: {time.TimeOfDay}). " +
                    "For this method user needs to be member of group Supervise.");
                throw new FaultException<SecurityException>(new SecurityException(message));
            }
        }
      

        public void TestConnection(string Korinik)
        {
            Console.WriteLine("Klijent {0} povezan na server.", Korinik);
        }
    }
}

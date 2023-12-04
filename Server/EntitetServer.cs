﻿using Client;
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
        public static string DataForClient { get; set; }
        public byte[] DataFromServerToCLient()
        {
            string key = SecretKey.GenerateKey();
            SecretKey.StoreKey(key ,"C:/Bezbednost/clientKey.txt");

            string message = MessageForSend(DataForClient);
            byte[] encriptedDataForClient = _3DES_Algorithm.Encrypt(key, System.Security.Cryptography.CipherMode.ECB, message);
            
            return encriptedDataForClient;
        }
        public string MessageForSend(string message)
        {


            string retVal;
            char[] m = message.ToCharArray();
            if (m.Length % 8 != 0)
            {
                int n = 8 - (m.Length % 8);
                Array.Resize(ref m, m.Length + n);
                for (int i = m.Length; i < 7; i++)
                {
                    m[i] = ' ';
                }
            }
            retVal = new string(m);

            return retVal;

        }
        public void DataFromServerToCLientDecripted()
        {
            throw new NotImplementedException();
        }

        public void Modify(string Korisnik, Entitet entitet, byte[] encripted)
        {
            if (Manager.CertManager.GetGroup(StoreName.My, StoreLocation.LocalMachine, Korisnik).Contains("OU=modify"))
            { 
                Console.WriteLine("MODIFY");
                string key = SecretKey.LoadKey("C:/Bezbednost/clientKey.txt");
                entitet.Dencripted = _3DES_Algorithm.Decrypt(key, CipherMode.ECB, encripted);
                string DecriptedMessage = ASCIIEncoding.ASCII.GetString(entitet.Dencripted);
                Console.WriteLine("Dekriptovana poruka od strane klijenta -> " + DecriptedMessage);

                DataForClient = File.ReadAllText("C:/Bezbednost/encripted.txt");
                DataFromServerToCLient();
            }
            else
            {
                string name = Thread.CurrentPrincipal.Identity.Name;
                DateTime time = DateTime.Now;
                string message = String.Format($"Access is denied. User [{Korisnik.ToUpper()}] tried to call Modify method (time: { time.TimeOfDay}). " +
                    "For this method user needs to be member of group Modify.");
                throw new FaultException<SecurityException>(new SecurityException(message));
            }
        }
        
        public void Read(string Korisnik, Entitet entitet, byte[] encripted)       
        {
            if (Manager.CertManager.GetGroup(StoreName.My, StoreLocation.LocalMachine, Korisnik).Contains("OU=read"))
            {
                Console.WriteLine("READ");
                string key = SecretKey.LoadKey("C:/Bezbednost/clientKey.txt");
                entitet.Dencripted = _3DES_Algorithm.Decrypt(key, CipherMode.ECB, encripted);
                string DecriptedMessage = ASCIIEncoding.ASCII.GetString(entitet.Dencripted);
                Console.WriteLine("Dekriptovana poruka od strane klijenta -> "+ DecriptedMessage);
                
                DataForClient = File.ReadAllText("C:/Bezbednost/encripted.txt");
                DataFromServerToCLient();
             
                
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

       
       

        public void Supervise(string Korisnik, Entitet entitet, byte[] encripted)
        {
            //Console.WriteLine("SUPERVISE");
            if (Manager.CertManager.GetGroup(StoreName.My, StoreLocation.LocalMachine, Korisnik).Contains("OU=supervise"))
            {
                Console.WriteLine("SUPERVISE");
                string key = SecretKey.LoadKey("C:/Bezbednost/clientKey.txt");
                entitet.Dencripted = _3DES_Algorithm.Decrypt(key, CipherMode.ECB, encripted);
                string DecriptedMessage = ASCIIEncoding.ASCII.GetString(entitet.Dencripted);
                Console.WriteLine("Dekriptovana poruka od strane klijenta -> " + DecriptedMessage);

                DataForClient = File.ReadAllText("C:/Bezbednost/encripted.txt");
                DataFromServerToCLient();
            }
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

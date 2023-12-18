using Client;
using Common;
using Manager;
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
            DataForClient = File.ReadAllText("C:/Bezbednost/Baza.txt");
            string message = MessageForSend(DataForClient);
          
            byte[] encriptedDataForClient = _3DES_Algorithm.Encrypt(key, System.Security.Cryptography.CipherMode.ECB, message);
            
            return encriptedDataForClient;
        }
      
        public void Modify(string id, string korisnik, byte[] sign)
        {
            
            if (Manager.CertManager.GetGroup(StoreName.My, StoreLocation.LocalMachine, korisnik).Contains("OU=modify"))
            {
              
                Console.WriteLine("MODIFY");
                string key = SecretKey.LoadKey("C:/Bezbednost/clientKey.txt");
                string clientNameSign = korisnik + "_sign";
                X509Certificate2 certificate = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
                StoreLocation.LocalMachine, clientNameSign);
                byte[] encmess = ASCIIEncoding.ASCII.GetBytes(id);
                string message = ASCIIEncoding.ASCII.GetString(encmess);
              //  entitet.Dencripted = _3DES_Algorithm.Decrypt(key, CipherMode.ECB, encripted);
               // string DecriptedMessage = ASCIIEncoding.ASCII.GetString(entitet.Dencripted);
              //  Console.WriteLine("Dekriptovana poruka od strane klijenta -> " + DecriptedMessage);
               


                //  DataFromServerToCLient();
                if (DigitalSignature.Verify(message, "SHA1", sign, certificate))
                {
                    Console.WriteLine($"{korisnik}'s sign is valid");
                    DataFromServerToCLient();
                    try
                    {

                        Audit.AuthorizationSuccess(korisnik,
                            OperationContext.Current.IncomingMessageHeaders.Action);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Sign is invalid");
                   
                }
            }
            else
            {
          
                DateTime time = DateTime.Now;
                try
                {
                    Audit.AuthorizationFailed(korisnik, OperationContext.Current.IncomingMessageHeaders.Action, "Modify");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + "modify");
                }


                string message = String.Format($"Access is denied. User [{korisnik.ToUpper()}] tried to call Modify method (time: { time.TimeOfDay}). " +
                    "For this method user needs to be member of group Modify.");
                throw new FaultException<SecurityException>(new SecurityException(message));
            }
        }
        
        public byte[] Read(byte[] id, string korisnik, byte[] sign)       
        {
            
            if (Manager.CertManager.GetGroup(StoreName.My, StoreLocation.LocalMachine, korisnik).Contains("OU=read"))
            {
                Console.WriteLine("READ");
                string retVal = "";
                byte[] EncryptedData = new byte[512];
                string clientNameSign = korisnik + "_sign";
                X509Certificate2 certificate = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
                StoreLocation.LocalMachine, clientNameSign);
                string key = SecretKey.LoadKey("C:/Bezbednost/clientKey.txt");
                
                byte[] dec = _3DES_Algorithm.Decrypt(key, CipherMode.ECB, id);
               
                string DecriptedMessage = ASCIIEncoding.ASCII.GetString(dec);
                Console.WriteLine("Dekriptovana poruka od strane klijenta -> " + DecriptedMessage);


                //    entitet.Dencripted = _3DES_Algorithm.Decrypt(key, CipherMode.ECB, encripted);
                //    string DecriptedMessage = ASCIIEncoding.ASCII.GetString(entitet.Dencripted);



                if (DigitalSignature.Verify(ASCIIEncoding.ASCII.GetString(id), "SHA1", sign, certificate))
                {
                    Console.WriteLine($"{korisnik}'s sign is valid");
                    Dictionary<int, Entitet> Entiteti = ReadFromDatabase();

                    if (Entiteti.ContainsKey(Int32.Parse(DecriptedMessage)))
                    {
                        retVal = Entiteti[Int32.Parse(DecriptedMessage)].ToString();
                        EncryptedData = _3DES_Algorithm.Encrypt(key, System.Security.Cryptography.CipherMode.ECB, retVal);
                    }
                    else
                    {
                        retVal = $"Ne postoji entitet sa ID : {DecriptedMessage}";
                        EncryptedData = _3DES_Algorithm.Encrypt(key, System.Security.Cryptography.CipherMode.ECB, retVal);
                    }
                        

                    try
                    {

                        Audit.AuthorizationSuccess(korisnik,
                            OperationContext.Current.IncomingMessageHeaders.Action);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Sign is invalid");
                   
                }

                return EncryptedData;
             
                
            }
            else
            {
               // string name = Manager.Formatter.ParseName(Thread.CurrentPrincipal.Identity.Name);
                DateTime time = DateTime.Now;
                try
                {
                    Audit.AuthorizationFailed(korisnik, OperationContext.Current.IncomingMessageHeaders.Action, "Read");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + "reader");
                }
                string message = String.Format($"Access is denied. User [{korisnik.ToUpper()}] tried to call Read method (time: {time.TimeOfDay}). " +
                    "For this method user needs to be member of group Read.");
                throw new FaultException<SecurityException>(new SecurityException(message));
              
            }
           
        }

       
       

        public void Supervise(string id, string korisnik, byte[] sign)
        {
            //Console.WriteLine("SUPERVISE");
            if (Manager.CertManager.GetGroup(StoreName.My, StoreLocation.LocalMachine, korisnik).Contains("OU=supervise"))
            {
                Console.WriteLine("SUPERVISE");
                string clientNameSign = korisnik + "_sign";
                X509Certificate2 certificate = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
                StoreLocation.LocalMachine, clientNameSign);
                string key = SecretKey.LoadKey("C:/Bezbednost/clientKey.txt");
                byte[] encmess = ASCIIEncoding.ASCII.GetBytes(id);
                string message = ASCIIEncoding.ASCII.GetString(encmess);
            //    entitet.Dencripted = _3DES_Algorithm.Decrypt(key, CipherMode.ECB, encripted);
             //   string DecriptedMessage = ASCIIEncoding.ASCII.GetString(entitet.Dencripted);
             //   Console.WriteLine("Dekriptovana poruka od strane klijenta -> " + DecriptedMessage);
               

                //      DataFromServerToCLient();
                if (DigitalSignature.Verify(message, "SHA1", sign, certificate))
                {
                    Console.WriteLine($"{korisnik}'s sign is valid");
                    DataFromServerToCLient();
                    try
                    {

                        Audit.AuthorizationSuccess(korisnik,
                            OperationContext.Current.IncomingMessageHeaders.Action);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                else
                {
                    Console.WriteLine("Sign is invalid");
                    
                }

            }
            else
            {
      
                DateTime time = DateTime.Now;
                try
                {
                    Audit.AuthorizationFailed(korisnik, OperationContext.Current.IncomingMessageHeaders.Action, "Supervise");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + "supervisor");
                }
                string message = String.Format($"Access is denied. User [{korisnik.ToUpper()}] tried to call Supervise method (time: {time.TimeOfDay}). "+"For this method user needs to be member of group Supervise.");
                throw new FaultException<SecurityException>(new SecurityException(message));
       
            }
        }
      
        public Dictionary<int, Entitet> ReadFromDatabase()
        {
            Dictionary<int, Entitet> entiteti = new Dictionary<int, Entitet>();
            string allEntities = File.ReadAllText("C:/Bezbednost/Baza.txt");
            string[] entitites = allEntities.Split(';');
            for (int i = 0; i < entitites.Length - 1; i++)
            {
                string[] entityParts = entitites[i].Split('|');
                int id = Int32.Parse(entityParts[0]);
                string name = entityParts[1];
                string SID = entityParts[2];
                DateTime time = DateTime.Parse(entityParts[3]);
                Entitet e = new Entitet { Id = id, Name = name, SID = SID, Time = time };
                entiteti.Add(e.Id, e);
            }
            return entiteti;
        }
        public void TestConnection(string Korisnik)
        {
            Console.WriteLine("Klijent {0} povezan na server.", Korisnik);
            try
            {
                Audit.AuthenticationSuccess(Korisnik);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public void DataFromServerToCLientDecripted()
        {
            throw new NotImplementedException();
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

    }
}

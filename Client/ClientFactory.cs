using Common;
using Manager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Threading;

namespace Client
{
    
    public class ClientFactory : ChannelFactory<IEntitet>, IEntitet, IDisposable
    {
        public static string ulogovanKorisnik = "";
      
        IEntitet factory;
        public ClientFactory(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            string cltCertCN =  Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            ulogovanKorisnik = cltCertCN;
            //CuvanjeUlogovanogClienta();
            this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.ChainTrust;
            this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);


            
            factory = this.CreateChannel();
           
        }

        public void Dispose()
        {
            if (factory != null)
            {
                factory = null;
            }

            this.Close();
        }
        public void TestConnection(string Korisnik)
        {
            try
            {
                factory.TestConnection(Korisnik);
                Console.WriteLine("Klijent {0} povezan na server...", ulogovanKorisnik);
            }
            catch (Exception e)
            {
                Console.WriteLine("Konekcija nije uspesna");
                Console.WriteLine("{0}", e.Message);
            }
        }
        public void Modify(string Korisnik, Entitet entitet)
        {
            try
            {
                
                factory.Modify(Korisnik, (entitet));
                Console.WriteLine();
                Console.WriteLine("Modify...");
            }
            catch (FaultException<SecurityException> e)
            {
                Console.WriteLine("Error while trying to Modify: {0}", e.Detail.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("[FAILED] ERROR = {0}", e.Message);
            }
        }
            public void Read(string Korisnik, Entitet entitet)
            {
            try
            {
                //string key = SecretKey.LoadKey("D:/Program Files (x86)/TEST/clientKey.txt");
                //string message = "TEST";
                //_3DES_Algorithm.Encrypt(key, CipherMode.ECB, message);
                // Console.WriteLine("{0}", _3DES_Algorithm.Encrypted);
                //factory.Read(_3DES_Algorithm.Encrypted);
         
                factory.Read(Korisnik, entitet);
                
          
                Console.WriteLine("Read...");
                

            }
            catch (FaultException<SecurityException> ex)
            {
                Console.WriteLine("Error while trying to Read: {0}", ex.Detail.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[FAILED] ERROR = {0}", ex.Message);
            }

        }

        public void Supervise(string Korisnik, Entitet entitet)
        {

            try
            {
                factory.Supervise(Korisnik, entitet);
                Console.WriteLine("Supervise...");
            }
            catch (FaultException<SecurityException> e)
            {
                Console.WriteLine("Error while trying to Supervise: {0}", e.Detail.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("[FAILED] ERROR = {0}", e.Message);
            }
        }

       
        
    }
}

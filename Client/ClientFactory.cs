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
        string ulogovanKorisnik = "";
        string putanja = "D:/Program Files (x86)/InformacionaBezbednost/Client/bin/Debug/cert.txt";
        IEntitet factory;
        public ClientFactory(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            string cltCertCN =  Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            ulogovanKorisnik = cltCertCN;
            CuvanjeUlogovanogClienta();
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
        public void TestConnection()
        {
            try
            {
                factory.TestConnection();
                Console.WriteLine("Klijent {0} povezan na server...", ulogovanKorisnik);
            }
            catch (Exception e)
            {
                Console.WriteLine("Konekcija nije uspesna");
                Console.WriteLine("{0}", e.Message);
            }
        }
        public void Modify()
        {
            try
            {
                factory.Modify();
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
            public void Read(byte [] sdf)
            {
            try
            {
                string key = SecretKey.LoadKey("D:/Program Files (x86)/InformacionaBezbednost/clientKey.txt");
                string message = "TEST";
                _3DES_Algorithm.Encrypt(key, CipherMode.ECB, message);
                Console.WriteLine("{0}", _3DES_Algorithm.Encrypted);
                factory.Read(_3DES_Algorithm.Encrypted);
                Console.WriteLine("Read...");

            }
            catch (FaultException<SecurityException> e)
            {
                Console.WriteLine("Error while trying to Read: {0}", e.Detail.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("[FAILED] ERROR = {0}", e.Message);
            }

        }

        public void Supervise()
        {

            try
            {
                factory.Supervise();
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

       
        public void CuvanjeUlogovanogClienta()
        {
            string text = ulogovanKorisnik;
            File.WriteAllText(putanja, text); 
        }
    }
}

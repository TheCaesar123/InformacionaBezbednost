using Common;
using Manager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading;

namespace Client
{

    public class ClientFactory : ChannelFactory<IEntitet>, IEntitet, IDisposable
    {
        public static string ulogovanKorisnik = "";
        public static byte[] poruka;

        IEntitet factory;
        public ClientFactory(NetTcpBinding binding, EndpointAddress address) : base(binding, address)
        {
            string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            //string cltCertCN = ulogovanKorisnik;
            ulogovanKorisnik = cltCertCN;

            this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.ChainTrust;
            this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            this.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new ClientCertValidator();

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
        public void Modify(string Korisnik, Entitet entitet, byte[] encripted, byte[] sign)
        {
            try
            {

                factory.Modify(Korisnik, (entitet), encripted, sign);

                Console.WriteLine("Modify...");
                poruka = factory.DataFromServerToCLient();
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
        public void Read(string Korisnik, Entitet entitet, byte[] encripted, byte[] sign)
        {
            try
            {

                factory.Read(Korisnik, entitet, encripted, sign);


                Console.WriteLine("Read...");


                poruka = factory.DataFromServerToCLient();

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

        public byte[] DataFromServerToCLient()
        {

            return null;
        }

        public void Supervise(string Korisnik, Entitet entitet, byte[] encripted, byte[] sign)
        {

            try
            {
                factory.Supervise(Korisnik, entitet, encripted, sign);
                Console.WriteLine("Supervise...");
                poruka = factory.DataFromServerToCLient();
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

        public void DataFromServerToCLientDecripted()
        {
            try
            {
                string key = SecretKey.LoadKey("C:/Bezbednost/clientKey.txt");
                string retVal = ASCIIEncoding.ASCII.GetString(_3DES_Algorithm.Decrypt(key, CipherMode.ECB, poruka));


                Console.WriteLine($"Dekriptovani podaci iz baze  za klijenta [{ulogovanKorisnik.ToUpper()}]->" + retVal);
            }
            catch (Exception e)
            {
                Console.WriteLine("[FAILED] ERROR = {0}", e.Message);
            }
        }
    }
}

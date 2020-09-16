using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Aspectize.Core;
using System.Security.Permissions;
using FP.Radius;
using System.Threading.Tasks;
using sc2i.common;

namespace TimosWebApp.Services
{
    public interface IAdministrationService
    {
        DataSet UploadFiles(UploadedFile[] uploadedFiles);
        void TestAppelServeurRadius(string strIP, string strSecret, string strUser, string strPassword);
    }

    [Service(Name = "AdministrationService")]
    public class AdministrationService : IAdministrationService //, IInitializable, ISingleton
    {

        public DataSet UploadFiles(UploadedFile[] uploadedFiles)
        {
            AspectizeUser aspectizeUser = ExecutingContext.CurrentUser;
            IEntityManager em = EntityManager.FromDataSet(DataSetHelper.Create());

            if (aspectizeUser.IsAuthenticated)
            {
                var fs = ExecutingContext.GetService<IFileService>("TimosFileService");

                foreach (UploadedFile file in uploadedFiles)
                {
                    fs.Write(file.Name, file.Stream);
                }
            }
            else
            {
                throw new SmartException(1100, "Votre session a expiré, veuillez vous reconnecter");
            }
            return em.Data;
        }

        public void TestAppelServeurRadius(string strHostName, string strSharedSecret, string strUserName, string strPassword)
        {
            try
            {
                AuthenticateRadius(strHostName, strSharedSecret, strUserName, strPassword).Wait();
            }
            catch (Exception e)
            {
                throw new SmartException(9900, "Erreur d'appel serveur Radius : " + e.Message);
            }
        }

        private async static Task AuthenticateRadius(string strHostName, string strSharedSecret, string strUserName, string strPassword)
        {
            string strCryptedPassword = C2iCrypto.Crypte(strPassword);

            RadiusClient rc = new RadiusClient(strHostName, strSharedSecret);
            RadiusPacket authPacket = rc.Authenticate(strUserName, strCryptedPassword);
            authPacket.SetAttribute(new VendorSpecificAttribute(10135, 1, UTF8Encoding.UTF8.GetBytes("Testing")));
            authPacket.SetAttribute(new VendorSpecificAttribute(10135, 2, new[] { (byte)7 }));
            RadiusPacket receivedPacket = await rc.SendAndReceivePacket(authPacket);
            if (receivedPacket == null)
                throw new SmartException(9901, "Can't contact remote radius server !");

            switch (receivedPacket.PacketType)
            {
                case RadiusCode.ACCESS_ACCEPT:
                    Console.WriteLine("Access-Accept");
                    foreach (var attr in receivedPacket.Attributes)
                        Console.WriteLine(attr.Type.ToString() + " = " + attr.Value);
                    break;
                case RadiusCode.ACCESS_CHALLENGE:
                    Console.WriteLine("Access-Challenge");
                    break;
                case RadiusCode.ACCESS_REJECT:
                    Console.WriteLine("Access-Reject");
                    if (!rc.VerifyAuthenticator(authPacket, receivedPacket))
                        Console.WriteLine("Authenticator check failed: Check your secret");
                    break;
                default:
                    Console.WriteLine("Rejected");
                    break;
            }

        }
    }
}

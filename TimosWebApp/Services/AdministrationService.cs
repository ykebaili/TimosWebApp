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
        string TestAppelServeurRadius(string strIP, string strSecret, string strUser, string strPassword);
        bool TestAppelServeur();
        string TestAppelServeurAvecParmatres(string alpha, string beta);

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

        public string TestAppelServeurRadius(string strHostName, string strSharedSecret, string strUserName, string strPassword)
        {
            try
            {
                return AuthenticateRadius(strHostName, 1815, strSharedSecret, strUserName, strPassword, "");
                
            }
            catch (Exception e)
            {
                throw new SmartException(9900, "Erreur d'appel serveur Radius : " + e.Message);
            }
        }

        public static string AuthenticateRadius(string strHostName, uint nPort, string strSharedSecret, string strUserName, string strPassword, string strStateAttribut)
        {
            //strStateAttribut = "30-34-30-61-33-66-39-34-2D-65-39-39-36-2D-34-32-38-62-2D-38-32-65-63-2D-30-63-64-32-63-32-64-66-36-35-31-31";
            //strStateAttribut = "040a3f94-e996-428b-82ec-0cd2c2df6511";

            RadiusClient rc = new RadiusClient(strHostName, strSharedSecret, authPort:nPort);
            RadiusPacket authPacket = rc.Authenticate(strUserName, strPassword);
            if (strStateAttribut != "")
            {
                //string buffer = String.Join("", strStateAttribut.Split('-'));
                byte[] data = Encoding.UTF8.GetBytes(strStateAttribut);

                authPacket.SetAttribute(new RadiusAttribute(RadiusAttributeType.STATE, data));
            }
            else
            {
                authPacket.SetAttribute(new VendorSpecificAttribute(10135, 1, UTF8Encoding.UTF8.GetBytes("Testing")));
                authPacket.SetAttribute(new VendorSpecificAttribute(10135, 2, new[] { (byte)7 }));
            }

            RadiusPacket receivedPacket = rc.SendAndReceivePacket(authPacket);
            if (receivedPacket == null)
                throw new SmartException(9901, "Can't contact remote radius server !");

            StringBuilder sbDebug = new StringBuilder();
            StringBuilder sbRetour = new StringBuilder();

            switch (receivedPacket.PacketType)
            {
                case RadiusCode.ACCESS_ACCEPT:
                    sbRetour.Append("2#");
                    sbDebug.AppendLine("Access-Accept");
                    foreach (var attr in receivedPacket.Attributes)
                    {
                        sbDebug.AppendLine(attr.Type.ToString() + " = " + attr.Value);
                    }
                    break;
                case RadiusCode.ACCESS_CHALLENGE:
                    sbRetour.Append("11#");
                    sbDebug.AppendLine("Access-Challenge");
                    foreach (var attr in receivedPacket.Attributes)
                    {
                        sbDebug.AppendLine(attr.Type.ToString() + " = " + attr.Value);
                        if (attr.Type == RadiusAttributeType.STATE)
                            sbRetour.Append(attr.Value);
                    }
                    break;
                case RadiusCode.ACCESS_REJECT:
                    sbRetour.Append("3#");
                    sbDebug.AppendLine("Access-Reject");
                    if (!rc.VerifyAuthenticator(authPacket, receivedPacket))
                        sbDebug.AppendLine("Authenticator check failed: Check your secret");
                    break;
                default:
                    sbRetour.Append("0#");
                    sbDebug.AppendLine("Rejected");
                    break;
            }

            //return sbDebug.ToString();
            return sbRetour.ToString();

        }


        //--------------------------------------------------------------------------------------------------
        // POUR DEBUG UNIQUEMENT
        public bool TestAppelServeur()
        {
            //throw new SmartException(9900, "Erreur dans test d'appel serveur");
            return true;
        }
        public string TestAppelServeurAvecParmatres(string alpha, string beta)
        {
            return "Paramètre alpha = " + alpha + Environment.NewLine + "Paramètre beta = " + beta;
        }

    }
}

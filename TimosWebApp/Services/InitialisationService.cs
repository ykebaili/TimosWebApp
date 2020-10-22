using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Aspectize.Core;
using System.Security.Permissions;
using sc2i.common;
using sc2i.multitiers.client;
using System.Security.Principal;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Lifetime;

namespace TimosWebApp.Services
{
    public interface IInitialisationService
    {
        void InitTimos();
    }

    [Service(Name = "InitialisationService", ConfigurationRequired = true)]
    public class InitialisationService : IInitialisationService, ISingleton //, IInitializable, 
    {

        [Parameter(Optional = false)]
        public string TimosServerURL = "";

        /*[Parameter(Optional = false)]
        public string RadiusServerURL = "";
        [Parameter(Optional = false)]
        public uint RadiusServerPort = 1815;
        [Parameter(Optional = false)]
        public string RadiusSharedKey = "";*/

        public void InitTimos()
        {
            CResultAErreur result = CResultAErreur.True;
            string strServeurUrl = TimosServerURL;
            int nTcpChannel = 0;
            string strBindTo = "";

            try
            {
                C2iEventLog.Init("TIMOS WEB", "Timos Web App", NiveauBavardage.VraiPiplette);

                AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);

                result = CSC2iMultitiersClient.Init(nTcpChannel, strServeurUrl, strBindTo);

                LifetimeServices.LeaseTime = new TimeSpan(0, 5, 0);
                LifetimeServices.LeaseManagerPollTime = new TimeSpan(0, 5, 0);
                LifetimeServices.SponsorshipTimeout = new TimeSpan(0, 3, 0);
                LifetimeServices.RenewOnCallTime = new TimeSpan(0, 8, 0);

                C2iSponsor.EnableSecurite();

                /*TcpChannel channel = new TcpChannel();
                ChannelServices.RegisterChannel(channel, false);*/
                               

                if (!result)
                    result.EmpileErreur("Erreur lors de l'initialisation");

                C2iEventLog.WriteInfo("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            }
            catch (Exception e)
            {
                result.EmpileErreur(e.Message);
            }
            
        }
    }

}

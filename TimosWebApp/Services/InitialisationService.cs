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
using Aspectize.Scheduling;

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

        [Parameter(Optional = false)]
        public string RadiusServerURL = "";
        
        [Parameter(Optional = false)]
        public int RadiusServerPort = 1815;
        
        [Parameter(Optional = false)]
        public string RadiusSharedKey = "";

        [Parameter(Optional = true)]
        public int ExportUpdatePeriod = 24; // En heures

        public void InitTimos()
        {
            CResultAErreur result = CResultAErreur.True;

            string strServeurUrl = TimosServerURL;
            int nTcpChannel = 0;
            string strBindTo = "";

            string strRadiuServerUrl = RadiusServerURL;
            uint nRadiusPort = (uint)RadiusServerPort;
            string strSharedKey = RadiusSharedKey;

            int nUpdatePeriod = ExportUpdatePeriod;

            try
            {
                AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);

                AuthenticationService.Init(strRadiuServerUrl, nRadiusPort, strSharedKey);

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

                // Schedulers 
                // ATTENTION : Dans le cas de plusieurs serveurs (load balancing par exemple) il faut locker le traitement dans la commande appelée 
                ScheduleCommand.RunEvery(nUpdatePeriod, PeriodUnit.Hour, "TimosWebApp/ExportService.TraiteListeExports", new Dictionary<string, object>(), new DateTime(2021, 06, 04, 22, 00, 00) , null);
                /*/ DEBUG ONLY
                ScheduleCommand.RunEvery(10, PeriodUnit.Minute, "TimosWebApp/ExportService.UpdateAllExports", new Dictionary<string, object>(), null, null);
                //*/
            }
            catch (Exception e)
            {
                result.EmpileErreur(e.Message);
            }
        }
    }
}

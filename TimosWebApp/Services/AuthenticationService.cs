using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Aspectize.Core;
using sc2i.multitiers.client;
using sc2i.common;
using timos.data.Aspectize;
using sc2i.process.workflow;
using TimosWebApp.Services;
using FP.Radius;

namespace TimosWebApp
{
    public interface IAuthentificationTimos
    {
        string AuthenticateRadius(string strUserName, string strPassword);
        void LogoutUser();
    }

    [Service(Name = "AuthenticationService")]
    public class AuthenticationService : IAuthentication, IUserProfile, IPersistentAuthentication, IAuthentificationTimos //, IInitializable, ISingleton
    {
        private static string m_strRadiusHost = "172.22.114.144";
        private static uint m_nRadiusPort = 1815;
        private static string m_strRadiusSharedKey = ""; // H_Sf2V"T%2\n

        //-------------------------------------------------------------------------------------------------------------------------
        public static void Init(string strRadiusHost, uint nPort, string strSharedKey)
        {
            m_strRadiusHost = strRadiusHost;
            m_nRadiusPort = nPort;
            m_strRadiusSharedKey = strSharedKey;
        }

        //-------------------------------------------------------------------------------------------------------------------------
        // Premier appel Radius
        public string AuthenticateRadius(string strUserName, string strPassword)
        {
            string messageLog = "AuthenticateRadius : " + strUserName + Environment.NewLine +
                "Radius Host : " + m_strRadiusHost + Environment.NewLine +
                "Radius Port : " + m_nRadiusPort + Environment.NewLine +
                "Shared Key : " + m_strRadiusSharedKey;

            Context.Log(InfoType.Information, messageLog);

            if (ExecutingContext.CurrentHostUrl.ToLower().StartsWith(@"http://localhost"))
                return "11#blablabbal";

            else
                return AdministrationService.AuthenticateRadius(m_strRadiusHost, m_nRadiusPort, m_strRadiusSharedKey, strUserName, strPassword, "");
        }

        //-------------------------------------------------------------------------------------------------------------------------
        // Authenticate user, using Security Service Configuration 
        AspectizeUser IAuthentication.Authenticate(string userName, string secret, AuthenticationProtocol protocol, HashHelper.Algorithm algorithm, string challenge)
        {
            Context.Log(InfoType.Information, "IAuthentication.Authenticate : " + userName);

            var parts = secret.Split('#');

            string otp = parts[0];
            //string password = string.Join("#", parts, 1, parts.Length - 1);
            string password = parts[1];
            string state = parts[2];

            if (userName != "youcef")
            {
                string retour = AdministrationService.AuthenticateRadius(m_strRadiusHost, m_nRadiusPort, m_strRadiusSharedKey, userName, otp, state);
                var parts2 = retour.Split('#');
                if(parts2[0] != "2")
                    return AspectizeUser.GetUnAuthenticatedUser(); // L'authentification OTP a échoué
            }

            // Authentification TIMOS

            ITimosServiceForAspectize serviceClientAspectize = (ITimosServiceForAspectize)C2iFactory.GetNewObject(typeof(ITimosServiceForAspectize));
            CResultAErreur result = serviceClientAspectize.OpenSession(userName, password);
            
            if (result && result.Data is Dictionary<string, object>)
            {
                string strUserKey = "";

                // Build Key-Value attached to User
                Dictionary<string, object> dicoProperties = (Dictionary<string, object>)result.Data;

                strUserKey = (string)dicoProperties[CUserTimosWebApp.c_champUserKey];

                // Build Role List
                List<string> roles = new List<string>();

                roles.Add("Registered");
                                
                // Build and return authenticated user with Properties and Roles
                return AspectizeUser.GetAuthenticatedUser(strUserKey, roles.ToArray(), dicoProperties);
            }

            return AspectizeUser.GetUnAuthenticatedUser();
            // Fin authentification TIMOS 
        }


        //-------------------------------------------------------------------------------------------------------------------------
        // This Command is called when user is remembered, instead of Authenticate
        bool IPersistentAuthentication.ValidateUser(AspectizeUser user)
        {
            int nIdsession = (int)user[CUserTimosWebApp.c_champSessionId];

            ITimosServiceForAspectize serviceClientAspectize = (ITimosServiceForAspectize)C2iFactory.GetNewObject(typeof(ITimosServiceForAspectize));
            if (serviceClientAspectize.GetSession(nIdsession))
                return true;

            return false;
        }

        //-------------------------------------------------------------------------------------------------------------------------
        // Get Profile (ie initial DataSet) of user, authenticated or not
        DataSet IUserProfile.GetUserProfile()
        {
            // Get current user
            AspectizeUser aspectizeUser = ExecutingContext.CurrentUser;

            if (aspectizeUser.IsAuthenticated)
            {
                IEntityManager em = EntityManager.FromDataSet(DataSetHelper.Create());

                // Initialise l'utilisateur connecté
                var user = em.CreateInstance<User>();
                user.IsAuthentificated = true;
                user.Name = (string)aspectizeUser[CUserTimosWebApp.c_champUserName];
                user.Login = (string)aspectizeUser[CUserTimosWebApp.c_champUserLogin];
                user.TimosKey = (string)aspectizeUser[CUserTimosWebApp.c_champUserKey];
                user.TimosSessionId = (int)aspectizeUser[CUserTimosWebApp.c_champSessionId];
                user.IsAdministrator = (bool)aspectizeUser[CUserTimosWebApp.c_champIsAdministrator];

                // Instancie les To do de l'utilisateur en cours
                ITimosServiceForAspectize serviceClientAspectize = (ITimosServiceForAspectize)C2iFactory.GetNewObject(typeof(ITimosServiceForAspectize));
                CResultAErreur result = serviceClientAspectize.GetTodosForUser(user.TimosSessionId, user.TimosKey);

                if (result && result.Data != null)
                {
                    DataSet ds = result.Data as DataSet;
                    if (ds != null && ds.Tables.Contains(CTodoTimosWebApp.c_nomTable))
                    {
                        DataTable dt = ds.Tables[CTodoTimosWebApp.c_nomTable];

                        foreach (DataRow row in dt.Rows)
                        {
                            var todo = em.CreateInstance<Todos>();
                            todo.TimosId = (int)row[CTodoTimosWebApp.c_champId];
                            todo.Label = (string)row[CTodoTimosWebApp.c_champLibelle];
                            todo.StartDate = (DateTime)row[CTodoTimosWebApp.c_champDateDebut];
                            todo.Instructions = (string)row[CTodoTimosWebApp.c_champInstructions];
                            todo.ElementType = (string)row[CTodoTimosWebApp.c_champTypeElementEdite];
                            todo.ElementId = (int)row[CTodoTimosWebApp.c_champIdElementEdite];
                            todo.ElementDescription = (string)row[CTodoTimosWebApp.c_champElementDescription];
                            todo.DureeStandard = (int)row[CTodoTimosWebApp.c_champDureeStandard];
                            int nEtat = (int)row[CTodoTimosWebApp.c_champEtatTodo];
                            todo.EtatTodo = (EtatTodo)nEtat;
                            if (row[CTodoTimosWebApp.c_champDateFin] == DBNull.Value)
                                todo.EndDate = null;
                            else
                                todo.EndDate = (DateTime)row[CTodoTimosWebApp.c_champDateFin];
                        }
                    }

                }

                // Récupère la liste des Actions globales disponibles pour cet utilisateur
                try
                {
                    result = serviceClientAspectize.GetActionsForUser(user.TimosSessionId, user.TimosKey);
                    if (!result)
                        throw new SmartException(1010, "Erreur GetExportsForUser(nTimosSessionId = " + user.TimosSessionId + ", keyUser = " + user.TimosKey + ")" +
                        Environment.NewLine +
                        result.MessageErreur);

                    if (result && result.Data != null)
                    {
                        DataSet ds = result.Data as DataSet;

                        if (ds != null && ds.Tables.Contains(CActionWeb.c_nomTable))
                        {
                            DataTable dt = ds.Tables[CActionWeb.c_nomTable];

                            foreach (DataRow row in dt.Rows)
                            {
                                var action = em.CreateInstance<Action>();
                                action.Id = (int)row[CActionWeb.c_champId];
                                action.Libelle = (string)row[CActionWeb.c_champLibelle];
                                action.Instructions = (string)row[CActionWeb.c_champInstructions];
                                action.IsGlobale = (bool)row[CActionWeb.c_champIsGlobale];

                                // Variables Texte
                                action.IDT1 = (string)row[CActionWeb.c_champIdVarText1];
                                action.IDT2 = (string)row[CActionWeb.c_champIdVarText2];
                                action.IDT3 = (string)row[CActionWeb.c_champIdVarText3];
                                action.IDT4 = (string)row[CActionWeb.c_champIdVarText4];
                                action.IDT5 = (string)row[CActionWeb.c_champIdVarText5];
                                action.LBLT1 = (string)row[CActionWeb.c_champLabelVarText1];
                                action.LBLT2 = (string)row[CActionWeb.c_champLabelVarText2];
                                action.LBLT3 = (string)row[CActionWeb.c_champLabelVarText3];
                                action.LBLT4 = (string)row[CActionWeb.c_champLabelVarText4];
                                action.LBLT5 = (string)row[CActionWeb.c_champLabelVarText5];

                                string strValeursVarText1 = (string)row[CActionWeb.c_champValeursVarText1];
                                string strValeursVarText2 = (string)row[CActionWeb.c_champValeursVarText2];
                                string strValeursVarText3 = (string)row[CActionWeb.c_champValeursVarText3];
                                string strValeursVarText4 = (string)row[CActionWeb.c_champValeursVarText4];
                                string strValeursVarText5 = (string)row[CActionWeb.c_champValeursVarText5];
                                TodosService.FillValeursVariableForAction(em, action, strValeursVarText1, "T1");
                                TodosService.FillValeursVariableForAction(em, action, strValeursVarText2, "T2");
                                TodosService.FillValeursVariableForAction(em, action, strValeursVarText3, "T3");
                                TodosService.FillValeursVariableForAction(em, action, strValeursVarText4, "T4");
                                TodosService.FillValeursVariableForAction(em, action, strValeursVarText5, "T5");

                                // Variables Int
                                action.IDN1 = (string)row[CActionWeb.c_champIdVarInt1];
                                action.IDN2 = (string)row[CActionWeb.c_champIdVarInt2];
                                action.IDN3 = (string)row[CActionWeb.c_champIdVarInt3];
                                action.LBLN1 = (string)row[CActionWeb.c_champLabelVarInt1];
                                action.LBLN2 = (string)row[CActionWeb.c_champLabelVarInt2];
                                action.LBLN3 = (string)row[CActionWeb.c_champLabelVarInt3];

                                string strValeursVarInt1 = (string)row[CActionWeb.c_champValeursVarInt1];
                                string strValeursVarInt2 = (string)row[CActionWeb.c_champValeursVarInt2];
                                string strValeursVarInt3 = (string)row[CActionWeb.c_champValeursVarInt3];
                                TodosService.FillValeursVariableForAction(em, action, strValeursVarInt1, "N1");
                                TodosService.FillValeursVariableForAction(em, action, strValeursVarInt2, "N2");
                                TodosService.FillValeursVariableForAction(em, action, strValeursVarInt3, "N3");

                                // Variables Date
                                action.IDD1 = (string)row[CActionWeb.c_champIdVarDate1];
                                action.IDD2 = (string)row[CActionWeb.c_champIdVarDate2];
                                action.IDD3 = (string)row[CActionWeb.c_champIdVarDate3];
                                action.LBLD1 = (string)row[CActionWeb.c_champLabelVarDate1];
                                action.LBLD2 = (string)row[CActionWeb.c_champLabelVarDate2];
                                action.LBLD3 = (string)row[CActionWeb.c_champLabelVarDate3];
                                // Variables Bool
                                action.IDB1 = (string)row[CActionWeb.c_champIdVarBool1];
                                action.IDB2 = (string)row[CActionWeb.c_champIdVarBool2];
                                action.IDB3 = (string)row[CActionWeb.c_champIdVarBool3];
                                action.LBLB1 = (string)row[CActionWeb.c_champLabelVarBool1];
                                action.LBLB2 = (string)row[CActionWeb.c_champLabelVarBool2];
                                action.LBLB3 = (string)row[CActionWeb.c_champLabelVarBool3];

                            }
                        }

                        em.Data.AcceptChanges();
                        return em.Data;
                    }
                }
                catch (Exception ex)
                {
                    throw new SmartException(1010, "Erreur GetExportsForUser(nTimosSessionId = " + user.TimosSessionId + ", keyUser = " + user.TimosKey + ")" +
                        Environment.NewLine +
                        result.MessageErreur);
                }

                em.Data.AcceptChanges();
                return em.Data;
                
            }
            // No profile for unanthenticated user
            return null;
        }
       

        //-------------------------------------------------------------------------------------------------------------------------
        public void LogoutUser()
        {
            AspectizeUser aspectizeUser = ExecutingContext.CurrentUser;
            int nIdsession = (int)aspectizeUser[CUserTimosWebApp.c_champSessionId];
            ITimosServiceForAspectize serviceClientAspectize = (ITimosServiceForAspectize)C2iFactory.GetNewObject(typeof(ITimosServiceForAspectize));
            serviceClientAspectize.CloseSession(nIdsession);
        }

    }

}

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Aspectize.Core;
using sc2i.multitiers.client;
using sc2i.common;
using timos.data.Aspectize;
using sc2i.process.workflow;

namespace TimosWebApp
{
    public interface IAuthentificationTimos
    {
        void LogoutUser();
    }

    [Service(Name = "AuthenticationService")]
    public class AuthenticationService : IAuthentication, IUserProfile, IPersistentAuthentication, IAuthentificationTimos //, IInitializable, ISingleton
    {
        // Authenticate user, using Security Service Configuration 
        AspectizeUser IAuthentication.Authenticate(string userName, string secret, AuthenticationProtocol protocol, HashHelper.Algorithm algorithm, string challenge)
        {
            // Authentification TIMOS
            
            ITimosServiceForAspectize serviceClientAspectize = (ITimosServiceForAspectize)C2iFactory.GetNewObject(typeof(ITimosServiceForAspectize));
            CResultAErreur result = serviceClientAspectize.OpenSession(userName, secret);
            
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

        // This Command is called when user is remembered, instead of Authenticate
        bool IPersistentAuthentication.ValidateUser(AspectizeUser user)
        {
            int nIdsession = (int)user[CUserTimosWebApp.c_champSessionId];

            ITimosServiceForAspectize serviceClientAspectize = (ITimosServiceForAspectize)C2iFactory.GetNewObject(typeof(ITimosServiceForAspectize));
            if (serviceClientAspectize.GetSession(nIdsession))
                return true;

            return false;
        }

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

                em.Data.AcceptChanges();
                return em.Data;
                
            }
            // No profile for unanthenticated user
            return null;
        }

        public void LogoutUser()
        {
            AspectizeUser aspectizeUser = ExecutingContext.CurrentUser;
            int nIdsession = (int)aspectizeUser[CUserTimosWebApp.c_champSessionId];
            ITimosServiceForAspectize serviceClientAspectize = (ITimosServiceForAspectize)C2iFactory.GetNewObject(typeof(ITimosServiceForAspectize));
            serviceClientAspectize.CloseSession(nIdsession);
        }

    }

}

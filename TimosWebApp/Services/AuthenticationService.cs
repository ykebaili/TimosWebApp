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
    [Service(Name = "AuthenticationService")]
    public class AuthenticationService : IAuthentication, IUserProfile, IPersistentAuthentication //, IInitializable, ISingleton
    {
        // Authenticate user, using Security Service Configuration 
        AspectizeUser IAuthentication.Authenticate(string userName, string secret, AuthenticationProtocol protocol, HashHelper.Algorithm algorithm, string challenge)
        {
            // Authentification TIMOS
            ITimosServiceForAspectize serviceClientAspectize = (ITimosServiceForAspectize)C2iFactory.GetNewObject(typeof(ITimosServiceForAspectize));
            //string strNomGestionnaire = serviceClientAspectize.GetType().ToString();
            CResultAErreur result = serviceClientAspectize.OpenSession(userName, secret);
            
            if (result)
            {
                string strUserKey = "";

                // Build Key-Value attached to User
                Dictionary<string, object> dicoProperties = result.Data as Dictionary<string, object>;
                if (dicoProperties == null)
                    dicoProperties = new Dictionary<string, object>();

                strUserKey = (string)dicoProperties[CUtilTimosUser.c_champUserKey];

                // Build Role List
                List<string> roles = new List<string>();

                roles.Add("Registered");
                                
                // Build and return authenticated user with Properties and Roles
                return AspectizeUser.GetAuthenticatedUser(strUserKey, roles.ToArray(), dicoProperties);
            }

            return AspectizeUser.GetUnAuthenticatedUser();
            // Fin authentification TIMOS 

            /*
            IDataManager dm = EntityManager.FromDataBaseService(ServiceName.DataService);

            // Get All Users with the email requested
            List<User> users = dm.GetEntities<User>(new QueryCriteria(User.Fields.Email, ComparisonOperator.Equal, userName.ToLower().Trim()));

            if (users.Count > 0)
            {
                User user = users[0];

                // Check password using Security Service Configuration
                bool match = PasswordHasher.CheckResponse(user.PassWord, challenge, algorithm, secret);

                if (match && user.Status != EnumUserStatus.Blocked)
                {
                    // Build Key-Value attached to User
                    Dictionary<string, object> dicoProperties = new Dictionary<string, object>();

                    dicoProperties.Add("Email", user.Email);

                    // Build Role List
                    List<string> roles = new List<string>();

                    roles.Add("Registered");

                    // Save Last Login
                    user.DateLastLogin = DateTime.Now;

                    dm.SaveTransactional();

                    // Build and return authenticated user with Properties and Roles
                    return AspectizeUser.GetAuthenticatedUser(user.Id.ToString("N"), roles.ToArray(), dicoProperties);
                }
            }

            

            return AspectizeUser.GetUnAuthenticatedUser();*/
        }

        // This Command is called when user is remembered, instead of Authenticate
        bool IPersistentAuthentication.ValidateUser(AspectizeUser user)
        {

            return false;

            /*/// Connect to Data Storage
            IDataManager dm = EntityManager.FromDataBaseService(ServiceName.DataService);

            // retreive user with current user Id
            User appliUser = dm.GetEntity<User>(new Guid(user.UserId));

            if (appliUser != null)
            {
                // Check user status
                if (appliUser.Status != EnumUserStatus.Blocked)
                {
                    // Attach Key-Value (same as in Authenticate Command)
                    user["Email"] = appliUser.Email;

                    // Save Last Login
                    appliUser.DateLastLogin = DateTime.Now;

                    dm.SaveTransactional();

                    return true;
                }
            }
            return false;*/
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
                user.Name = (string)aspectizeUser[CUtilTimosUser.c_champUserName];
                user.Login = (string)aspectizeUser[CUtilTimosUser.c_champUserLogin];
                user.TimosKey = (string)aspectizeUser[CUtilTimosUser.c_champUserKey];
                user.TimosSessionId = (int)aspectizeUser[CUtilTimosUser.c_champSessionId];


                // Instancie les To do de l'utilisateur en cours
                ITimosServiceForAspectize serviceClientAspectize = (ITimosServiceForAspectize)C2iFactory.GetNewObject(typeof(ITimosServiceForAspectize));
                CResultAErreur result = serviceClientAspectize.GetTodosForUser(user.TimosSessionId, user.TimosKey);

                if(result && result.Data != null)
                {
                    DataSet ds = result.Data as DataSet;
                    if(ds != null && ds.Tables.Contains(CUtilTimosTodos.c_nomTable))
                    {
                        DataTable dt = ds.Tables[CUtilTimosTodos.c_nomTable];

                        foreach (DataRow row in dt.Rows)
                        {
                            var todos = em.CreateInstance<Todos>();
                            todos.TimosId = (int)row[CUtilTimosTodos.c_champId];
                            todos.Label = (string)row[CUtilTimosTodos.c_champLibelle];
                            todos.StartDate = (DateTime)row[CUtilTimosTodos.c_champDateDebut];
                            todos.Instructions = (string)row[CUtilTimosTodos.c_champInstructions];
                            todos.ElementType = (string)row[CUtilTimosTodos.c_champTypeElementEdite];
                            todos.ElementId = (int)row[CUtilTimosTodos.c_champIdElementEdite];
                            todos.ElementDescription = (string)row[CUtilTimosTodos.c_champElementDescription];
                        }
                    }

                }

                

 
                em.Data.AcceptChanges();
                return em.Data;

                /*
                // Connect to Data Storage
                IDataManager dm = EntityManager.FromDataBaseService(ServiceName.DataService);

                // retreive user with current user Id
                User user = dm.GetEntity<User>(new Guid(aspectizeUser.UserId));

                if (user != null)
                {
                    // Use IEntityManager to manage Entities in memory
                    IEntityManager em = dm as IEntityManager;

                    // Build non-persistent Entity to bind CurrentUser
                    CurrentUser currentUser = em.CreateInstance<CurrentUser>();

                    // Associate 2 Entities
                    em.AssociateInstance<IsUser>(currentUser, user);

                    // Always return non changed DataSet
                    em.Data.AcceptChanges();

                    return dm.Data;
                }*/
            }

            // No profile for unanthenticated user
            return null;
        }

    }

}

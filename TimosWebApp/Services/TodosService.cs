using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Aspectize.Core;
using System.Security.Permissions;
using timos.data.Aspectize;
using sc2i.common;
using sc2i.multitiers.client;
using System.IO;
using System.Linq;
using System.Collections;

namespace TimosWebApp.Services
{
    public interface ITodosService
    {
        DataSet GetTodoDetails(int nIdTodo);
        [Command(IsSaveCommand = true)]
        void SaveTodo(DataSet dataSet, int nIdTodo, string elementType, int elementId);
        DataSet EndTodo(int nIdTodo);
        void DeleteCaracteristique(int nIdCarac, string strTypeElement);
        [Command(IsSaveCommand = true)]
        DataSet SaveCaracteristique(DataSet dataSet, int nIdCarac, string strTypeElement, int nIdTodo);
        DataSet UploadDocuments(UploadedFile[] uploadedFiles, int nIdTodo, int nIdDocument, string strLibelle, int nIdCategorie);
        void DeleteDocument(string strKeyFile);
        byte[] DownloadDocument(string strKeyFile, string strFileName);
        [Command(IsSaveCommand = true)]
        string ExecuteAction(DataSet dataSet, int nIdAction, string elementType, int elementId);
        Dictionary<string, object>[] GetDatasList(string term, string strChampId);
    }

    [Service(Name = "TodosService")]
    public class TodosService : ITodosService //, IInitializable, ISingleton
    {
        public DataSet GetTodoDetails(int nIdTodo)
        {
            AspectizeUser aspectizeUser = ExecutingContext.CurrentUser;
            IEntityManager em = EntityManager.FromDataSet(DataSetHelper.Create());

            if (aspectizeUser.IsAuthenticated)
            {
                int nTimosSessionId = (int)aspectizeUser[CUserTimosWebApp.c_champSessionId];

                ITimosServiceForAspectize serviceClientAspectize = (ITimosServiceForAspectize)C2iFactory.GetNewObject(typeof(ITimosServiceForAspectize));
                CResultAErreur result = serviceClientAspectize.GetSession(nTimosSessionId);
                if (!result)
                {
                    throw new SmartException(1100, "Votre session a expiré, veuillez vous reconnecter");
                }

                try
                {
                    result = serviceClientAspectize.GetTodoDetails(nTimosSessionId, nIdTodo);
                    if (!result)
                        throw new SmartException(1010, "Erreur GetTodoDetails(nTimosSessionId = " + nTimosSessionId + ", nIdTodo = " + nIdTodo + ")" +
                        Environment.NewLine +
                        result.MessageErreur);

                    if (result && result.Data != null)
                    {
                        DataSet ds = result.Data as DataSet;
                        FillEntitiesFromDataSet(ds, em);

                        em.Data.AcceptChanges();
                        return em.Data;
                    }
                }
                catch (Exception ex)   
                {
                    throw new SmartException(1010,
                        "Erreur GetTodoDetails(nTimosSessionId = " + nTimosSessionId + ", nIdTodo = " + nIdTodo + ")" +
                        Environment.NewLine +
                        ex.Message);
                }
            }
            else
            {
                throw new SmartException(1100, "Votre session a expiré, veuillez vous reconnecter");
            }
            return null;
        }

           
        //-----------------------------------------------------------------------------------------
        public void SaveTodo(DataSet dataSet, int nIdTodo, string elementType, int elementId)
        {
            if (!dataSet.HasChanges())
                return;

            AspectizeUser aspectizeUser = ExecutingContext.CurrentUser;

            if (aspectizeUser.IsAuthenticated)
            {
                if (dataSet.HasChanges())
                {
                    int nTimosSessionId = (int)aspectizeUser[CUserTimosWebApp.c_champSessionId];
                    IEntityManager em = EntityManager.FromDataSet(dataSet);

                    ITimosServiceForAspectize serviceClientAspectize = (ITimosServiceForAspectize)C2iFactory.GetNewObject(typeof(ITimosServiceForAspectize));
                    CResultAErreur result = serviceClientAspectize.GetSession(nTimosSessionId);
                    if (!result)
                    {
                        throw new SmartException(1100, "Votre session a expiré, veuillez vous reconnecter");
                    }
                    try
                    {
                        // Pré-traitement des champs AutoComplete
                        result = TraiteAutoCompleteValues(dataSet, "TodoValeurChamp");
                        result = serviceClientAspectize.SaveTodo(nTimosSessionId, dataSet, nIdTodo, elementType, elementId);
                        if (!result)
                            throw new SmartException(1010, result.MessageErreur);
                    }
                    catch (Exception ex)
                    {
                        throw new SmartException(1010, ex.Message);
                    }
                }
            }
            else
            {
                throw new SmartException(1100, "Votre session a expiré, veuillez vous reconnecter");
            }

        }

        //-----------------------------------------------------------------------------------------
        private CResultAErreur TraiteAutoCompleteValues(DataSet ds, string strNomTable)
        {
            CResultAErreur result = CResultAErreur.True;
            if(ds != null)
            {
                DataTable tableATraiter = ds.Tables[strNomTable];
                if(tableATraiter != null)
                {
                    foreach(DataRow row in tableATraiter.Rows)
                    {
                        string strChampId = row["ChampTimosId"].ToString();
                        string strValeur = (string)row["ValeurChamp"];
                        if (row.RowState == DataRowState.Modified || row.RowState == DataRowState.Added)
                        {
                            Dictionary<string, string> dicValeursChamp = m_htChampValeursPossibles[strChampId] as Dictionary<string, string>;
                            if (dicValeursChamp != null)
                            {
                                try
                                {
                                    string strKey = dicValeursChamp.Where(p => p.Value == strValeur).First().Key;
                                    row["ValeurChamp"] = strKey;
                                }
                                catch(Exception ex)
                                {
                                    result.EmpileErreur(ex.Message);
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }

        //-----------------------------------------------------------------------------------------
        public DataSet EndTodo(int nIdTodo)
        {
            AspectizeUser aspectizeUser = ExecutingContext.CurrentUser;
            IEntityManager em = EntityManager.FromDataSet(DataSetHelper.Create());

            if (aspectizeUser.IsAuthenticated)
            {
                int nTimosSessionId = (int)aspectizeUser[CUserTimosWebApp.c_champSessionId];

                ITimosServiceForAspectize serviceClientAspectize = (ITimosServiceForAspectize)C2iFactory.GetNewObject(typeof(ITimosServiceForAspectize));
                CResultAErreur result = serviceClientAspectize.GetSession(nTimosSessionId);
                if (!result)
                {
                    throw new SmartException(1100, "Votre session a expiré, veuillez vous reconnecter");
                }
                result = serviceClientAspectize.EndTodo(nTimosSessionId, nIdTodo);
                if (!result)
                    throw new SmartException(1020, result.MessageErreur);

                DataSet dsRetour = result.Data as DataSet;
                FillEntitiesFromDataSet(dsRetour, em);
                em.Data.AcceptChanges();
                return em.Data;
            }
            else
            {
                throw new SmartException(1100, "Votre session a expiré, veuillez vous reconnecter");
            }
        }
        
        //------------------------------------------------------------------------------------------------------------
        public DataSet SaveCaracteristique(DataSet dataSet, int nIdCarac, string strTypeElement, int nIdTodo)
        {
            if (!dataSet.HasChanges())
                return dataSet;

            AspectizeUser aspectizeUser = ExecutingContext.CurrentUser;
            IEntityManager em = EntityManager.FromDataSet(dataSet);

            if (aspectizeUser.IsAuthenticated)
            {

                int nTimosSessionId = (int)aspectizeUser[CUserTimosWebApp.c_champSessionId];

                ITimosServiceForAspectize serviceClientAspectize = (ITimosServiceForAspectize)C2iFactory.GetNewObject(typeof(ITimosServiceForAspectize));
                CResultAErreur result = serviceClientAspectize.GetSession(nTimosSessionId);
                if (!result)
                {
                    throw new SmartException(1100, "Votre session a expiré, veuillez vous reconnecter");
                }
                try
                {
                    DataSet dsClone = dataSet.Copy();
                    result = TraiteAutoCompleteValues(dsClone, "CaracValeurChamp");
                    result = serviceClientAspectize.SaveCaracteristique(nTimosSessionId, dsClone, nIdCarac, strTypeElement, nIdTodo);
                    if (!result)
                        throw new SmartException(1010, result.MessageErreur);
                }
                catch (Exception ex)
                {
                    throw new SmartException(1010, ex.Message);
                }
                DataSet dsRetour = result.Data as DataSet;
                FillEntitiesFromDataSet(dsRetour, em);
                em.Data.AcceptChanges();
                return em.Data;
            }
            else
            {
                throw new SmartException(1100, "Votre session a expiré, veuillez vous reconnecter");
            }

            dataSet.AcceptChanges();
            return dataSet;
        }

        //------------------------------------------------------------------------------------------------------------
        public void DeleteCaracteristique(int nIdCarac, string strTypeElement)
        {
            AspectizeUser aspectizeUser = ExecutingContext.CurrentUser;

            if (aspectizeUser.IsAuthenticated)
            {
                int nTimosSessionId = (int)aspectizeUser[CUserTimosWebApp.c_champSessionId];
                ITimosServiceForAspectize serviceClientAspectize = (ITimosServiceForAspectize)C2iFactory.GetNewObject(typeof(ITimosServiceForAspectize));
                CResultAErreur result = serviceClientAspectize.GetSession(nTimosSessionId);
                if (!result)
                {
                    throw new SmartException(1100, "Votre session a expiré, veuillez vous reconnecter");
                }
                result = serviceClientAspectize.DeleteCaracteristique(nTimosSessionId, nIdCarac, strTypeElement);
                if (!result)
                    throw new SmartException(1010, result.MessageErreur);
            }
            else
            {
                throw new SmartException(1100, "Votre session a expiré, veuillez vous reconnecter");
            }
        }

        //------------------------------------------------------------------------------------------------------------
        public DataSet UploadDocuments(UploadedFile[] uploadedFiles, int nIdTodo, int nIdDocument, string strLibelle, int nIdCategorie)
        {
            AspectizeUser aspectizeUser = ExecutingContext.CurrentUser;
            IEntityManager em = EntityManager.FromDataSet(DataSetHelper.Create());

            if (aspectizeUser.IsAuthenticated)
            {
                int nTimosSessionId = (int)aspectizeUser[CUserTimosWebApp.c_champSessionId];
                ITimosServiceForAspectize serviceClientAspectize = (ITimosServiceForAspectize)C2iFactory.GetNewObject(typeof(ITimosServiceForAspectize));
                CResultAErreur result = serviceClientAspectize.GetSession(nTimosSessionId);
                if (!result)
                {
                    throw new SmartException(1100, "Votre session a expiré, veuillez vous reconnecter");
                }
                DocumentsAttendus doc = em.CreateInstance<DocumentsAttendus>();

                doc.TimosId = nIdDocument;
                doc.Libelle = strLibelle;
                doc.IdCategorie = nIdCategorie;
                doc.DateLastUpload = DateTime.Now;

                /** DEBUG ** var fs = ExecutingContext.GetService<IFileService>("FichierLocalTemporaire"); */

                if (doc != null)
                {
                    foreach (UploadedFile file in uploadedFiles)
                    {
                        var fichier = em.CreateInstance<FichiersAttaches>();
                        fichier.NomFichier = file.Name;
                        fichier.TimosKey = Guid.NewGuid().ToString();
                        fichier.DateUpload = DateTime.Now;
                        int nIndex = fichier.NomFichier.LastIndexOf(".");
                        string strExtension = "";
                        if (nIndex > 0)
                            strExtension = fichier.NomFichier.Substring(nIndex + 1);
                        else
                            strExtension = "bin";
                        fichier.Extension = strExtension.ToLower();

                        em.AssociateInstance<RelationFichiers>(doc, fichier);

                        BinaryReader reader = new BinaryReader(file.Stream);
                        try
                        {
                            byte[] octets = reader.ReadBytes((int)file.Stream.Length);

                            CResultAErreur resultFile = serviceClientAspectize.AddFile(nTimosSessionId, file.Name, octets);
                            if (!resultFile)
                            {
                                throw new SmartException(1030, resultFile.MessageErreur);
                            }
                            string cheminTimos = (string)resultFile.Data;
                            fichier.CheminTemporaire = cheminTimos;

                        }
                        catch (Exception ex)
                        {
                            throw new SmartException(1031, ex.Message);
                        }
                        finally
                        {
                            reader.Close();
                        }
                    }
                }

                em.Data.AcceptChanges();
                result = serviceClientAspectize.SaveDocument(nTimosSessionId, em.Data, nIdDocument, nIdCategorie);
                if (!result)
                {
                    throw new SmartException(1031, result.MessageErreur);
                }

            }
            else
            {
                throw new SmartException(1100, "Votre session a expiré, veuillez vous reconnecter");
            }
            return em.Data;
        }

        //-----------------------------------------------------------------------------------------
        public void DeleteDocument(string strKeyFile)
        {
            AspectizeUser aspectizeUser = ExecutingContext.CurrentUser;
            IEntityManager em = EntityManager.FromDataSet(DataSetHelper.Create());

            if (aspectizeUser.IsAuthenticated)
            {
                int nTimosSessionId = (int)aspectizeUser[CUserTimosWebApp.c_champSessionId];
                ITimosServiceForAspectize serviceClientAspectize = (ITimosServiceForAspectize)C2iFactory.GetNewObject(typeof(ITimosServiceForAspectize));
                CResultAErreur result = serviceClientAspectize.GetSession(nTimosSessionId);
                if (!result)
                {
                    throw new SmartException(1100, "Votre session a expiré, veuillez vous reconnecter");
                }
                result = serviceClientAspectize.DeleteFile(nTimosSessionId, strKeyFile);
                if(!result)
                {
                    throw new SmartException(1060, result.MessageErreur);
                }

            }
            else
            {
                throw new SmartException(1100, "Votre session a expiré, veuillez vous reconnecter");
            }
        }

        //-----------------------------------------------------------------------------------------
        public byte[] DownloadDocument(string strKeyFile, string strFileName)
        {
            AspectizeUser aspectizeUser = ExecutingContext.CurrentUser;
            IEntityManager em = EntityManager.FromDataSet(DataSetHelper.Create());

            if (aspectizeUser.IsAuthenticated) // Attention problème potentiel sur iOS avec les cookies
            {
                int nTimosSessionId = (int)aspectizeUser[CUserTimosWebApp.c_champSessionId];
                ITimosServiceForAspectize serviceClientAspectize = (ITimosServiceForAspectize)C2iFactory.GetNewObject(typeof(ITimosServiceForAspectize));
                CResultAErreur result = serviceClientAspectize.GetSession(nTimosSessionId);
                if (!result)
                {
                    throw new SmartException(1100, "Votre session a expiré, veuillez vous reconnecter");
                }
                result = serviceClientAspectize.DownloadFile(nTimosSessionId, strKeyFile);
                if (!result)
                {
                    throw new SmartException(1060, result.MessageErreur);
                }
                else
                {
                    byte[] octets = result.Data as byte[];
                    if(octets != null)
                    {
                        ExecutingContext.SetHttpDownloadFileName(strFileName);
                        return octets;
                    }
                }
            }
            else
            {
                throw new SmartException(1100, "Votre session a expiré, veuillez vous reconnecter");
            }

            return null;
        }

        //-----------------------------------------------------------------------------------------
        // Retourne le message de succes de l'execution de l'action Timos
        public string ExecuteAction(DataSet dataSet, int nIdAction, string elementType, int elementId)
        {
            if (!dataSet.HasChanges())
                return "";

            AspectizeUser aspectizeUser = ExecutingContext.CurrentUser;

            if (aspectizeUser.IsAuthenticated)
            {
                if (dataSet.HasChanges())
                {
                    int nTimosSessionId = (int)aspectizeUser[CUserTimosWebApp.c_champSessionId];
                    IEntityManager em = EntityManager.FromDataSet(dataSet);

                    ITimosServiceForAspectize serviceClientAspectize = (ITimosServiceForAspectize)C2iFactory.GetNewObject(typeof(ITimosServiceForAspectize));
                    CResultAErreur result = serviceClientAspectize.GetSession(nTimosSessionId);
                    if (!result)
                    {
                        throw new SmartException(1100, "Votre session a expiré, veuillez vous reconnecter");
                    }
                    result = serviceClientAspectize.ExecuteAction(nTimosSessionId, dataSet, nIdAction, elementType, elementId);
                    if (!result)
                        throw new SmartException(1010, result.MessageErreur);

                    if (result.Data != null)
                        return result.Data.ToString();
                }
            }
            else
            {
                throw new SmartException(1100, "Votre session a expiré, veuillez vous reconnecter");
            }
            return "default";
        }


        //-----------------------------------------------------------------------------------------
        private void FillEntitiesFromDataSet(DataSet ds, IEntityManager em)
        {
            if (ds == null || em == null)
                return;


            // Création des Todos
            if (ds.Tables.Contains(CTodoTimosWebApp.c_nomTable))
            {
                DataTable tableTodos = ds.Tables[CTodoTimosWebApp.c_nomTable];
                if (tableTodos.Rows.Count > 0)
                {
                    // la première row contient les données du todo demandé
                    DataRow rowTodo = tableTodos.Rows[0];
                    int nIdTodo = (int)rowTodo[CTodoTimosWebApp.c_champId];
                    var todo = em.GetInstance<Todos>(nIdTodo);
                    if (todo == null)
                        todo = em.CreateInstance<Todos>();
                    todo.TimosId = nIdTodo;
                    todo.Label = (string)rowTodo[CTodoTimosWebApp.c_champLibelle];
                    todo.StartDate = (DateTime)rowTodo[CTodoTimosWebApp.c_champDateDebut];
                    todo.Instructions = (string)rowTodo[CTodoTimosWebApp.c_champInstructions];
                    todo.ElementType = (string)rowTodo[CTodoTimosWebApp.c_champTypeElementEdite];
                    todo.ElementId = (int)rowTodo[CTodoTimosWebApp.c_champIdElementEdite];
                    todo.ElementDescription = (string)rowTodo[CTodoTimosWebApp.c_champElementDescription];
                    todo.DureeStandard = (int)rowTodo[CTodoTimosWebApp.c_champDureeStandard];
                    int nEtat = (int)rowTodo[CTodoTimosWebApp.c_champEtatTodo];
                    todo.EtatTodo = (EtatTodo)nEtat;
                    if (rowTodo[CTodoTimosWebApp.c_champDateFin] == DBNull.Value)
                        todo.EndDate = null;
                    else
                        todo.EndDate = (DateTime)rowTodo[CTodoTimosWebApp.c_champDateFin];

                    // Création des groupes de champs
                    if (ds.Tables.Contains(CGroupeChamps.c_nomTable))
                    {
                        DataTable tableGroupes = ds.Tables[CGroupeChamps.c_nomTable];
                        bool bExpand = true;
                        foreach (DataRow rowGroupe in tableGroupes.Rows)
                        {
                            string strTitre = (string)rowGroupe[CGroupeChamps.c_champTitre];
                            if (strTitre.ToUpper().Contains("DOCUMENT"))
                                continue;

                            int nIdGroupe = (int)rowGroupe[CGroupeChamps.c_champId];
                            if (em.GetInstance<GroupeChamps>(nIdGroupe) == null)
                            {
                                var groupeChamps = em.CreateInstance<GroupeChamps>();
                                groupeChamps.TimosId = nIdGroupe;
                                groupeChamps.Titre = strTitre;
                                groupeChamps.OrdreAffichage = (int)rowGroupe[CGroupeChamps.c_champOrdreAffichage];
                                groupeChamps.InfosSecondaires = (bool)rowGroupe[CGroupeChamps.c_champIsInfosSecondaires];
                                groupeChamps.Expand = bExpand;
                                groupeChamps.CanAddCaracteristiques = (bool)rowGroupe[CGroupeChamps.c_champCanAddCaracteristiques];
                                groupeChamps.TitreCaracteristiques = (string)rowGroupe[CGroupeChamps.c_champTitreCaracteristiques];
                                bExpand = false;

                                try
                                {
                                    em.AssociateInstance<RelationTodoGroupeChamps>(todo, groupeChamps);
                                }
                                catch (Exception ex)
                                {
                                    Context.Log(ex, InfoType.Error, "Erreur d'association Todos <-> GroupeChamps" + Environment.NewLine + ex.Message);
                                }

                            }
                        }
                    }

                    // Création des caractéristiques
                    if (ds.Tables.Contains(CCaracteristique.c_nomTable))
                    {
                        DataTable tableCaracteristiques = ds.Tables[CCaracteristique.c_nomTable];
                        foreach (DataRow rowCarac in tableCaracteristiques.Rows)
                        {
                            int nIdElement = (int)rowCarac[CCaracteristique.c_champTimosId]; // Id de l'element Timos (Caractristique, Site, Dossier...)
                            string strTypeElement = (string)rowCarac[CCaracteristique.c_champElementType]; // Type de l'élément Timos
                            string strTitre = (string)rowCarac[CCaracteristique.c_champTitre];
                            int nIdGroupe = (int)rowCarac[CCaracteristique.c_champIdGroupeChamps];
                            string strIdCarac = (string)rowCarac[CCaracteristique.c_champId];

                            if (em.GetInstance<Caracteristiques>(strIdCarac) == null)
                            {
                                var caracteristique = em.CreateInstance<Caracteristiques>();
                                caracteristique.Id = strIdCarac;
                                caracteristique.TimosId = nIdElement;
                                caracteristique.ElementType = strTypeElement;
                                caracteristique.Titre = strTitre;
                                caracteristique.OrdreAffichage = (int)rowCarac[CCaracteristique.c_champOrdreAffichage];
                                caracteristique.IdGroupePourFiltre = nIdGroupe;
                                caracteristique.IsTemplate = (bool)rowCarac[CCaracteristique.c_champIsTemplate];
                                caracteristique.IdMetaType = (int)rowCarac[CCaracteristique.c_champIdMetaType];
                                caracteristique.ParentElementType = (string)rowCarac[CCaracteristique.c_champParentElementType];
                                caracteristique.ParentElementId = (int)rowCarac[CCaracteristique.c_champParentElementId];
                                var groupeChamps = em.GetInstance<GroupeChamps>(nIdGroupe);
                                if (groupeChamps != null)
                                    caracteristique.CanDeleteCaracteristique = groupeChamps.CanAddCaracteristiques;

                                try
                                {
                                    em.AssociateInstance<RelationTodoCaracteristique>(todo, caracteristique);
                                }
                                catch (Exception ex)
                                {
                                    Context.Log(ex, InfoType.Error, "Erreur d'association Todos <-> Caracteristiques" + Environment.NewLine + ex.Message);
                                }

                            }
                        }
                    }

                    // Gestion des documents attendus sur todo
                    if (ds.Tables.Contains(CDocumentAttendu.c_nomTable))
                    {
                        DataTable tableDocuementsAttendus = ds.Tables[CDocumentAttendu.c_nomTable];
                        foreach (DataRow rowDoc in tableDocuementsAttendus.Rows)
                        {
                            var doc = em.CreateInstance<DocumentsAttendus>();

                            doc.TimosId = (int)rowDoc[CDocumentAttendu.c_champId];
                            doc.Libelle = (string)rowDoc[CDocumentAttendu.c_champLibelle];
                            doc.IdCategorie = (int)rowDoc[CDocumentAttendu.c_champIdCategorie];
                            doc.NombreMin = (int)rowDoc[CDocumentAttendu.c_champNombreMin];
                            if (rowDoc[CDocumentAttendu.c_champDateLastUpload] == DBNull.Value)
                                doc.DateLastUpload = null;
                            else
                                doc.DateLastUpload = (DateTime)rowDoc[CDocumentAttendu.c_champDateLastUpload];

                            try
                            {
                                em.AssociateInstance<RelationTodoDocument>(todo, doc);
                            }
                            catch (Exception ex)
                            {
                                Context.Log(ex, InfoType.Error, "Erreur d'association Todos <-> DocumentsAttendus" + Environment.NewLine + ex.Message);
                            }

                            // Traitement des fichiers joints
                            if (ds.Tables.Contains(CFichierAttache.c_nomTable))
                            {
                                DataTable tableFichiers = ds.Tables[CFichierAttache.c_nomTable];
                                foreach (DataRow rowFichier in tableFichiers.Rows)
                                {
                                    int nIdDoc = (int)rowFichier[CFichierAttache.c_champIdDocumentAttendu];
                                    if (nIdDoc == doc.TimosId)
                                    {
                                        var fichier = em.CreateInstance<FichiersAttaches>();
                                        fichier.TimosKey = (string)rowFichier[CFichierAttache.c_champKey];
                                        fichier.NomFichier = (string)rowFichier[CFichierAttache.c_champNomFichier];
                                        fichier.Commentaire = (string)rowFichier[CFichierAttache.c_champCommentaire];
                                        fichier.DateUpload = (DateTime)rowFichier[CFichierAttache.c_champDateUpload];
                                        fichier.DateDocument = (DateTime)rowFichier[CFichierAttache.c_champDateDocument];
                                        fichier.DocumentId = nIdDoc;
                                        int nIndex = fichier.NomFichier.LastIndexOf(".");
                                        string strExtension = "";
                                        if (nIndex > 0)
                                            strExtension = fichier.NomFichier.Substring(nIndex + 1);
                                        else
                                            strExtension = "bin";
                                        fichier.Extension = strExtension.ToLower(); ;

                                        try
                                        {
                                            em.AssociateInstance<RelationFichiers>(doc, fichier);
                                        }
                                        catch (Exception ex)
                                        {
                                            Context.Log(ex, InfoType.Error, "Erreur d'association DocumentsAttendus <-> FichiersAttaches" + Environment.NewLine + ex.Message);
                                        }

                                    }
                                }
                            }
                        }
                    }
                    // Traitement des Actions
                    if(ds.Tables.Contains(CActionWeb.c_nomTable))
                    {
                        DataTable tableActions = ds.Tables[CActionWeb.c_nomTable];
                        foreach (DataRow row in tableActions.Rows)
                        {
                            int nIdAction = (int)row[CActionWeb.c_champId];
                            if (em.GetInstance<Action>(nIdAction) == null)
                            {
                                Action action = em.CreateInstance<Action>();
                                action.Id = nIdAction;
                                action.Libelle = (string)row[CActionWeb.c_champLibelle];
                                action.Instructions = (string)row[CActionWeb.c_champInstructions];
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
                                FillValeursVariableForAction(em, action, strValeursVarText1, "T1");
                                FillValeursVariableForAction(em, action, strValeursVarText2, "T2");
                                FillValeursVariableForAction(em, action, strValeursVarText3, "T3");
                                FillValeursVariableForAction(em, action, strValeursVarText4, "T4");
                                FillValeursVariableForAction(em, action, strValeursVarText5, "T5");

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
                                FillValeursVariableForAction(em, action, strValeursVarInt1, "N1");
                                FillValeursVariableForAction(em, action, strValeursVarInt2, "N2");
                                FillValeursVariableForAction(em, action, strValeursVarInt3, "N3");

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

                                try
                                {
                                    em.AssociateInstance<RelationTodoActions>(todo, action);
                                }
                                catch (Exception ex)
                                {
                                    Context.Log(ex, InfoType.Error, "Erreur d'association Todos <-> Action" + Environment.NewLine + ex.Message);
                                }
                            }
                        }

                    }
                }
            }


            // Définition des champs
            if (ds.Tables.Contains(CChampTimosWebApp.c_nomTable))
            {
                DataTable tableChampsTimos = ds.Tables[CChampTimosWebApp.c_nomTable];
                foreach (DataRow rowChamp in tableChampsTimos.Rows)
                {
                    int nIdChamp = (int)rowChamp[CChampTimosWebApp.c_champId];
                    var champTimos = em.GetInstance<ChampTimos>(nIdChamp);
                    if (champTimos == null)
                        champTimos = em.CreateInstance<ChampTimos>();
                    champTimos.TimosId = nIdChamp;
                    champTimos.Nom = (string)rowChamp[CChampTimosWebApp.c_champNom];
                    champTimos.DisplayOrder = (int)rowChamp[CChampTimosWebApp.c_champOrdreAffichage];
                    champTimos.TypeDonneChamp = (TypeDonnee)rowChamp[CChampTimosWebApp.c_champTypeDonne];
                    champTimos.LibelleConvivial = (string)rowChamp[CChampTimosWebApp.c_champLibelleConvivial];
                    champTimos.Editable = (bool)rowChamp[CChampTimosWebApp.c_champIsEditable];
                    champTimos.CustomClass = (string)rowChamp[CChampTimosWebApp.c_champCustomClass];

                    bool bUseAutoComplete = (bool)rowChamp[CChampTimosWebApp.c_champUseAutoComplete];
                    bool bIsSelect = (bool)rowChamp[CChampTimosWebApp.c_champIsChoixParmis];
                    bool bMultiline = (bool)rowChamp[CChampTimosWebApp.c_champIsMultiline];
                    int nIdGroupeAssocie = (int)rowChamp[CChampTimosWebApp.c_champIdGroupeChamps];
                    string strIdCaracAssociee = (string)rowChamp[CChampTimosWebApp.c_champIdCaracteristique];

                    champTimos.UseAutoComplete = bUseAutoComplete;
                    if (bIsSelect && !bUseAutoComplete)
                    {
                        champTimos.IsSelect = true;
                        champTimos.AspectizeFieldType = champTimos.TimosId.ToString();
                    }
                    else
                    {
                        switch (champTimos.TypeDonneChamp)
                        {
                            case TypeDonnee.TypeEntier:
                                champTimos.AspectizeFieldType = "String";
                                champTimos.AspectizeControlType = "Html.Number";
                                break;
                            case TypeDonnee.TypeDecimal:
                                champTimos.AspectizeFieldType = "String";
                                champTimos.AspectizeControlType = "";
                                break;
                            case TypeDonnee.TypeString:
                                champTimos.AspectizeFieldType = "String";
                                champTimos.AspectizeControlType = bMultiline ? "Html.MultilineTextBox" : "";
                                break;
                            case TypeDonnee.TypeDate:
                                champTimos.AspectizeFieldType = "Date";
                                champTimos.AspectizeControlType = "MonControleDate";
                                champTimos.FormatDate = "dd/MM/yyyy";
                                break;
                            case TypeDonnee.TypeBool:
                                champTimos.AspectizeFieldType = "Boolean";
                                champTimos.AspectizeControlType = "";
                                break;
                            case TypeDonnee.ObjetTimos:
                                champTimos.AspectizeFieldType = "String";
                                champTimos.AspectizeControlType = "";
                                break;
                            default:
                                break;
                        }

                    }


                    // Le champ appartient à un Groupe ou une Caracteristique
                    try
                    {
                        GroupeChamps groupeAssocie = em.GetInstance<GroupeChamps>(nIdGroupeAssocie);
                        if (groupeAssocie != null && champTimos.GetAssociatedInstance<GroupeChamps, RelationGroupeChampsChampsTimos>() != groupeAssocie)
                        {
                            em.AssociateInstance<RelationGroupeChampsChampsTimos>(groupeAssocie, champTimos);
                            if (champTimos.UseAutoComplete)
                            {
                                groupeAssocie.LibelleChampAutoComplete = champTimos.LibelleConvivial;
                                groupeAssocie.IdChampAutoComplete = champTimos.TimosId;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Context.Log(ex, InfoType.Error, "Erreur d'association GroupeChamps <-> ChampTimos" + Environment.NewLine + ex.Message);
                    }
                    try
                    {
                        Caracteristiques caracAssociee = em.GetInstance<Caracteristiques>(strIdCaracAssociee);
                        if (caracAssociee != null && champTimos.GetAssociatedInstance<Caracteristiques, RelationCaracChamp>() != caracAssociee)
                        {
                            em.AssociateInstance<RelationCaracChamp>(caracAssociee, champTimos);
                            if (champTimos.UseAutoComplete)
                            {
                                caracAssociee.LibelleChampAutoComplete = champTimos.LibelleConvivial;
                                caracAssociee.IdChampAutoComplete = champTimos.TimosId;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Context.Log(ex, InfoType.Error, "Erreur d'association Caracteristiques <-> ChampTimos" + Environment.NewLine + ex.Message);
                    }
                    
                }
            }
            
            // Gestion des valeurs possibles de champs
            if (ds.Tables.Contains(CChampValeursPossibles.c_nomTable))
            {
                int nIlfautPrendreaumoins2valeursPossibles = 2;
                DataTable tableValeursPossibles = ds.Tables[CChampValeursPossibles.c_nomTable];
                foreach (DataRow rowValPossbile in tableValeursPossibles.Rows)
                {
                    string strChampTimosId = rowValPossbile[CChampValeursPossibles.c_champId].ToString();
                    int nIndex = (int)rowValPossbile[CChampValeursPossibles.c_champIndex];
                    string strStoredValue = (string)rowValPossbile[CChampValeursPossibles.c_champValue];
                    string strDisplayeddValue = (string)rowValPossbile[CChampValeursPossibles.c_champDisplay];
                    int nIdGroupeAssocie = (int)rowValPossbile[CChampValeursPossibles.c_champIdGroupe];
                    string strIdCaracAssociee = (string)rowValPossbile[CChampValeursPossibles.c_champIdCaracteristique];

                    ChampTimos champ = em.GetInstance<ChampTimos>(strChampTimosId);
                    if (champ != null)
                    {
                        bool bAutoComplete = champ.UseAutoComplete;
                        if (bAutoComplete)
                        {
                            // Remplissage du dictionnaire AUTO COMPLETE
                            Dictionary<string, string> dicValeursChamp = m_htChampValeursPossibles[strChampTimosId] as Dictionary<string, string>;
                            if (dicValeursChamp == null)
                            {
                                dicValeursChamp = new Dictionary<string, string>();
                                m_htChampValeursPossibles[strChampTimosId] = dicValeursChamp;
                            }
                            dicValeursChamp[strStoredValue] = strDisplayeddValue;
                        }
                        if(!bAutoComplete)
                        {
                            string strId = strChampTimosId + "-" + strStoredValue;
                            ValeursChamp valPossible = em.GetInstance<ValeursChamp>(strId);
                            if (valPossible == null)
                            {
                                valPossible = em.CreateInstance<ValeursChamp>();
                                valPossible.Id = strId;
                                valPossible.ChampTimosId = strChampTimosId;
                                valPossible.Index = nIndex;
                                valPossible.StoredValue = strStoredValue;
                                valPossible.DisplayedValue = strDisplayeddValue;
                            }
                            /*/
                            ChampTimos champDejaAssocie = valPossible.GetAssociatedInstance<ChampTimos, RelationChampValeursPossibles>();
                            if(champDejaAssocie == null || champDejaAssocie.TimosId != champ.TimosId)
                                em.AssociateInstance<RelationChampValeursPossibles>(champ, valPossible);
                            //*/
                            //*/ Assoication de la cvaleur possible au Groupe et/ou Caracteristique
                            try
                            {
                                GroupeChamps groupeAssocie = em.GetInstance<GroupeChamps>(nIdGroupeAssocie);
                                if (groupeAssocie != null)
                                    em.AssociateInstance<ValeursPossibles>(groupeAssocie, valPossible);
                            }
                            catch (Exception ex)
                            {
                                Context.Log(ex, InfoType.Error, "Erreur d'association GroupeChamps <-> ValeursChamp" + Environment.NewLine + ex.Message);
                            }
                            try
                            {
                                Caracteristiques caracAssociee = em.GetInstance<Caracteristiques>(strIdCaracAssociee);
                                if (caracAssociee != null)
                                    em.AssociateInstance<RelationCaracValeursPossibles>(caracAssociee, valPossible);
                            }
                            catch (Exception ex)
                            {
                                Context.Log(ex, InfoType.Error, "Erreur d'association Caracteristiques <-> ValeursChamps" + Environment.NewLine + ex.Message);
                            }
                            //*/
                        }
                    }
                }
            }

            // Récupère les valeurs de champs associées au Todo par groupe de champs
            if (ds.Tables.Contains(CTodoValeurChamp.c_nomTable))
            {
                DataTable tableTodoValeursChamps = ds.Tables[CTodoValeurChamp.c_nomTable];
                foreach (DataRow rowVal in tableTodoValeursChamps.Rows)
                {
                    int nIdGroupeAssocie = (int)rowVal[CTodoValeurChamp.c_champIdGroupeChamps];
                    int nIdChampTimosAssocie = (int)rowVal[CTodoValeurChamp.c_champId];
                    string strIdCompose = nIdGroupeAssocie.ToString() + nIdChampTimosAssocie.ToString();

                    var valChampTimos = em.GetInstance<TodoValeurChamp>(strIdCompose);
                    if (valChampTimos == null)
                    {
                        valChampTimos = em.CreateInstance<TodoValeurChamp>();
                        valChampTimos.Id = strIdCompose;
                    }
                    valChampTimos.ChampTimosId = nIdChampTimosAssocie;
                    valChampTimos.LibelleChamp = (string)rowVal[CTodoValeurChamp.c_champLibelle];
                    valChampTimos.OrdreChamp = (int)rowVal[CTodoValeurChamp.c_champOrdreAffichage];
                    valChampTimos.ElementType = (string)rowVal[CTodoValeurChamp.c_champElementType];
                    valChampTimos.ElementId = (int)rowVal[CTodoValeurChamp.c_champElementId];
                    valChampTimos.UseAutoComplete = (bool)rowVal[CTodoValeurChamp.c_champUseAutoComplete];

                    string strValeurChamp = "";
                    object valeur = rowVal[CTodoValeurChamp.c_champValeur];
                    if (valeur != DBNull.Value)
                        strValeurChamp = valeur.ToString();

                    valChampTimos.ValeurChamp = strValeurChamp;
                    
                    ChampTimos champ = em.GetInstance<ChampTimos>(valChampTimos.ChampTimosId);
                    if (champ != null)
                    {
                        try
                        {
                            GroupeChamps groupeAssocie = em.GetInstance<GroupeChamps>(nIdGroupeAssocie);
                            if (groupeAssocie != null)
                                em.AssociateInstance<RelationTodoValeurChamp>(groupeAssocie, valChampTimos);
                        }
                        catch (Exception ex)
                        {
                            Context.Log(ex, InfoType.Error, "Erreur d'association GroupeChamps <-> TodoValeurChamp" + Environment.NewLine + ex.Message);
                        }
                    }
                }
            }

            // Récupère les valeurs de champs associées aux Caractéristiques
            if (ds.Tables.Contains(CCaracValeurChamp.c_nomTable))
            {
                DataTable tableCaracValeursChamps = ds.Tables[CCaracValeurChamp.c_nomTable];
                foreach (DataRow rowVal in tableCaracValeursChamps.Rows)
                {
                    string valeurChamp = (string)rowVal[CCaracValeurChamp.c_champValeur];
                    string strIdCaracAssociee = (string)rowVal[CCaracValeurChamp.c_champIdCaracteristique];
                    int nIdChampTimosAssocie = (int)rowVal[CCaracValeurChamp.c_champId];

                    string strIdCompose = strIdCaracAssociee + nIdChampTimosAssocie.ToString();
                    CaracValeurChamp valChampTimos = em.GetInstance<CaracValeurChamp>(strIdCompose);
                    if (valChampTimos == null)
                    {
                        valChampTimos = em.CreateInstance<CaracValeurChamp>();
                        valChampTimos.Id = strIdCompose;
                    }
                    valChampTimos.ChampTimosId = nIdChampTimosAssocie;
                    valChampTimos.LibelleChamp = (string)rowVal[CCaracValeurChamp.c_champLibelle];
                    valChampTimos.OrdreChamp = (int)rowVal[CCaracValeurChamp.c_champOrdreAffichage];
                    valChampTimos.ElementType = (string)rowVal[CCaracValeurChamp.c_champElementType];
                    valChampTimos.ElementId = (int)rowVal[CCaracValeurChamp.c_champElementId];
                    valChampTimos.UseAutoComplete = (bool)rowVal[CCaracValeurChamp.c_champUseAutoComplete];
                    valChampTimos.ValeurChamp = valeurChamp;

                    ChampTimos champ = em.GetInstance<ChampTimos>(nIdChampTimosAssocie);

                    if (champ != null)
                    {
                        try
                        { 
                        Caracteristiques caracAssociee = em.GetInstance<Caracteristiques>(strIdCaracAssociee);
                        if (caracAssociee != null)
                        {
                            em.AssociateInstance<RelationCaracValeurChamp>(caracAssociee, valChampTimos);
                        }
                        }
                        catch (Exception ex)
                        {
                            Context.Log(ex, InfoType.Error, "Erreur d'association Caracteristiques <-> CaracValeurChamp" + Environment.NewLine + ex.Message);
                        }

                    }
                }
            }
        }

        //--------------------------------------------------------------------------------------------------
        private void FillValeursVariableForAction(IEntityManager em, Action action, string strvaleurs, string strIdVariable)
        {
            if (em == null)
                return;
            if (strvaleurs != "")
            {
                string[] parts = strvaleurs.Split('#');
                for (int i = 0; i < parts.Length -1; i = i + 2)
                {
                    string strValue = parts[i];
                    string strDisplay = parts[i + 1];
                    var valeurVariable = em.CreateInstance<ValeursVariable>();
                    valeurVariable.Id = action.Id + strValue;
                    valeurVariable.Value = strValue;
                    valeurVariable.Display = strDisplay;
                    valeurVariable.IdVariable = strIdVariable;
                    em.AssociateInstance<RelationActionValeursVariable>(action, valeurVariable);
                }
            }
        }

        //--------------------------------------------------------------------------------------------------
        private static Hashtable m_htChampValeursPossibles = new Hashtable();
        public Dictionary<string, object>[] GetDatasList(string term, string strChampId)
        {
            var listeResult = new List<Dictionary<string, object>>();

            Dictionary<string, string> dicValeursChamp = m_htChampValeursPossibles[strChampId] as Dictionary<string, string>;
            if (dicValeursChamp != null)
            {
                List<KeyValuePair<string, string>> listeFiltre = dicValeursChamp.Where(val => val.Value.ToUpper().Contains(term.ToUpper())).Take(10).ToList();

                foreach(KeyValuePair<string, string> valeurChamp in listeFiltre)
                {
                    listeResult.Add(new Dictionary<string, object>() { { "label", valeurChamp.Value }, { "value", valeurChamp.Key } });
                }

                /*
                result.Add(new Dictionary<string, object>() { { "label", "toto" }, { "value", 1 } });
                result.Add(new Dictionary<string, object>() { { "label", "tata" }, { "value", 2 } });
                result.Add(new Dictionary<string, object>() { { "label", "tutu" }, { "value", 3 } });
                */
            }

            return listeResult.ToArray();
        }
    }

}

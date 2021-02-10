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
        bool TestAppelServeur();
        string TestAppelServeurAvecParmatres(string alpha, string beta);


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
                    throw new SmartException(1100, "Votre session a expir�, veuillez vous reconnecter");
                }

                result = serviceClientAspectize.GetTodoDetails(nTimosSessionId, nIdTodo);

                if (result && result.Data != null)
                {
                    DataSet ds = result.Data as DataSet;
                    FillEntitiesFromDataSet(ds, em);

                    em.Data.AcceptChanges();
                    return em.Data;
                }
            }
            else
            {
                throw new SmartException(1100, "Votre session a expir�, veuillez vous reconnecter");
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
                        throw new SmartException(1100, "Votre session a expir�, veuillez vous reconnecter");
                    }
                    result = serviceClientAspectize.SaveTodo(nTimosSessionId, dataSet, nIdTodo, elementType, elementId);

                    if (!result)
                        throw new SmartException(1010, result.MessageErreur);

                    /* DEBUG
                    var valeurChamp = em.GetAllInstances<TodoValeurChamp>();
                    foreach (var val in valeurChamp)
                    {
                        int idchamp = val.ChampTimosId;
                    }*/
                }
            }
            else
            {
                throw new SmartException(1100, "Votre session a expir�, veuillez vous reconnecter");
            }

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
                    throw new SmartException(1100, "Votre session a expir�, veuillez vous reconnecter");
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
                throw new SmartException(1100, "Votre session a expir�, veuillez vous reconnecter");
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
                    throw new SmartException(1100, "Votre session a expir�, veuillez vous reconnecter");
                }
                result = serviceClientAspectize.SaveCaracteristique(nTimosSessionId, dataSet, nIdCarac, strTypeElement, nIdTodo);
                if (!result)
                    throw new SmartException(1010, result.MessageErreur);

                DataSet dsRetour = result.Data as DataSet;
                FillEntitiesFromDataSet(dsRetour, em);
                em.Data.AcceptChanges();
                return em.Data;
            }
            else
            {
                throw new SmartException(1100, "Votre session a expir�, veuillez vous reconnecter");
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
                    throw new SmartException(1100, "Votre session a expir�, veuillez vous reconnecter");
                }
                result = serviceClientAspectize.DeleteCaracteristique(nTimosSessionId, nIdCarac, strTypeElement);
                if (!result)
                    throw new SmartException(1010, result.MessageErreur);
            }
            else
            {
                throw new SmartException(1100, "Votre session a expir�, veuillez vous reconnecter");
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
                    throw new SmartException(1100, "Votre session a expir�, veuillez vous reconnecter");
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
                throw new SmartException(1100, "Votre session a expir�, veuillez vous reconnecter");
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
                    throw new SmartException(1100, "Votre session a expir�, veuillez vous reconnecter");
                }
                result = serviceClientAspectize.DeleteFile(nTimosSessionId, strKeyFile);
                if(!result)
                {
                    throw new SmartException(1060, result.MessageErreur);
                }

            }
            else
            {
                throw new SmartException(1100, "Votre session a expir�, veuillez vous reconnecter");
            }
        }

        //-----------------------------------------------------------------------------------------
        public byte[] DownloadDocument(string strKeyFile, string strFileName)
        {
            AspectizeUser aspectizeUser = ExecutingContext.CurrentUser;
            IEntityManager em = EntityManager.FromDataSet(DataSetHelper.Create());

            if (aspectizeUser.IsAuthenticated) // Attention probl�me potentiel sur iOS avec les cookies
            {
                int nTimosSessionId = (int)aspectizeUser[CUserTimosWebApp.c_champSessionId];
                ITimosServiceForAspectize serviceClientAspectize = (ITimosServiceForAspectize)C2iFactory.GetNewObject(typeof(ITimosServiceForAspectize));
                CResultAErreur result = serviceClientAspectize.GetSession(nTimosSessionId);
                if (!result)
                {
                    throw new SmartException(1100, "Votre session a expir�, veuillez vous reconnecter");
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
                throw new SmartException(1100, "Votre session a expir�, veuillez vous reconnecter");
            }

            return null;
        }

        //-----------------------------------------------------------------------------------------
        private void FillEntitiesFromDataSet(DataSet ds, IEntityManager em)
        {
            if (ds == null || em == null)
                return;


            // Cr�ation des Todos
            if (ds.Tables.Contains(CTodoTimosWebApp.c_nomTable))
            {
                DataTable tableTodos = ds.Tables[CTodoTimosWebApp.c_nomTable];
                if (tableTodos.Rows.Count > 0)
                {
                    // la premi�re row contient les donn�es du todo demand�
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

                    // Cr�ation des groupes de champs
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

                    // Cr�ation des caract�ristiques
                    if (ds.Tables.Contains(CCaracteristique.c_nomTable))
                    {
                        DataTable tableCaracteristiques = ds.Tables[CCaracteristique.c_nomTable];
                        foreach (DataRow rowCarac in tableCaracteristiques.Rows)
                        {
                            int nIdElement = (int)rowCarac[CCaracteristique.c_champTimosId]; // Id de l'element Timos (Caractristique, Site, Dossier...)
                            string strTypeElement = (string)rowCarac[CCaracteristique.c_champElementType]; // Type de l'�l�ment Timos
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
                }
            }


            // D�finition des champs
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
                    bool bIsSelect = (bool)rowChamp[CChampTimosWebApp.c_champIsChoixParmis];
                    bool bMultiline = (bool)rowChamp[CChampTimosWebApp.c_champIsMultiline];
                    int nIdGroupeAssocie = (int)rowChamp[CChampTimosWebApp.c_champIdGroupeChamps];
                    string strIdCaracAssociee = (string)rowChamp[CChampTimosWebApp.c_champIdCaracteristique];

                    if (bIsSelect)
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


                    try
                    {
                        // Le champ appartient � un Groupe ou une Caracteristique
                        GroupeChamps groupeAssocie = em.GetInstance<GroupeChamps>(nIdGroupeAssocie);
                        if (groupeAssocie != null && champTimos.GetAssociatedInstance<GroupeChamps, RelationGroupeChampsChampsTimos>() != groupeAssocie)
                            em.AssociateInstance<RelationGroupeChampsChampsTimos>(groupeAssocie, champTimos);
                    }
                    catch (Exception ex)
                    {
                        Context.Log(ex, InfoType.Error, "Erreur d'association GroupeChamps <-> ChampTimos" + Environment.NewLine + ex.Message);
                    }
                    try
                    {
                        Caracteristiques caracAssociee = em.GetInstance<Caracteristiques>(strIdCaracAssociee);
                        if (caracAssociee != null && champTimos.GetAssociatedInstance<Caracteristiques, RelationCaracChamp>() != caracAssociee)
                            em.AssociateInstance<RelationCaracChamp>(caracAssociee, champTimos);

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

            // R�cup�re les valeurs de champs associ�es au Todo par groupe de champs
            if (ds.Tables.Contains(CTodoValeurChamp.c_nomTable))
            {
                DataTable tableTodoValeursChamps = ds.Tables[CTodoValeurChamp.c_nomTable];
                foreach (DataRow rowVal in tableTodoValeursChamps.Rows)
                {
                    int nIdTodoValeurChamp = (int)rowVal[CTodoValeurChamp.c_champId];
                    var valChampTimos = em.GetInstance<TodoValeurChamp>(nIdTodoValeurChamp);
                    if (valChampTimos == null)
                        valChampTimos = em.CreateInstance<TodoValeurChamp>();
                    valChampTimos.ChampTimosId = nIdTodoValeurChamp;
                    valChampTimos.LibelleChamp = (string)rowVal[CTodoValeurChamp.c_champLibelle];
                    valChampTimos.OrdreChamp = (int)rowVal[CTodoValeurChamp.c_champOrdreAffichage];
                    valChampTimos.ElementType = (string)rowVal[CTodoValeurChamp.c_champElementType];
                    valChampTimos.ElementId = (int)rowVal[CTodoValeurChamp.c_champElementId];

                    string valeurChamp = (string)rowVal[CTodoValeurChamp.c_champValeur];
                    if (valeurChamp != "")
                    {
                        var champTimos = em.GetInstance<ChampTimos>(valChampTimos.ChampTimosId);
                        if (champTimos.IsSelect)
                        {
                            bool bFound = false;
                            var valPossibles = em.GetAllInstances<ValeursChamp>();
                            foreach (var valPossible in valPossibles)
                            {
                                if (valPossible.StoredValue == valeurChamp)
                                {
                                    valeurChamp = valPossible.StoredValue;
                                    bFound = true;
                                    break;
                                }
                            }
                            if (!bFound)
                                valeurChamp = "ND";
                            //throw new SmartException("Probl�me de valeurs possibles sur le champ id = " + valTimos.ChampTimosId + ", valeurChamp = " + valeurChamp + ", n'est pas dans la liste des valeurs possibles. ");
                        }
                    }
                    valChampTimos.ValeurChamp = valeurChamp;
                    int nIdGroupeAssocie = (int)rowVal[CTodoValeurChamp.c_champIdGroupeChamps];
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

            // R�cup�re les valeurs de champs associ�es aux Caract�ristiques
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

                    ChampTimos champ = em.GetInstance<ChampTimos>(nIdChampTimosAssocie);

                    if (valeurChamp != "")
                    {
                        if (champ != null && champ.IsSelect)
                        {
                            bool bFound = false;
                            var valPossibles = em.GetAllInstances<ValeursChamp>();
                            foreach (var valPossible in valPossibles)
                            {
                                if (valPossible.StoredValue == valeurChamp)
                                {
                                    valeurChamp = valPossible.StoredValue;
                                    bFound = true;
                                    break;
                                }
                            }
                            if (!bFound)
                                valeurChamp = "ND";
                            //throw new SmartException("Probl�me de valeurs possibles sur le champ id = " + valTimos.ChampTimosId + ", valeurChamp = " + valeurChamp + ", n'est pas dans la liste des valeurs possibles. ");
                        }
                    }
                    valChampTimos.ValeurChamp = valeurChamp;
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
        // POUR DEBUG UNIQUEMENT
        public bool TestAppelServeur()
        {
            //throw new SmartException(9900, "Erreur dans test d'appel serveur");
            return true;
        }
        public string TestAppelServeurAvecParmatres(string alpha, string beta)
        {
            return "Param�tre alpha = " + alpha + Environment.NewLine + "Param�tre beta = " + beta;
        }
    }

}

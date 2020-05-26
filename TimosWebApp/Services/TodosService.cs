using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Aspectize.Core;
using System.Security.Permissions;
using timos.data.Aspectize;
using sc2i.common;
using sc2i.multitiers.client;

namespace TimosWebApp.Services
{
    public interface ITodosService
    {
        DataSet GetTodoDetails(int nIdTodo);

        [Command(IsSaveCommand = true)]
        void SaveTodo(DataSet dataSet, int nIdTodo, string elementType, int elementId);
        void EndTodo(int nIdTodo);
        DataSet UploadDocuments(UploadedFile[] uploadedFiles, int nIdTodo, int nIdDocument, string strCategorie);
    }

    [Service(Name = "TodosService")]
    public class TodosService : ITodosService //, IInitializable, ISingleton
    {
        public DataSet GetTodoDetails(int nIdTodo)
        {
            AspectizeUser aspectizeUser = ExecutingContext.CurrentUser;

            if (aspectizeUser.IsAuthenticated)
            {
                int nTimosSessionId = (int)aspectizeUser[CUserTimosWebApp.c_champSessionId];
                IEntityManager em = EntityManager.FromDataSet(DataSetHelper.Create());

                ITimosServiceForAspectize serviceClientAspectize = (ITimosServiceForAspectize)C2iFactory.GetNewObject(typeof(ITimosServiceForAspectize));
                CResultAErreur result = serviceClientAspectize.GetTodoDetails(nTimosSessionId, nIdTodo);

                if (result && result.Data != null)
                {
                    DataSet ds = result.Data as DataSet;
                    if (ds != null && ds.Tables.Contains(CTodoTimosWebApp.c_nomTable) && ds.Tables.Contains(CChampTimosWebApp.c_nomTable))
                    {
                        DataTable tableTodos = ds.Tables[CTodoTimosWebApp.c_nomTable];
                        if (tableTodos.Rows.Count > 0)
                        {
                            DataRow rowTodo = tableTodos.Rows[0]; // la première row contient les données du todo demandé
                            var todo = em.CreateInstance<Todos>();
                            todo.TimosId = (int)rowTodo[CTodoTimosWebApp.c_champId];
                            todo.Label = (string)rowTodo[CTodoTimosWebApp.c_champLibelle];
                            todo.StartDate = (DateTime)rowTodo[CTodoTimosWebApp.c_champDateDebut];
                            todo.Instructions = (string)rowTodo[CTodoTimosWebApp.c_champInstructions];
                            todo.ElementType = (string)rowTodo[CTodoTimosWebApp.c_champTypeElementEdite];
                            todo.ElementId = (int)rowTodo[CTodoTimosWebApp.c_champIdElementEdite];
                            todo.ElementDescription = (string)rowTodo[CTodoTimosWebApp.c_champElementDescription];

                            Dictionary<string, string> dicoChampIdValeurPossible = new Dictionary<string, string>();

                            DataTable tableChampsTimos = ds.Tables[CChampTimosWebApp.c_nomTable];
                            foreach (DataRow rowChamp in tableChampsTimos.Rows)
                            {
                                var champTimos = em.CreateInstance<ChampTimos>();
                                champTimos.Nom = (string)rowChamp[CChampTimosWebApp.c_champNom];
                                champTimos.DisplayOrder = (int) rowChamp[CChampTimosWebApp.c_champOrdreAffichage];
                                champTimos.TimosId = (int) rowChamp[CChampTimosWebApp.c_champId];
                                champTimos.TypeDonneChamp = (TypeDonnee) rowChamp[CChampTimosWebApp.c_champTypeDonne];
                                champTimos.LibelleConvivial = (string)rowChamp[CChampTimosWebApp.c_champLibelleConvivial];
                                bool bIsSelect = (bool)rowChamp[CChampTimosWebApp.c_champIsChoixParmis];
                                bool bMultiline = (bool)rowChamp[CChampTimosWebApp.c_champIsMultiline];


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

                                em.AssociateInstance<RelationTodoDefinitionChamp>(todo, champTimos);

                            }

                            DataTable tableValeursPossibles = ds.Tables[CChampValeursPossibles.c_nomTable];
                            foreach (DataRow rowValPossbile in tableValeursPossibles.Rows)
                            {
                                var valPossible = em.CreateInstance<ValeursChamp>();
                                valPossible.ChampTimosId = rowValPossbile[CChampValeursPossibles.c_champId].ToString();
                                valPossible.Index = (int) rowValPossbile[CChampValeursPossibles.c_champIndex];
                                valPossible.StoredValue= (string) rowValPossbile[CChampValeursPossibles.c_champValue];
                                valPossible.DisplayedValue = (string) rowValPossbile[CChampValeursPossibles.c_champDisplay];

                                em.AssociateInstance<ValeursPossibles>(todo, valPossible);

                            }

                            // Récupère les valeurs de champs
                            DataTable tableValeursChamps = ds.Tables[CTodoValeurChamp.c_nomTable];
                            foreach (DataRow rowVal in tableValeursChamps.Rows)
                            {
                                var valTimos = em.CreateInstance<TodoValeurChamp>();
                                valTimos.LibelleChamp = (string)rowVal[CTodoValeurChamp.c_champLibelle];
                                valTimos.OrdreChamp = (int)rowVal[CTodoValeurChamp.c_champOrdreAffichage];
                                valTimos.ChampTimosId = (int)rowVal[CTodoValeurChamp.c_champId];

                                string valeurChamp = (string)rowVal[CTodoValeurChamp.c_champValeur];

                                if (valeurChamp != "")
                                {
                                    var champTimos = em.GetInstance<ChampTimos>(valTimos.ChampTimosId);
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
                                            throw new SmartException("On a un problème de valeurs possibles sur le champ id " + valTimos.ChampTimosId);
                                    }
                                }
                                valTimos.ValeurChamp = valeurChamp;

                                em.AssociateInstance<RelationTodoValeurChamp>(todo, valTimos);

                            }


                            // Gestion des documents attendus sur todo
                            DataTable tableDocuementsAttendus = ds.Tables[CDocumentAttendu.c_nomTable];
                            foreach (DataRow rowDoc in tableDocuementsAttendus.Rows)
                            {
                                var doc = em.CreateInstance<DocumentsAttendus>();

                                doc.TimosId = (int)rowDoc[CDocumentAttendu.c_champId];
                                doc.Libelle = (string)rowDoc[CDocumentAttendu.c_champLibelle];
                                doc.CategorieDocument = (string)rowDoc[CDocumentAttendu.c_champCategorieDocument];
                                doc.NombreMin = (int)rowDoc[CDocumentAttendu.c_champNombreMin];
                                if (rowDoc[CDocumentAttendu.c_champDateLastUpload] == DBNull.Value)
                                    doc.DateLastUpload = null;
                                else
                                    doc.DateLastUpload = (DateTime)rowDoc[CDocumentAttendu.c_champDateLastUpload];

                                em.AssociateInstance<RelationTodoDocument>(todo, doc);
                            }

                            em.Data.AcceptChanges();
                            return em.Data;
                        }
                    }
                }
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
                    CResultAErreur result = serviceClientAspectize.SaveTodo(nTimosSessionId, dataSet, nIdTodo, elementType, elementId);

                    if (!result)
                        throw new SmartException(1000, result.MessageErreur);

                    /* DEBUG
                    var valeurChamp = em.GetAllInstances<TodoValeurChamp>();
                    foreach (var val in valeurChamp)
                    {
                        int idchamp = val.ChampTimosId;
                    }*/
                }
            }
        }
   
        //-----------------------------------------------------------------------------------------
        public void EndTodo(int nIdTodo)
        {

            throw new SmartException(1004, "Le service EndTodo n'est pas implémenté");
        }

        //-----------------------------------------------------------------------------------------
        public DataSet UploadDocuments(UploadedFile[] uploadedFiles, int nIdTodo, int nIdDocument, string strCategorie)
        {

            IEntityManager em = EntityManager.FromDataSet(DataSetHelper.Create());

            DocumentsAttendus doc = em.CreateInstance<DocumentsAttendus>();
            doc.TimosId = nIdDocument;
            doc.CategorieDocument = strCategorie;
            doc.DateLastUpload = DateTime.Now;

            if (doc != null)
            {
                foreach (UploadedFile file in uploadedFiles)
                {
                    var fichier = em.CreateInstance<FichiersAttaches>();
                    fichier.NomFichier = file.Name;
                    fichier.TimosKey = file.Name;
                    fichier.DateUpload = DateTime.Now;



                    em.AssociateInstance<RelationFichiers>(doc, fichier);
                }
            }
            em.Data.AcceptChanges();
            return em.Data;

        }
    }

}

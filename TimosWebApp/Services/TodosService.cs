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
                            DataRow rowTodo = tableTodos.Rows[0]; // la premlière row contient les données du todo demandé
                            var todo = em.CreateInstance<Todos>();
                            todo.TimosId = (int)rowTodo[CTodoTimosWebApp.c_champId];
                            todo.Label = (string)rowTodo[CTodoTimosWebApp.c_champLibelle];
                            todo.StartDate = (DateTime)rowTodo[CTodoTimosWebApp.c_champDateDebut];
                            todo.Instructions = (string)rowTodo[CTodoTimosWebApp.c_champInstructions];
                            todo.ElementType = (string)rowTodo[CTodoTimosWebApp.c_champTypeElementEdite];
                            todo.ElementId = (int)rowTodo[CTodoTimosWebApp.c_champIdElementEdite];
                            todo.ElementDescription = (string)rowTodo[CTodoTimosWebApp.c_champElementDescription];

                            DataTable tableChampsTimos = ds.Tables[CChampTimosWebApp.c_nomTable];
                            foreach (DataRow rowChamp in tableChampsTimos.Rows)
                            {
                                var champTimos = em.CreateInstance<ChampTimos>();
                                champTimos.Nom = (string)rowChamp[CChampTimosWebApp.c_champNom];
                                champTimos.DisplayOrder = (int) rowChamp[CChampTimosWebApp.c_champOrdreAffichage];
                                champTimos.TimosId = (int) rowChamp[CChampTimosWebApp.c_champId];
                                champTimos.TypeDonneChamp = (TypeDonnee) rowChamp[CChampTimosWebApp.c_champTypeDonne];
                                champTimos.LibelleConvivial = (string)rowChamp[CChampTimosWebApp.c_champLibelleConvivial];

                                switch (champTimos.TypeDonneChamp)
                                {
                                    case TypeDonnee.TypeEntier:
                                        champTimos.AspectizeFieldType = "String";
                                        champTimos.AspectizeControlType = "";
                                        break;
                                    case TypeDonnee.TypeDecimal:
                                        champTimos.AspectizeFieldType = "String";
                                        champTimos.AspectizeControlType = "";
                                        break;
                                    case TypeDonnee.TypeString:
                                        champTimos.AspectizeFieldType = "String";
                                        champTimos.AspectizeControlType = "";
                                        break;
                                    case TypeDonnee.TypeDate:
                                        champTimos.AspectizeFieldType = "String";
                                        champTimos.AspectizeControlType = "";
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


                                var valTimos = em.CreateInstance<TodoValeurChamp>();
                                valTimos.ValeurChamp = "";
                                valTimos.LibelleChamp = (string)rowChamp[CChampTimosWebApp.c_champLibelleConvivial];
                                valTimos.OrdreChamp = (int)rowChamp[CChampTimosWebApp.c_champOrdreAffichage];
                                valTimos.ChampTimosId = (int)rowChamp[CChampTimosWebApp.c_champId];

                                em.AssociateInstance<RelationTodoDefinitionChamp>(todo, champTimos);
                                em.AssociateInstance<RelationTodoValeurChamp>(todo, valTimos);

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
            IEntityManager em = EntityManager.FromDataSet(dataSet);

            var valeurChamp = em.GetAllInstances<TodoValeurChamp>();
            foreach (var val in valeurChamp)
            {
                int idchamp = val.ChampTimosId;
            }

            throw new SmartException(1000, "Données non sauvegardées");
        }
    }

}

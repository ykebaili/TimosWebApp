using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Aspectize.Core;
using System.Security.Permissions;

namespace TimosWebApp.Services
{
    public interface ITodosService
    {
        DataSet GetTodoDetails(int nIdTodo);
    }

    [Service(Name = "TodosService")]
    public class TodosService : ITodosService //, IInitializable, ISingleton
    {
        public DataSet GetTodoDetails(int nIdTodo)
        {
            IEntityManager em = EntityManager.FromDataSet(DataSetHelper.Create());

            var todo = em.CreateInstance<Todos>();
            var champTimos = em.CreateInstance<ChampTimos>();
            var valTimos = em.CreateInstance<TodoValeurChamp>();

            em.AssociateInstance<RelationTodoDefinitionChamp>(todo, champTimos);

            return em.Data;
        }
    }

}

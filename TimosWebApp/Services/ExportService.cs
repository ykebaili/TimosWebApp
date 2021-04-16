using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Aspectize.Core;
using System.Security.Permissions;
using Aspectize.Office;
using sc2i.common;
using timos.data.Aspectize;
using sc2i.multitiers.client;

namespace TimosWebApp.Services
{
    public interface IExportService
    {
        DataSet GetListeExportsForCurrentUser();
        byte[] GetExport();
    }

    [Service(Name = "ExportService")]
    public class ExportService : IExportService //, IInitializable, ISingleton
    {

        public DataSet GetListeExportsForCurrentUser()
        {
            AspectizeUser aspectizeUser = ExecutingContext.CurrentUser;
            IEntityManager em = EntityManager.FromDataSet(DataSetHelper.Create());

            if (aspectizeUser.IsAuthenticated)
            {
                int nTimosSessionId = (int)aspectizeUser[CUserTimosWebApp.c_champSessionId];
                string keyUser = (string)aspectizeUser[CUserTimosWebApp.c_champUserKey];

                ITimosServiceForAspectize serviceClientAspectize = (ITimosServiceForAspectize)C2iFactory.GetNewObject(typeof(ITimosServiceForAspectize));
                CResultAErreur result = serviceClientAspectize.GetSession(nTimosSessionId);
                if (!result)
                {
                    throw new SmartException(1100, "Votre session a expiré, veuillez vous reconnecter");
                }

                try
                {
                    result = serviceClientAspectize.GetExportsForUser(nTimosSessionId, keyUser);
                    if (!result)
                        throw new SmartException(1010, "Erreur GetExportsForUser(nTimosSessionId = " + nTimosSessionId + ", keyUser = " + keyUser + ")" +
                        Environment.NewLine +
                        result.MessageErreur);

                    if (result && result.Data != null)
                    {
                        DataSet ds = result.Data as DataSet;
                        
                        if(ds != null && ds.Tables.Contains(CExportWeb.c_nomTable))
                        {
                            DataTable dt = ds.Tables[CExportWeb.c_nomTable];

                            foreach (DataRow row in dt.Rows)
                            {
                                var export = em.CreateInstance<Export>();
                                export.Id = (int)row[CExportWeb.c_champId];
                                export.Libelle = (string)row[CExportWeb.c_champLibelle];
                                export.Description = (string)row[CExportWeb.c_champDescription];
                                if (row[CExportWeb.c_champDateDonnees] == DBNull.Value)
                                    export.DataDate = DateTime.Now;
                                else
                                    export.DataDate = (DateTime)row[CExportWeb.c_champDateDonnees];
                            }

                        }


                        em.Data.AcceptChanges();
                        return em.Data;
                    }
                }
                catch (Exception ex)
                {
                    throw new SmartException(1010,
                        "Erreur GetExportsForUser(nTimosSessionId = " + nTimosSessionId + ", keyUser = " + keyUser + ")" +
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

        public byte[] GetExport()
        {
            var dsExport = DataSetHelper.Create();

            var dtExport = dsExport.Tables.Add("Organisation");

            dtExport.Columns.Add("Segment", typeof(String));

            var drExport = dtExport.NewRow();
            //drExport["Segment"] = site.Segment;
            dtExport.Rows.Add(drExport);

            IAspectizeExcel aspectizeExcel = ExecutingContext.GetService<IAspectizeExcel>("AspectizeExcel");

            var bytes = aspectizeExcel.ToExcel(dsExport, null);

            ExecutingContext.SetHttpDownloadFileName(string.Format("Organisation_Vitalrest_{0:yyyyMMddHHmm}.xlsx", DateTime.Now));

            return bytes;
        }

    }

}

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
using System.IO;
using Newtonsoft.Json;
using System.Threading;

namespace TimosWebApp.Services
{
    public interface IExportService
    {
        void UpdateAllExports();
        DataSet GetListeExportsForCurrentUser();
        bool GetDataSetExport(string keyExport);
        DataSet GetExportForDisplay(string keyExport, string strLibelle, string strDescription);
        byte[] GetExportForExcel(string keyExport, string strLibelle);
    }

    [Service(Name = "ExportService")]
    public class ExportService : IExportService //, IInitializable, ISingleton
    {


        //-------------------------------------------------------------------------------------------------------------------------------
        public void UpdateAllExports()
        {
            try
            {
                ITimosServiceForAspectize serviceClientAspectize = (ITimosServiceForAspectize)C2iFactory.GetNewObject(typeof(ITimosServiceForAspectize));
                CResultAErreur result = serviceClientAspectize.GetExportsForUser(0, "");

                if (result && result.Data != null)
                {
                    DataSet ds = result.Data as DataSet;

                    if (ds != null && ds.Tables.Contains(CExportWeb.c_nomTable))
                    {
                        DataTable dt = ds.Tables[CExportWeb.c_nomTable];
                        foreach (DataRow row in dt.Rows)
                        {
                            string keyExport = (string)row[CExportWeb.c_champId];

                            try
                            {
                                result = serviceClientAspectize.GetDataSetExport(0, keyExport);
                                if (!result)
                                    Context.Log(InfoType.Warning, result.MessageErreur);

                                if (result && result.Data != null)
                                {
                                    DataSet dsExport = result.Data as DataSet;
                                    if (dsExport != null && dsExport.Tables.Count > 0)
                                    {
                                        DataTable dtExport = dsExport.Tables[0];
                                        var fs = ExecutingContext.GetService<IFileService>("TimosFileService");

                                        string relativePath = keyExport + ".json";
                                        string json = JsonConvert.SerializeObject(dsExport, Formatting.None);
                                        MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(json));
                                        fs.Write(relativePath, stream);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Context.Log(InfoType.Warning, "Erreur GetDataSetExport(" + keyExport + ")" + Environment.NewLine + ex.Message);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Context.Log(InfoType.Warning, ex.Message);
            }

        }
            
        //-------------------------------------------------------------------------------------------------------------------------------
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
                                export.Id = (string)row[CExportWeb.c_champId];
                                export.Libelle = (string)row[CExportWeb.c_champLibelle];
                                export.Description = (string)row[CExportWeb.c_champDescription];
                               
                                var fs = ExecutingContext.GetService<IFileService>("TimosFileService");
                                string relativePath = export.Id + ".json";
                                string fullPath = fs.GetFileUrl(relativePath);
                                fullPath = fullPath.Substring(16);
                                if (File.Exists(fullPath))
                                    export.DataDate = File.GetLastWriteTime(fullPath);
                                else
                                    export.DataDate = null;
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

        //-------------------------------------------------------------------------------------------------------------------------------
        public bool GetDataSetExport(string keyExport)
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
                    result = serviceClientAspectize.GetDataSetExport(nTimosSessionId, keyExport);
                    if (!result)
                        throw new SmartException(1010, "Erreur GetDataSetExport(nTimosSessionId = " + nTimosSessionId + ", keyExport = " + keyExport + ")" +
                        Environment.NewLine +
                        result.MessageErreur);

                    if (result && result.Data != null)
                    {
                        DataSet ds = result.Data as DataSet;
                        if (ds != null && ds.Tables.Count > 0)
                        {
                            var fs = ExecutingContext.GetService<IFileService>("TimosFileService");

                            string relativePath = keyExport + ".json";
                            string json = JsonConvert.SerializeObject(ds, Formatting.None);
                            MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(json));
                            fs.Write(relativePath, stream);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new SmartException(1010,
                        "Erreur GetExportsForUser(nTimosSessionId = " + nTimosSessionId + ", keyExport = " + keyExport + ")" +
                        Environment.NewLine +
                        ex.Message);
                }
            }
            else
            {
                throw new SmartException(1100, "Votre session a expiré, veuillez vous reconnecter");
            }
            return true;
        }


        //-------------------------------------------------------------------------------------------------------------------------------
        public DataSet GetExportForDisplay(string keyExport, string strLibelle, string strDescription)
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
                    IFileService fs = ExecutingContext.GetService<IFileService>("TimosFileService");
                    string relativePath = keyExport + ".json";
                    string fullPath = fs.GetFileUrl(relativePath);
                    fullPath = fullPath.Substring(16);
                    if (File.Exists(fullPath))
                    {
                        byte[] buffer = fs.ReadBytes(relativePath);
                        string jsonLecture = Encoding.ASCII.GetString(buffer);
                        DataSet dsExport = JsonConvert.DeserializeObject<DataSet>(jsonLecture);

                        if (dsExport != null && dsExport.Tables.Count > 0)
                        {
                            Export export = em.CreateInstance<Export>();
                            export.Id = keyExport;
                            export.Libelle = strLibelle;
                            export.Description = strDescription;
                            export.DataDate = File.GetLastWriteTime(fullPath);

                            // Extraction des données du DataSet
                            DataTable tableExport = dsExport.Tables[0]; // On traite uniquement la première table
                            int nIndexCol = 1; // Les 10 premières colonnes uniquement
                            foreach (DataColumn col in tableExport.Columns)
                            {
                                export.data["COL" + nIndexCol] = col.ColumnName;
                                nIndexCol++;
                                if (nIndexCol > 10)
                                    break;
                            }
                            // Traitement des données (lignes)
                            int nIndexRow = 0;
                            foreach (DataRow row in tableExport.Rows)
                            {
                                string strIdCompose = keyExport + "#" + nIndexRow++;
                                ExportDatas expData = em.GetInstance<ExportDatas>(strIdCompose);
                                if (expData == null)
                                {
                                    expData = em.CreateInstance<ExportDatas>();
                                    expData.Id = strIdCompose;
                                    em.AssociateInstance<RelationExportDatas>(export, expData);
                                }
                                for (int i = 0; i < tableExport.Columns.Count && i < 10; i++)
                                {
                                    if (row[i] == DBNull.Value)
                                        expData.data[i+1] = "";
                                    else
                                        expData.data[i+1] = row[i];
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new SmartException(1010,
                        "Erreur GetExportForDisplay(nTimosSessionId = " + nTimosSessionId + ", keyExport = " + keyExport + ")" +
                        Environment.NewLine +
                        ex.Message);
                }
            }
            else
            {
                throw new SmartException(1100, "Votre session a expiré, veuillez vous reconnecter");
            }
            em.Data.AcceptChanges();
            return em.Data;
        }

        //-------------------------------------------------------------------------------------------------------------------------------
        public byte[] GetExportForExcel(string keyExport, string strLibelle)
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
                    IFileService fs = ExecutingContext.GetService<IFileService>("TimosFileService");
                    string relativePath = keyExport + ".json";
                    string fullPath = fs.GetFileUrl(relativePath);
                    fullPath = fullPath.Substring(16);
                    if (File.Exists(fullPath))
                    {
                        byte[] buffer = fs.ReadBytes(relativePath);
                        string jsonLecture = Encoding.ASCII.GetString(buffer);
                        DataSet dsExport = JsonConvert.DeserializeObject<DataSet>(jsonLecture);
                        if (dsExport != null)
                        {
                            IAspectizeExcel aspectizeExcel = ExecutingContext.GetService<IAspectizeExcel>("AspectizeExcel");
                            var bytes = aspectizeExcel.ToExcel(dsExport, null);
                            ExecutingContext.SetHttpDownloadFileName(string.Format(strLibelle + " {0:yyyyMMddHHmm}.xlsx", File.GetLastWriteTime(fullPath)));
                            return bytes;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new SmartException(1010,
                        "Erreur GetExportForExcel(nTimosSessionId = " + nTimosSessionId + ", keyExport = " + keyExport + ")" +
                        Environment.NewLine +
                        ex.Message);
                }

            }
            else
            {
                throw new SmartException(1100, "Votre session a expiré, veuillez vous reconnecter");
            }
            return new byte[] { };
        }
    }
}

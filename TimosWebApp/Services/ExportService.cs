using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Aspectize.Core;
using System.Security.Permissions;
using Aspectize.Office;

namespace TimosWebApp.Services
{
    public interface IExportService
    {
        byte[] GetExport();
    }

    [Service(Name = "ExportService")]
    public class ExportService : IExportService //, IInitializable, ISingleton
    {


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

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Aspectize.Core;
using System.Security.Permissions;
using System.IO;

namespace TimosWebApp.Services
{

    [Service(Name = "EventLogService")]
    public class EventLogService : ILog //, IInitializable, ISingleton
    {
        void ILog.WriteLog(TraceInfo traceInfo)
        {
            string message = traceInfo.Message;

            using (StreamWriter sw = File.AppendText("c:\\partage\\TimosWebAppLog.txt"))
            {
                sw.WriteLine("\r\nLog Entry : " + traceInfo.InfoTypeName);
                sw.WriteLine("Date : " + traceInfo.Received.ToString("dd/MM/yyyy HH:mm:ss"));
                sw.WriteLine("Service name : " + traceInfo.ServiceName);
                sw.WriteLine("Level : " + traceInfo.Level);
                sw.WriteLine("Message : " + message);
                sw.WriteLine("-----------------------------------------------------------------------------------------");
            }
            

        }
    }

}

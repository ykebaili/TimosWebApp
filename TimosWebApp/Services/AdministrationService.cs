using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Aspectize.Core;
using System.Security.Permissions;

namespace TimosWebApp.Services
{
    public interface IAdministrationService
    {
        DataSet UploadFiles(UploadedFile[] uploadedFiles);

    }

    [Service(Name = "AdministrationService")]
    public class AdministrationService : IAdministrationService //, IInitializable, ISingleton
    {

        public DataSet UploadFiles(UploadedFile[] uploadedFiles)
        {
            AspectizeUser aspectizeUser = ExecutingContext.CurrentUser;
            IEntityManager em = EntityManager.FromDataSet(DataSetHelper.Create());

            if (aspectizeUser.IsAuthenticated)
            {
               var fs = ExecutingContext.GetService<IFileService>("TimosFileService");

                foreach (UploadedFile file in uploadedFiles)
                {
                    fs.Write(file.Name, file.Stream);
                }
            }
            else
            {
                throw new SmartException(1100, "Votre session a expiré, veuillez vous reconnecter");
            }
            return em.Data;
        }
    }

}

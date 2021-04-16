var vListeExports = Aspectize.CreateView("ListeExports", aas.Controls.ListeExports, aas.Zones.Home.ZoneInfo, false);
vListeExports.OnActivated.BindCommand(aas.Services.Browser.History.PushState(aas.ViewName.ListeExports));
vListeExports.OnLoad.BindCommand(aas.Services.Server.ExportService.GetListeExportsForCurrentUser(), aas.Data.MainData, true, true);

var vResumeExport = Aspectize.CreateRepeatedView("ResumeExport", aas.Controls.ResumeExport, aas.Zones.ListeExports.PanelResumesExports, aas.Data.MainData.Export);
vResumeExport.LibelleExport.BindData(vResumeExport.ParentData.Libelle);
vResumeExport.DescriptionExport.BindData(vResumeExport.ParentData.Description);

var vAdministration = Aspectize.CreateView("Administration", aas.Controls.Administration, aas.Zones.Home.ZoneInfo, false);
vAdministration.OnActivated.BindCommand(aas.Services.Browser.History.PushState(aas.ViewName.Administration));

vAdministration.UploaderFichierAdmin.MultipleFiles.BindData(true);
vAdministration.UploaderFichierAdmin.OnFileSelected.BindCommand(aas.Services.Server.AdministrationService.UploadFiles(vAdministration.UploaderFichierAdmin.SelectedFile), "", false, true);

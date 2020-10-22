var vAdministration = Aspectize.CreateView("Administration", aas.Controls.Administration, aas.Zones.Home.ZoneInfo, false);
vAdministration.OnActivated.BindCommand(aas.Services.Browser.History.PushState(aas.ViewName.Administration));

vAdministration.UploaderFichierAdmin.MultipleFiles.BindData(true);
vAdministration.UploaderFichierAdmin.OnFileSelected.BindCommand(aas.Services.Server.AdministrationService.UploadFiles(vAdministration.UploaderFichierAdmin.SelectedFile), "", false, true);
vAdministration.BoutonTestAppelServeur.click.BindCommand(aas.Services.Browser.ClientTodosService.TestAppelServeur());
vAdministration.BoutonTestAppelServeurAvecParametres.click.BindCommand(aas.Services.Browser.ClientTodosService.TestAppelServeurParametres(vAdministration.TextAlpha.value, vAdministration.TextBeta.value));

vAdministration.BoutonTestAppelServeurRadius.click.BindCommand(aas.Services.Browser.ClientAuthenticationService.TestAppelServeurRadius(vAdministration.TextIP.value, vAdministration.TextSecret.value, vAdministration.TextUserName.value, vAdministration.TextPassword.value));

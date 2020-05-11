var errorView = Aspectize.CreateView('ErrorView', aas.Controls.Bootstrap.ModalMessage);
errorView.AddAuthorizationRole(aas.Roles.Anonymous, aas.Enum.AccessControl.ReadWrite);
errorView.Title.BindData('Warning');
errorView.CancelCaption.BindData('Close');
errorView.DisplayConfirm.BindData('hidden');
errorView.Message.BindData('');
errorView.OnActivated.BindCommand(aas.Services.Browser.UIService.GetContextValue('ErrorMessage'), errorView.Message, true);
errorView.Cancel.click.BindCommand(aas.Services.Browser.BootStrapClientService.CloseModal(aas.ViewName.ErrorView));

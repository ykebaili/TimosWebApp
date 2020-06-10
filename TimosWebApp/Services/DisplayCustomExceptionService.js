/// <reference path="S:\Delivery\Aspectize.core\AspectizeIntellisenseLibrary.js" />

Global.DisplayCustomExceptionService = {

    aasService: 'DisplayCustomExceptionService',
    
    Display: function (x, m) {

        var defaultMessage = "Your request did not succeed. We ask you to try again. If the problem persists, please contact us.";

        try {
            
            if (Aspectize.App.IsLocalHost) alert(m);

            var uiService = Aspectize.GetService('UIService');

            var message = '';

            var showDefaultMessage = (x.Level && x.Level > 0);

            if (showDefaultMessage) {

                if (x.Message) {
                    message = x.Message;
                }

                if (x.Errors && x.Errors.length > 0 && x.Errors[0].ErrorMessages.length > 0) {
                    message = x.Errors[0].ErrorMessages[0];
                }

            }
            else {
                message = defaultMessage;
            }

            // Show Error Modal
            uiService.SetContextValue('ErrorMessage', message);

            setTimeout(function () {
                if (x.Level === 1020) {
                    Aspectize.ExecuteCommand(aas.Services.Browser.BootStrapClientService.CloseModal(aas.ViewName.ConfirmationEndTodo));
                    Aspectize.ExecuteCommand(aas.Services.Browser.ClientTodosService.ToastAlert("Erreur de saisie", message, "warning"));
                }
                else {
                    Aspectize.ExecuteCommand(aas.Services.Browser.BootStrapClientService.ShowModal(aas.ViewName.ErrorView, false, false));
                }
                if (x.EndDisplay) {
                    x.EndDisplay();
                }

            }, 300);

        }
        catch (e) {
            alert(defaultMessage);
        }

    }
};



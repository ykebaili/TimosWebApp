/// <reference path="S:\Delivery\Aspectize.core\AspectizeIntellisenseLibrary.js" />

Global.DisplayCustomExceptionService = {

    aasService: 'DisplayCustomExceptionService',
    
    Display: function (x, m) {

        var defaultMessage = "Erreur code : " + x.Level + " - Votre demande n'a pas pu aboutir. Impossible de contacter le serveur. Vérifiez votre connexion.";

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
                if (x.Level === 1020) { // Code 1020 : Exception sur EndTodo 
                    Aspectize.ExecuteCommand(aas.Services.Browser.BootStrapClientService.CloseModal(aas.ViewName.ConfirmationEndTodo));
                    //Aspectize.ExecuteCommand(aas.Services.Browser.ClientTodosService.ToastAlert("Erreur de saisie", message, "warning"));
                }
                Aspectize.ExecuteCommand(aas.Services.Browser.BootStrapClientService.ShowModal(aas.ViewName.ErrorView, true, true, false));

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



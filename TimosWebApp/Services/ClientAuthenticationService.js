/// <reference path="C:\www\\Aspectize.core\AspectizeIntellisense.js" />

Global.ClientAuthenticationService = {

    aasService: 'ClientAuthenticationService',
    aasPublished: true,

    //-------------------------------------------------------------------------------------------------------
    // DEBUG UNIQUEMENT
    TestAppelServeurRadius: function (host, secret, user, password) {

        var cmd = Aspectize.PrepareCommand();
        cmd.Attributes.aasShowWaiting = true;
        cmd.Attributes.aasAsynchronousCall = true;
        cmd.OnComplete = function (result) {
            // Executé au retour de l'appel serveur si tout est OK (pas d'excetpion)
            Aspectize.ExecuteCommand(aas.Services.Browser.ClientTodosService.ToastAlert('Appel serveur Radius OK', result));
        }
        cmd.Call(aas.Services.Server.AdministrationService.TestAppelServeurRadius(host, secret, user, password));
    },

    //------------------------------------------------------------ APPEL RADIUS ETAPE 1 ---------------------------------------------
    AuthenticateRadiusEtape1: function (userName, password, rememberMe) {

        var cmd = Aspectize.PrepareCommand();
        cmd.Attributes.aasShowWaiting = true;
        cmd.Attributes.aasAsynchronousCall = true;
        cmd.OnComplete = function (result) {
            
            var parts = result.toString().split('#');

            if (parts[0] == '11') {
                Aspectize.ExecuteCommand(aas.Services.Browser.UIService.SetContextValue('TimosUserName', userName));
                Aspectize.ExecuteCommand(aas.Services.Browser.UIService.SetContextValue('TimosUserPwd', password));
                Aspectize.ExecuteCommand(aas.Services.Browser.UIService.SetContextValue('TimosRememberMe', rememberMe));
                Aspectize.ExecuteCommand(aas.Services.Browser.UIService.SetContextValue('RadiusState', parts[1]));
                // Affiche la vue de saisie du code OTP
                Aspectize.ExecuteCommand(aas.Services.Browser.UIService.ShowView(aas.ViewName.Challenge));
            }
            else {
                Aspectize.ExecuteCommand(aas.Services.Browser.UIService.SetContextValue('TimosUserName', ''));
                Aspectize.ExecuteCommand(aas.Services.Browser.UIService.SetContextValue('TimosUserPwd', ''));
                Aspectize.ExecuteCommand(aas.Services.Browser.UIService.SetContextValue('TimosRememberMe', ''));
                Aspectize.ExecuteCommand(aas.Services.Browser.UIService.SetContextValue('RadiusState', ''));

                Aspectize.ExecuteCommand(aas.Services.Browser.ClientTodosService.ToastAlert('Erreur authentification', 'Login ou mot de passe incorrect : ' + result, 'error'));
            }
        }
        cmd.Call(aas.Services.Server.AuthenticationService.AuthenticateRadius(userName, password));
    },

    //------------------------------------------------------------- APPEL RADIUS ETAPE 2 --------------------------------------------
    AuthenticateRadiusEtape2: function (OTP) {

        var userName = Aspectize.ExecuteCommand(aas.Services.Browser.UIService.GetContextValue('TimosUserName'));
        var password = Aspectize.ExecuteCommand(aas.Services.Browser.UIService.GetContextValue('TimosUserPwd'));
        var rememberMe = Aspectize.ExecuteCommand(aas.Services.Browser.UIService.GetContextValue('TimosRememberMe'));
        var state = Aspectize.ExecuteCommand(aas.Services.Browser.UIService.GetContextValue('RadiusState'));

        var secret = OTP + '#' + password + '#' + state;

        Aspectize.ExecuteCommand(aas.Services.Browser.SecurityServices.Authenticate, userName, secret, rememberMe, function (isAuthenticated) {
            // Authentification 
            if (isAuthenticated) {
                Aspectize.ExecuteCommand(aas.Services.Browser.ClientTodosService.ToastAlert('Authentification réussie', userName));
            }
            else {
                Aspectize.ExecuteCommand(aas.Services.Browser.ClientTodosService.ToastAlert('Echec authentification', 'Code OTP incorrect. STATE : ' + state, 'error'));
            }

        });
    }
};


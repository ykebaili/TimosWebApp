/// <reference path="C:\www\\Aspectize.core\AspectizeIntellisense.js" />

Global.ClientTodosService = {

    aasService: 'ClientTodosService',
    aasPublished: true,
    MainData: 'MainData',

    //-------------------------------------------------------------------------------------------------------
    FiltreTodos: function (filtreLabel, etatDemarre, etatTermine, etatRetard) {
        var filtre = '';
        if (filtreLabel) {
            filtre = '((Label).toLowerCase().indexOf("' + filtreLabel + '".toLowerCase())) !== -1';
        }
        if (etatDemarre || etatTermine || etatRetard) {
            var filtreEtat = '';
            if (etatDemarre)
                filtreEtat += '(EtatTodo == 2)';
            if (etatTermine) {
                if (filtreEtat)
                    filtreEtat += ' || ';
                filtreEtat += '(EtatTodo == 3)';
            }
            if (etatRetard) {
                if (filtreEtat)
                    filtreEtat += ' || ';
                filtreEtat += '(EtatTodo == 6)';
            }

            if (filtre)
                filtre += ' && ';
            filtre += '(' + filtreEtat + ')';
        }
        Aspectize.ExecuteCommand(aas.Services.Browser.UIService.SetCustomFilter(aas.ViewName.ListeTodos.GridListeTodos, filtre));

    },

    //-------------------------------------------------------------------------------------------------------
    InitPropertyGridOld: function (gridId, edit) {

        var colLabel = 'col-xs-12 col-md-4';
        var colValue = 'col-xs-12 col-md-8';

        if (edit) {
            $('#' + gridId + '.aasPropertyGrid').addClass('form-horizontal');
            $('#' + gridId + '.aasPropertyGrid > div.aasDynamicControl').addClass('form-group');
            $('#' + gridId + '.aasPropertyGrid > .aasDynamicControl .aasLabelZone').addClass('control-label ' + colLabel);
            $('#' + gridId + '.aasPropertyGrid > .aasDynamicControl textarea').addClass('form-control').wrap("<div class='" + colValue + "'></div>");
            $('#' + gridId + '.aasPropertyGrid > .aasDynamicControl select').addClass('form-control').wrap("<div class='" + colValue + "'></div>");
            $('#' + gridId + '.aasPropertyGrid > .aasDynamicControl input[type=\'text\']:not(.BootstrapDateTimePicker)').addClass('form-control').wrap("<div class='" + colValue + "'></div>");
            $('#' + gridId + '.aasPropertyGrid > .aasDynamicControl input[type=\'number\']').addClass('form-control').wrap("<div class='" + colValue + "'></div>");
            $('#' + gridId + '.aasPropertyGrid > .aasDynamicControl input[type="checkbox"]').wrap("<div class='" + colValue + "'><div class='checkbox'><label></label></div></div>");

            $('#' + gridId + '.aasPropertyGrid > .aasDynamicControl .aasDate.aasValueZone').addClass(colValue);
        } else {
            $('.aasPropertyGrid').addClass('form-horizontal');
            $('.aasPropertyGrid > div.aasDynamicControl').addClass('form-group');
            $('.aasPropertyGrid > .aasDynamicControl .aasLabelZone').addClass('control-label ' + colLabel);
            if (!$('.aasPropertyGrid > .aasDynamicControl .aasValueZone').parent().hasClass('form-control-static')) {
                $('.aasPropertyGrid > .aasDynamicControl .aasValueZone').wrap("<div class='" + colValue + " form-control-static'></div>");
            }
        }

    },

    //-------------------------------------------------------------------------------------------------------
    InitPropertyGrid: function (gridId) {

        var colLabel = 'col-xs-12 col-md-4';
        var colValue = 'col-xs-12 col-md-8';

        gridId = jq(gridId);

        $('#' + gridId + '.aasPropertyGrid').addClass('form-horizontal');
        $('#' + gridId + '.aasPropertyGrid > div.aasDynamicControl').addClass('form-group');
        $('#' + gridId + '.aasPropertyGrid > .aasDynamicControl .aasLabelZone').addClass('control-label ' + colLabel);
        $('#' + gridId + '.aasPropertyGrid > .aasDynamicControl .aasValueZoneContainer').addClass(colValue);
        $('#' + gridId + '.aasPropertyGrid > .aasDynamicControl .aasValueZoneContainer div.aasValueZone').addClass('form-control-static');

        $('#' + gridId + '.aasPropertyGrid > .aasDynamicControl .aasValueZoneContainer textarea').addClass('form-control');
        $('#' + gridId + '.aasPropertyGrid > .aasDynamicControl .aasValueZoneContainer select').addClass('form-control');
        $('#' + gridId + '.aasPropertyGrid > .aasDynamicControl .aasValueZoneContainer input[type=\'text\']:not(.BootstrapDateTimePicker)').addClass('form-control');
        $('#' + gridId + '.aasPropertyGrid > .aasDynamicControl .aasValueZoneContainer input[type=\'number\']').addClass('form-control');

    },


    //-------------------------------------------------------------------------------------------------------
    UploadDocument: function (id, uploader) {

        //DocumentAttendu:0-UploaderDocument
        //DocumentAttendu-UploaderDocument

        var remplace = ':' + id + '-';
        var selector = uploader.replace('-', remplace);

        //alert('#' + selector + ' input');

        $('#' + jq(selector) + ' input')[0].click();

    },

    //-------------------------------------------------- Terminer un todo -------------------------------------------
    EndTodo: function (nIdTodo, labelTodo) {

        var cmd = Aspectize.PrepareCommand();

        cmd.Attributes.aasShowWaiting = true;
        cmd.Attributes.aasAsynchronousCall = true;
        cmd.Attributes.aasMergeData = true;
        cmd.Attributes.aasDataName = this.MainData;
        cmd.OnComplete = function (result) {
            // Executé au retour de l'appel serveur si tout est OK (pas d'excetpion)
            Aspectize.ExecuteCommand(aas.Services.Browser.BootStrapClientService.CloseModal(aas.ViewName.ConfirmationEndTodo));
            Aspectize.ExecuteCommand(aas.Services.Browser.ClientTodosService.ToastAlert("Todo terminé", "L'étape " + labelTodo + " a été validée avec succès."));
        }
        cmd.Call(aas.Services.Server.TodosService.EndTodo(nIdTodo));
    },

    //-------------------------------------------------- Supprimer un fihcier attaché -------------------------------------------
    DeleteDocument: function (strKeyFile, strNomFichier) {

        var dataName = this.MainData;
        var cmd = Aspectize.PrepareCommand();
        cmd.Attributes.aasShowWaiting = true;
        cmd.Attributes.aasAsynchronousCall = true;
        cmd.OnComplete = function (result) {
            var em = Aspectize.EntityManagerFromContextDataName(dataName);
            em.ClearInstance('FichiersAttaches', { 'TimosKey': strKeyFile });
            Aspectize.ExecuteCommand(aas.Services.Browser.BootStrapClientService.CloseModal(aas.ViewName.ConfirmationSupprimerFichier));
            Aspectize.ExecuteCommand(aas.Services.Browser.ClientTodosService.ToastAlert("Fichier supprimé", "Le fichier " + strNomFichier + " a été supprimé avec succès."));
        }
        cmd.Call(aas.Services.Server.TodosService.DeleteDocument(strKeyFile));

    },

    //-------------------------------------------------- Visualier un fihcier attaché -------------------------------------------
    DownloadDocument: function (strKeyFile, strNomFichier) {

        var dataName = this.MainData;
        var cmd = Aspectize.PrepareCommand();
        cmd.Attributes.aasShowWaiting = true;
        cmd.Attributes.aasAsynchronousCall = true;
        cmd.OnComplete = function (result) {

        }
        cmd.Call(aas.Services.Server.TodosService.DownloadDocument(strKeyFile));

    },


    //------------------------------------------------------ TOASTR --------------------------------------------
    ToastAlert: function (titre, message, state) {

        state = state || "success";
        toastr.options = {
            "closeButton": true,
            "debug": false,
            "newestOnTop": false,
            "progressBar": false,
            "positionClass": "toast-top-center",
            "preventDuplicates": false,
            "onclick": null,
            "showDuration": "3000",
            "hideDuration": "1000",
            "timeOut": "9000",
            "extendedTimeOut": "1000",
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut"
        }

        toastr[state](message, titre);

    },

    //-------------------------------------------------------------------------------------------------------
    ExpandGroup: function (nIdGroupe) {

        var em = Aspectize.EntityManagerFromContextDataName(this.MainData);
        var groupeToExpand = em.GetInstance('GroupeChamps', { 'TimosId': nIdGroupe });
        groupeToExpand.SetField('Expand', !groupeToExpand.Expand);

    },

    //-------------------------------------------------------------------------------------------------------
    // DEBUG UNIQUEMENT
    TestAppelServeur: function () {

        var cmd = Aspectize.PrepareCommand();
        cmd.Attributes.aasShowWaiting = true;
        cmd.Attributes.aasAsynchronousCall = true;
        cmd.OnComplete = function (result) {
            // Executé au retour de l'appel serveur si tout est OK (pas d'excetpion)
            Aspectize.ExecuteCommand(aas.Services.Browser.ClientTodosService.ToastAlert("Appel serveur OK", "Resultat : " + result));
        }
        cmd.Call(aas.Services.Server.TodosService.TestAppelServeur());
    },

    //-------------------------------------------------------------------------------------------------------
    // DEBUG UNIQUEMENT
    TestAppelServeurParametres: function (alpha, beta) {

        var cmd = Aspectize.PrepareCommand();
        cmd.Attributes.aasShowWaiting = true;
        cmd.Attributes.aasAsynchronousCall = true;
        cmd.OnComplete = function (result) {
            // Executé au retour de l'appel serveur si tout est OK (pas d'excetpion)
            Aspectize.ExecuteCommand(aas.Services.Browser.ClientTodosService.ToastAlert('Appel serveur OK', result));
        }
        cmd.Call(aas.Services.Server.TodosService.TestAppelServeurAvecParmatres(alpha, beta));
    },

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

    //---------------------------------------------------------------------------------------------------------
    AuthenticateRadiusEtape1: function (userName, password, rememberMe) {

        var cmd = Aspectize.PrepareCommand();
        cmd.Attributes.aasShowWaiting = true;
        cmd.Attributes.aasAsynchronousCall = true;
        cmd.OnComplete = function (result) {
            // Executé au retour de l'appel serveur si tout est OK (pas d'excetpion)
            if(result) {
                Aspectize.ExecuteCommand(aas.Services.Browser.UIService.SetContextValue('TimosUserName', userName));
                Aspectize.ExecuteCommand(aas.Services.Browser.UIService.SetContextValue('TimosUserPwd', password));
                Aspectize.ExecuteCommand(aas.Services.Browser.UIService.SetContextValue('TimosRememberMe', rememberMe));
                Aspectize.ExecuteCommand(aas.Services.Browser.UIService.ShowView(aas.ViewName.Challenge));
            }
            else {
                Aspectize.ExecuteCommand(aas.Services.Browser.UIService.SetContextValue('TimosUserName', ''));
                Aspectize.ExecuteCommand(aas.Services.Browser.UIService.SetContextValue('TimosUserPwd', ''));
                Aspectize.ExecuteCommand(aas.Services.Browser.UIService.SetContextValue('TimosRememberMe', ''));
                Aspectize.ExecuteCommand(aas.Services.Browser.ClientTodosService.ToastAlert('Erreur', 'Login ou mot de passe incorrect', 'error'));
            }
        }
        cmd.Call(aas.Services.Server.AuthenticationService.AuthenticateRadius(userName, password));
    },

    AuthenticateRadiusEtape2: function (OTP) {

        var userName = Aspectize.ExecuteCommand(aas.Services.Browser.UIService.GetContextValue('TimosUserName'));
        var password = Aspectize.ExecuteCommand(aas.Services.Browser.UIService.GetContextValue('TimosUserPwd'));
        var rememberMe = Aspectize.ExecuteCommand(aas.Services.Browser.UIService.GetContextValue('TimosRememberMe'));

        var secret = OTP + '#' + password;

        Aspectize.ExecuteCommand(aas.Services.Browser.SecurityServices.Authenticate, userName, secret, rememberMe, function (isAuthenticated) {
            // Authentification 
            if (isAuthenticated) {
                //Aspectize.ExecuteCommand(aas.Services.Browser.ClientTodosService.ToastAlert('OK', ''));
            }
            else {
                Aspectize.ExecuteCommand(aas.Services.Browser.ClientTodosService.ToastAlert('Erreur OTP', 'Echec authentification', 'error'));
            }

        });
    }

};


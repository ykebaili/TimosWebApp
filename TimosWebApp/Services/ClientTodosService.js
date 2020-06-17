/// <reference path="C:\www\\Aspectize.core\AspectizeIntellisense.js" />

Global.ClientTodosService = {

    aasService: 'ClientTodosService',
    aasPublished: true,
    MainData : 'MainData',

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
    InitPropertyGrid: function (gridId, edit) {

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
        
    }


};


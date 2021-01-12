/// <reference path="C:\www\\Aspectize.core\AspectizeIntellisenseLibrary.js" />

Global.ClientTodosService = {

    aasService: 'ClientTodosService',
    aasPublished: true,
    MainData: 'MainData',
    IdNegatif: -999999,

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
    InitPropertyGrid: function (gridId) {

        var colLabel = 'col-xs-3 col-md-4';
        var colValue = 'col-xs-9 col-md-8';

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

    //---------------------------------------------------------------------------------------------------
    AddCaracteristic: function (nIdTodo, nIdGroupe) {

        Aspectize.ExecuteCommand(aas.Services.Browser.BootStrapClientService.ShowModal(aas.ViewName.EditionCarac, false, false, true));

        var em = Aspectize.EntityManagerFromContextDataName(this.MainData);
        var todo = em.GetInstance('Todos', { TimosId: nIdTodo });
        var groupe = em.GetInstance('GroupeChamps', { 'TimosId': nIdGroupe });
        var caracteristics = todo.GetAssociated('RelationTodoCaracteristique', 'Caracteristiques').Filter('IdGroupePourFiltre === ' + nIdGroupe + ' && IsTemplate');
        
        if (caracteristics.length > 0) {
            var caracTemplate = caracteristics[0];
            if (caracTemplate) {

                var nIdNegatif = this.IdNegatif--;
                var nIdGroupe = caracTemplate.IdGroupePourFiltre;
                var strTypeElement = caracTemplate.ElementType;
                var lastPoint = strTypeElement.lastIndexOf('.');
                var idProvisoir = strTypeElement.substring(lastPoint + 1, strTypeElement.length - 1) + nIdNegatif;

                var newCarac = em.CreateInstance('Caracteristiques', { 'Id': idProvisoir });
                em.AssociateInstance('RelationTodoCaracteristique', todo, 'Todos', newCarac, 'Caracteristiques');
                newCarac.SetField('TimosId',  nIdNegatif);
                newCarac.SetField('ElementType', strTypeElement);
                newCarac.SetField('IdGroupePourFiltre', nIdGroupe);
                newCarac.SetField('IsTemplate', false);
                newCarac.SetField('Titre', caracTemplate.Titre);
                newCarac.SetField('IdMetaType', caracTemplate.IdMetaType);

                // Association de tous les Champs
                var champs = caracTemplate.GetAssociated('RelationCaracChamp', 'ChampTimos');
                for (var i = 0; i < champs.length; i++) {
                    var champ = champs[i];
                    em.AssociateInstance('RelationCaracChamp', newCarac, 'Caracteristiques', champ, 'ChampTimos');
                }
                // Association de toutes les valeurs possibles
                var valPossibles = caracTemplate.GetAssociated('RelationCaracValeursPossibles', 'ValeursChamp');
                for (var i = 0; i < valPossibles.length; i++) {
                    var valPossible = valPossibles[i];
                    em.AssociateInstance('RelationCaracValeursPossibles', newCarac, 'Caracteristiques', valPossible, 'ValeursChamp');
                }
                // Création et association de toutes les valeurs de champs
                var valeurs = caracTemplate.GetAssociated('RelationCaracValeurChamp', 'CaracValeurChamp');
                for (var i = 0; i < valeurs.length; i++) {
                    var valeur = valeurs[i];
                    var newValeur = em.CreateInstance('CaracValeurChamp', { 'Id': newCarac.Id + '-' + valeur.ChampTimosId });
                    newValeur.SetField('LibelleChamp', valeur.LibelleChamp);
                    newValeur.SetField('OrdreChamp', valeur.OrdreChamp);
                    newValeur.SetField('ChampTimosId', valeur.ChampTimosId);
                    newValeur.SetField('ElementType', valeur.ElementType);
                    newValeur.SetField('ElementId', newCarac.TimosId);
                    newValeur.SetField('ValeurChamp', '');

                    em.AssociateInstance('RelationCaracValeurChamp', newCarac, 'Caracteristiques', newValeur, 'CaracValeurChamp');
                }

                Aspectize.ExecuteCommand(aas.Services.Browser.UIService.SetCurrent(aas.Path.MainData.Todos.RelationTodoCaracteristique.Caracteristiques, newCarac.Id));
            }
        }
    },

    //---------------------------------------------------------- Sauvegarde d'une Caracteristique ----------------------------------------
    SaveCaracteristic: function (dataSet, nIdCarac, strTypeElement, nIdMetaType, nIdTodo, nIdElementParent, strTypeElmentParent) {

        var em = Aspectize.EntityManagerFromContextDataName(this.MainData);
        var cmd = Aspectize.PrepareCommand();
        cmd.Attributes.aasShowWaiting = true;
        cmd.Attributes.aasAsynchronousCall = true;
        cmd.Attributes.aasMergeData = true;
        cmd.Attributes.aasDataName = this.MainData;
        cmd.OnComplete = function (result) {
            if (nIdCarac < 0) {
                //em.ClearInstance('Caracteristiques', { TimosId: nIdCarac });
            }
            Aspectize.ExecuteCommand(aas.Services.Browser.BootStrapClientService.CloseModal(aas.ViewName.EditionCarac));
            
        }
        cmd.Call(aas.Services.Server.TodosService.SaveCaracteristique(dataSet, nIdCarac, strTypeElement, nIdMetaType, nIdTodo, nIdElementParent, strTypeElmentParent));
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
            Aspectize.ExecuteCommand(aas.Services.Browser.ClientTodosService.ToastAlert("Appel serveur OK", "Resultat : \n" + result));
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
    }

};


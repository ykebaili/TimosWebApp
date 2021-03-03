/// <reference path="C:\www\\Aspectize.core\AspectizeIntellisenseLibrary.js" />

Global.ClientTodosService = {

    aasService: 'ClientTodosService',
    aasPublished: true,
    MainData: 'MainData',
    IdNegatif: -10000,

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

        var colLabel = 'col-xs-6 col-md-4';
        var colValue = 'col-xs-6 col-md-8';

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

        $('#' + gridId + ' select').selectpicker({
            liveSearch: true,
            size: 10
        });

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

    //-------------------------------------------------------------------------------------------------------
    ExecuteAction: function (dataSet, nIdAction, elementType, elementId)
    {
        var cmd = Aspectize.PrepareCommand();
        cmd.Attributes.aasShowWaiting = true;
        cmd.Attributes.aasAsynchronousCall = true;

        cmd.OnComplete = function (result) {
            Aspectize.ExecuteCommand(aas.Services.Browser.BootStrapClientService.CloseModal(aas.ViewName.ExecutionAction));
            Aspectize.ExecuteCommand(aas.Services.Browser.ClientTodosService.ToastAlert(result));
        }
        cmd.Call(aas.Services.Server.TodosService.ExecuteAction(dataSet, nIdAction, elementType, elementId));

    },


    //---------------------------------------------------------------------------------------------------
    AddCaracteristic: function (nIdTodo, nIdGroupe) {

        var em = Aspectize.EntityManagerFromContextDataName(this.MainData);

        Aspectize.ExecuteCommand(aas.Services.Browser.DataRecorder.Start(em.GetDataSet()));
        //Aspectize.ExecuteCommand(aas.Services.Browser.BootStrapClientService.ShowModal(aas.ViewName.EditionCarac, false, false, true));

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
                var idProvisoir = strTypeElement.substring(lastPoint + 1) + nIdNegatif;
                idProvisoir = idProvisoir.replace('-', 'N');

                var newCarac = em.CreateInstance('Caracteristiques', { 'Id': idProvisoir });
                
                newCarac.SetField('TimosId',  nIdNegatif);
                newCarac.SetField('ElementType', strTypeElement);
                newCarac.SetField('IdGroupePourFiltre', nIdGroupe);
                newCarac.SetField('IsTemplate', false);
                newCarac.SetField('Titre', caracTemplate.Titre);
                newCarac.SetField('IdMetaType', caracTemplate.IdMetaType);
                newCarac.SetField('ParentElementType', caracTemplate.ParentElementType);
                newCarac.SetField('ParentElementId', caracTemplate.ParentElementId);

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
                    var newValeur = em.CreateInstance('CaracValeurChamp', { 'Id': newCarac.Id + valeur.ChampTimosId });
                    newValeur.SetField('LibelleChamp', valeur.LibelleChamp);
                    newValeur.SetField('OrdreChamp', valeur.OrdreChamp);
                    newValeur.SetField('ChampTimosId', valeur.ChampTimosId);
                    newValeur.SetField('ElementType', valeur.ElementType);
                    newValeur.SetField('ElementId', newCarac.TimosId);
                    newValeur.SetField('ValeurChamp', valeur.ValeurChamp);

                    em.AssociateInstance('RelationCaracValeurChamp', newCarac, 'Caracteristiques', newValeur, 'CaracValeurChamp');
                }
                em.AssociateInstance('RelationTodoCaracteristique', todo, 'Todos', newCarac, 'Caracteristiques');
                Aspectize.ExecuteCommand(aas.Services.Browser.UIService.SetCurrent(aas.Path.MainData.Todos.RelationTodoCaracteristique.Caracteristiques, newCarac.Id));
                Aspectize.ExecuteCommand(aas.Services.Browser.BootStrapClientService.ShowModal(aas.ViewName.EditionCarac, false, false, true));
            }
        }
        else {
            Aspectize.ExecuteCommand(aas.Services.Browser.DataRecorder.CancelRowChanges(em.GetDataSet()));
            Aspectize.ExecuteCommand(aas.Services.Browser.ClientTodosService.ToastAlert('Opération non autorisée', 'Ajout impossible de :  ' + groupe.TitreCaracteristiques, 'error'));
        }
    },

    //---------------------------------------------------------- Sauvegarde d'une Caracteristique ----------------------------------------
    SaveCaracteristic: function (dataSet, nIdCarac, strTypeElement, nIdTodo) {

        var em = Aspectize.EntityManagerFromContextDataName(this.MainData);
        var cmd = Aspectize.PrepareCommand();
        cmd.Attributes.aasShowWaiting = true;
        cmd.Attributes.aasAsynchronousCall = true;
        cmd.Attributes.aasMergeData = true;
        cmd.Attributes.aasDataName = this.MainData;

        var lastPoint = strTypeElement.lastIndexOf('.');
        var idProvisoir = strTypeElement.substring(lastPoint + 1) + nIdCarac;
        idProvisoir = idProvisoir.replace('-', 'N');

        if (nIdCarac < 0) {
            var caracAnettoyer = em.GetInstance('Caracteristiques', { Id: idProvisoir });
            if (caracAnettoyer) {
                var valeursPossibles = caracAnettoyer.GetAssociated('RelationCaracValeursPossibles', 'ValeursChamp');
                for (var i = 0; i < valeursPossibles.length; i++) {
                    var valPossible = valeursPossibles[i];
                    em.ClearAssociation('RelationCaracValeursPossibles', caracAnettoyer, 'Caracteristiques', valPossible, 'ValeursChamp');
                }
                var champsTimos = caracAnettoyer.GetAssociated('RelationCaracChamp', 'ChampTimos');
                for (var i = 0; i < champsTimos.length; i++) {
                    var champTimos = champsTimos[i];
                    em.ClearAssociation('RelationCaracChamp', caracAnettoyer, 'Caracteristiques', champTimos, 'ChampTimos');
                }
            }
        }

        cmd.OnComplete = function (result) {
            if (nIdCarac < 0) {
                var caracAsupprimer = em.GetInstance('Caracteristiques', { Id: idProvisoir });
                if (caracAsupprimer) {
                    var valeursAsupprimer = caracAsupprimer.GetAssociated('RelationCaracValeurChamp', 'CaracValeurChamp');
                    for (var i = 0; i < valeursAsupprimer.length; i++) {
                        var valeur = valeursAsupprimer[i];
                        em.ClearInstance('CaracValeurChamp', {'Id' : valeur.Id})
                    }
                    em.ClearInstance('Caracteristiques', { 'Id' : idProvisoir });
                }
            }
            Aspectize.ExecuteCommand(aas.Services.Browser.BootStrapClientService.CloseModal(aas.ViewName.EditionCarac));
            
        }
        cmd.Call(aas.Services.Server.TodosService.SaveCaracteristique(dataSet, nIdCarac, strTypeElement, nIdTodo));
    },
    
    //-----------------------------------------------------  Supprime une Caracterisique --------------------------------------------
    DeleteCaracteristc: function(nIdCarac, strTypeElement){

        var em = Aspectize.EntityManagerFromContextDataName(this.MainData);
        var cmd = Aspectize.PrepareCommand();
        cmd.Attributes.aasShowWaiting = true;
        cmd.Attributes.aasAsynchronousCall = true;
        cmd.Attributes.aasMergeData = true;
        cmd.Attributes.aasDataName = this.MainData;
        cmd.OnComplete = function (result) {

            var lastPoint = strTypeElement.lastIndexOf('.');
            var idProvisoir = strTypeElement.substring(lastPoint + 1) + nIdCarac;
            var caracAsupprimer = em.GetInstance('Caracteristiques', { Id: idProvisoir });
            if (caracAsupprimer) {
                var valeursAsupprimer = caracAsupprimer.GetAssociated('RelationCaracValeurChamp', 'CaracValeurChamp');
                for (var i = 0; i < valeursAsupprimer.length; i++) {
                    var valeur = valeursAsupprimer[i];
                    em.ClearInstance('CaracValeurChamp', { 'Id': valeur.Id })
                }
                em.ClearInstance('Caracteristiques', { 'Id': idProvisoir });
            }

        }
        cmd.Call(aas.Services.Server.TodosService.DeleteCaracteristique(nIdCarac, strTypeElement));
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
        cmd.Call(aas.Services.Server.AdministrationService.TestAppelServeur());
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
        cmd.Call(aas.Services.Server.AdministrationService.TestAppelServeurAvecParmatres(alpha, beta));
    },

    //-------------------------------------------------------------------------------------------------------
    // Récupère la valeur de l'input AutoComplete et set la valeur du champ Timos associé
    SelectDataFromList: function (aasEventArg, nIdChamp, nIdGroupe, nIdCarac) {

        if(aasEventArg && aasEventArg.Item){
            var item = aasEventArg.Item;
            //alert(item.label);
            var valeur = item.value;

            var em = Aspectize.EntityManagerFromContextDataName(this.MainData);
            var groupeChamps = em.GetInstance('GroupeChamps', { 'TimosId': nIdGroupe });
            if (groupeChamps) {
                var idValeurChamp = nIdGroupe + '' + nIdChamp;
                var todoValeurChamp = em.GetInstance('TodoValeurChamp', { 'Id': idValeurChamp });
                if (todoValeurChamp) {
                    todoValeurChamp.SetField('ValeurChamp', valeur);
                }
            }
            
        }
        
    }

};


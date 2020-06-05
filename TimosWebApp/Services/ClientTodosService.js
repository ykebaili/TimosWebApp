/// <reference path="C:\www\\Aspectize.core\AspectizeIntellisense.js" />

Global.ClientTodosService = {

    aasService: 'ClientTodosService',
    aasPublished: true,
    MainData : 'MainData',

    FiltreTodos: function (filtreLabel) {
        var filtre = '';
        if (filtreLabel) {
            filtre = '((Label).toLowerCase().indexOf("' + filtreLabel + '".toLowerCase())) !== -1';
        }
        Aspectize.ExecuteCommand(aas.Services.Browser.UIService.SetCustomFilter(aas.ViewName.ListeTodos.GridListeTodos, filtre));

    },

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
            Aspectize.ExecuteCommand(aas.Services.Browser.ClientTodosService.ToastAlert("L'étape " + labelTodo + " a été validée avec succès.", "Todo terminé"));
        }
        cmd.Call(aas.Services.Server.TodosService.EndTodo(nIdTodo));
    },

    //------------------------------------------------------ TOASTR --------------------------------------------
    ToastAlert: function (titre, message) {

        toastr.options = {
            "closeButton": false,
            "debug": false,
            "newestOnTop": false,
            "progressBar": true,
            "positionClass": "toast-top-full-width",
            "preventDuplicates": false,
            "onclick": null,
            "showDuration": "3000",
            "hideDuration": "1000",
            "timeOut": "7000",
            "extendedTimeOut": "1000",
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut"
        }

        toastr["success"](message, titre);
                
    },

    ExpandGroup: function (nIdGroupe) {
        
        var em = Aspectize.EntityManagerFromContextDataName(this.MainData);
        var groupeToExpand = em.GetInstance('GroupeChamps', { 'TimosId': nIdGroupe });
        groupeToExpand.SetField('Expand', !groupeToExpand.Expand);
        
    }


};


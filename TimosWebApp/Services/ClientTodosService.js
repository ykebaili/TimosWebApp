/// <reference path="C:\www\\Aspectize.core\AspectizeIntellisense.js" />

Global.ClientTodosService = {

    aasService: 'ClientTodosService',
    aasPublished: true,

    FiltreTodos: function (filtreLabel) {
        var filtre = '';
        if (filtreLabel) {
            filtre = '((Label).toLowerCase().indexOf("' + filtreLabel + '".toLowerCase())) !== -1';
        }
        Aspectize.ExecuteCommand(aas.Services.Browser.UIService.SetCustomFilter(aas.ViewName.ListeTodos.GridListeTodos, filtre));

    },

    InitPropertyGrid: function (gridId, edit) {

        var colLabel = 'col-xs-4';
        var colValue = 'col-xs-8';

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
            $('#' + gridId + '.aasPropertyGrid').addClass('form-horizontal');
            $('#' + gridId + '.aasPropertyGrid > div.aasDynamicControl').addClass('form-group');
            $('#' + gridId + '.aasPropertyGrid > .aasDynamicControl .aasLabelZone').addClass('control-label ' + colLabel);
            if (!$('#' + gridId + '.aasPropertyGrid > .aasDynamicControl .aasValueZone').parent().hasClass('form-control-static')) {
                $('#' + gridId + '.aasPropertyGrid > .aasDynamicControl .aasValueZone').wrap("<div class='" + colValue + " form-control-static'></div>");
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

    }

};


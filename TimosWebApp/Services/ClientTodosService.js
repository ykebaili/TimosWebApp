/// <reference path="C:\www\\Aspectize.core\AspectizeIntellisense.js" />

Global.ClientTodosService = {

   aasService:'ClientTodosService',
   aasPublished:true,
      
   FiltreTodos : function (filtreLabel) {
       var filtre = '';
       if(filtreLabel)
       {
           filtre = '((Label).toLowerCase().indexOf("' + filtreLabel + '".toLowerCase())) !== -1';
       }
       Aspectize.ExecuteCommand(aas.Services.Browser.UIService.SetCustomFilter(aas.ViewName.ListeTodos.GridListeTodos, filtre));
   },

   InitPropertyGrid: function () {
       $('.aasPropertyGrid').addClass('form-horizontal');
       $('.aasPropertyGrid .aasDynamicControl textarea').addClass('form-control col-xs-6');
       $('.aasPropertyGrid .aasDynamicControl input').addClass('form-control col-xs-6');
       $('.aasPropertyGrid .aasDynamicControl label').addClass('control-label col-xs-6');
       $('.aasPropertyGrid > div.aasDynamicControl').addClass('row col-xs-12');
   }
};


/// <reference path="C:\www\\Aspectize.core\AspectizeIntellisense.js" />

Global.ClientTodosService = {

   aasService:'ClientTodosService',
   aasPublished:true,
      
   FiltreTodos : function (filtreLabel) {
       var filtre = '';
       if(filtreLabel)
       {
           filtre = '((Label).toLowerCase().indexOf("' + filtreLabel + '".toLowerCase()) !== -1';
       }
       Aspectize.ExecuteCommand(aas.Services.Browser.UIService.SetCustomFilter(aas.ViewName.ListeTodos.GridListeTodos, filtre));
   }
};


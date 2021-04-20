/// <reference path="C:\www\\Aspectize.core\AspectizeIntellisense.js" />

Global.ClientExportService = {

   aasService:'ClientExportService',
   aasPublished:true,
      
   //-------------------------------------------------------------------------------------------------------
   FiltreDatas : function (filtreTexte) {
       var filtre = '';
       if (filtreTexte) {
           filtre = '((VAL1).toLowerCase().indexOf("' + filtreTexte + '".toLowerCase())) !== -1';
           for (var i = 2; i <= 10; i++) {
               filtre += ' || ' + '((VAL' + i + ').toLowerCase().indexOf("' + filtreTexte + '".toLowerCase())) !== -1';
           }
       }
       Aspectize.ExecuteCommand(aas.Services.Browser.UIService.SetCustomFilter(aas.ViewName.ConsultationExport.GridExportDatas, filtre));
   }
};


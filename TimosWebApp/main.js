/// <reference path="C:\www\\Aspectize.core\AspectizeIntellisenseLibrary.js" />

function Main(args) {
    Aspectize.App.Initialize(function () {

        Aspectize.ExecuteCommand(aas.Services.Browser.UIService.ShowView(aas.ViewName.Home));
        Aspectize.InitializeHistoryManager();
	});
}

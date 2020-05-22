/// <reference path="C:\www\\Aspectize.core\AspectizeIntellisenseLibrary.js" />

function jq(myid) {
    return myid.replace(/(:|;|\.|\[|\])/g, "\\$1");
}

function Main(args) {
    Aspectize.App.Initialize(function () {

        Aspectize.ExecuteCommand(aas.Services.Browser.UIService.ShowView(aas.ViewName.Home));
        Aspectize.InitializeHistoryManager();
	});
}

// Création de l'onglet Infos complémentaires
var vInfosSecondaires = Aspectize.CreateView("InfosSecondaires", aas.Controls.InfosSecondaires, "DetailTodoTabs.2:Infos", false, aas.Data.MainData.Todos.RelationTodoGroupeChamps.GroupeChamps);

//******************************** Configuration de la PropertyGrid en lecture seule *******************************************
vInfosSecondaires.GridChampsInfosSecondaires.BindList(vInfosSecondaires.ParentData.RelationTodoValeurChamp.TodoValeurChamp, "ValeurChamp", "LibelleChamp", "OrdreChamp", aas.Expression('InfosSecondaires'));
vInfosSecondaires.GridChampsInfosSecondaires.TypeTableName.BindData(vInfosSecondaires.ParentPath.RelationGroupeChampsChampsTimos.ChampTimos);
vInfosSecondaires.GridChampsInfosSecondaires.TypeTableNameColumn.BindData("LibelleConvivial");
vInfosSecondaires.GridChampsInfosSecondaires.TypeTableTypeColumn.BindData("AspectizeFieldType");
vInfosSecondaires.GridChampsInfosSecondaires.TypeTableControlTypeColumn.BindData("AspectizeControlType");
vInfosSecondaires.GridChampsInfosSecondaires.TypeTableFormatColumn.BindData(aas.Path.MainData.ChampTimos.FormatDate);
vInfosSecondaires.GridChampsInfosSecondaires.EnumValuesTableName.BindData(vInfosSecondaires.ParentPath.ValeursPossibles.ValeursChamp);
vInfosSecondaires.GridChampsInfosSecondaires.EnumValuesTableOptionTextColumn.BindData("DisplayedValue");
vInfosSecondaires.GridChampsInfosSecondaires.EnumValuesTableOptionValueColumn.BindData("StoredValue");
vInfosSecondaires.GridChampsInfosSecondaires.EnumValuesTableTypeColumn.BindData("ChampTimosId");
vInfosSecondaires.OnActivated.BindCommand(aas.Services.Browser.ClientTodosService.InitPropertyGrid(aas.ViewName.InfosSecondaires.GridChampsInfosSecondaires, false));


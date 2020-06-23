// Création de l'onglet Infos complémentaires
var vInfosSecondaires = Aspectize.CreateView("InfosSecondaires", aas.Controls.InformationsSecondaires, "DetailTodoTabs.2:Infos", false, aas.Data.MainData.Todos);

// Binding des groupes de champs secondaires
var vGroupeInfosSecondaires = Aspectize.CreateRepeatedView("GroupeChampsSecondaires", aas.Controls.GourpeInfosSecondaires, aas.Zones.InfosSecondaires.PanelInfosSecondaires, aas.Data.MainData.Todos.RelationTodoGroupeChamps.GroupeChamps, "", aas.Expression('InfosSecondaires'));
vGroupeInfosSecondaires.TitreInfosSecondaires.BindData(vGroupeInfosSecondaires.ParentData.Titre);

//******************************** Configuration de la PropertyGrid en lecture seule *******************************************
vGroupeInfosSecondaires.GridChampsInfosSecondaires.BindList(vGroupeInfosSecondaires.ParentData.RelationTodoValeurChamp.TodoValeurChamp, "ValeurChamp", "LibelleChamp", "OrdreChamp", aas.Expression('InfosSecondaires'));
vGroupeInfosSecondaires.GridChampsInfosSecondaires.TypeTableName.BindData(vGroupeInfosSecondaires.ParentPath.RelationGroupeChampsChampsTimos.ChampTimos);
vGroupeInfosSecondaires.GridChampsInfosSecondaires.TypeTableNameColumn.BindData("LibelleConvivial");
vGroupeInfosSecondaires.GridChampsInfosSecondaires.TypeTableTypeColumn.BindData("AspectizeFieldType");
vGroupeInfosSecondaires.GridChampsInfosSecondaires.TypeTableControlTypeColumn.BindData("AspectizeControlType");
vGroupeInfosSecondaires.GridChampsInfosSecondaires.TypeTableFormatColumn.BindData(aas.Path.MainData.ChampTimos.FormatDate);
vGroupeInfosSecondaires.GridChampsInfosSecondaires.TypeTableEditModeColumn.BindData(aas.Path.MainData.ChampTimos.Editable);
vGroupeInfosSecondaires.GridChampsInfosSecondaires.TypeTableClassColumn.BindData(aas.Path.MainData.ChampTimos.CustomClass);
vGroupeInfosSecondaires.GridChampsInfosSecondaires.EnumValuesTableName.BindData(vGroupeInfosSecondaires.ParentPath.ValeursPossibles.ValeursChamp);
vGroupeInfosSecondaires.GridChampsInfosSecondaires.EnumValuesTableOptionTextColumn.BindData("DisplayedValue");
vGroupeInfosSecondaires.GridChampsInfosSecondaires.EnumValuesTableOptionValueColumn.BindData("StoredValue");
vGroupeInfosSecondaires.GridChampsInfosSecondaires.EnumValuesTableTypeColumn.BindData("ChampTimosId");
vGroupeInfosSecondaires.GridChampsInfosSecondaires.OnEndRender.BindCommand(aas.Services.Browser.ClientTodosService.InitPropertyGrid(aas.Expression('GroupeChampsSecondaires:' + vGroupeInfosSecondaires.ParentData.TimosId + '-GridChampsInfosSecondaires')));



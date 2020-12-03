/*/ Création des caractéristiques
var vCaracteristiques = Aspectize.CreateRepeatedView("Caracteristique", aas.Controls.Caracteristique, aas.Zones.GroupeChamps.RepeaterPanelCaracteristiques,
    aas.Data.MainData.Todos.RelationTodoCaracteristique.Caracteristiques, '', '');
vCaracteristiques.LibelleCarac.BindData(vCaracteristiques.ParentData.Titre);

/// Initialisation de la property grid
vCaracteristiques.GridChampsCaracteristique.BindList(vCaracteristiques.ParentData.RelationCaracValeurChamp.CaracValeurChamp, "ValeurChamp", "LibelleChamp", "OrdreChamp");

vCaracteristiques.GridChampsCaracteristique.TypeTableName.BindData(vCaracteristiques.ParentPath.RelationCaracChamp.ChampTimos);
vCaracteristiques.GridChampsCaracteristique.TypeTableNameColumn.BindData("LibelleConvivial");
vCaracteristiques.GridChampsCaracteristique.TypeTableTypeColumn.BindData("AspectizeFieldType");
vCaracteristiques.GridChampsCaracteristique.TypeTableControlTypeColumn.BindData("AspectizeControlType");
vCaracteristiques.GridChampsCaracteristique.TypeTableFormatColumn.BindData(aas.Path.MainData.ChampTimos.FormatDate);
vCaracteristiques.GridChampsCaracteristique.TypeTableEditModeColumn.BindData(aas.Path.MainData.ChampTimos.Editable);
vCaracteristiques.GridChampsCaracteristique.TypeTableClassColumn.BindData(aas.Path.MainData.ChampTimos.CustomClass);

vCaracteristiques.GridChampsCaracteristique.EnumValuesTableName.BindData(vCaracteristiques.ParentPath.RelationCaracValeursPossibles.ValeursChamp);
vCaracteristiques.GridChampsCaracteristique.EnumValuesTableOptionTextColumn.BindData("DisplayedValue");
vCaracteristiques.GridChampsCaracteristique.EnumValuesTableOptionValueColumn.BindData("StoredValue");
vCaracteristiques.GridChampsCaracteristique.EnumValuesTableTypeColumn.BindData("ChampTimosId");

vCaracteristiques.GridChampsCaracteristique.OnEndRender.BindCommand(aas.Services.Browser.ClientTodosService.InitPropertyGrid(aas.Expression('Caracteristique:' + vCaracteristiques.ParentData.TimosId + '-GridChampsCaracteristique')));
//*/
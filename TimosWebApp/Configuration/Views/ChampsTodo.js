// Création de l'onglet Champs Todo
var vChampsTodo = Aspectize.CreateView("ChampsTodo", aas.Controls.ChampsTodo, "DetailTodoTabs.0:Champs", true, aas.Data.MainData.Todos);

var vTabsGroupesChamps = Aspectize.CreateRepeatedView("GroupeNavTab", aas.Controls.GroupeNavTab, aas.Zones.ChampsTodo.PanelGroupesNavTabs, aas.Data.MainData.Todos.RelationTodoGroupeChamps.GroupeChamps, "", aas.Expression('!InfosSecondaires'));
vTabsGroupesChamps.TitreGroupe.BindData(vTabsGroupesChamps.ParentData.Titre);
vTabsGroupesChamps.IdGroupe.BindData(vTabsGroupesChamps.ParentData.TimosId);

// Binding des groupes de champs
var vGroupeChamps = Aspectize.CreateRepeatedView("GroupeChamps", aas.Controls.GroupeChamps, aas.Zones.ChampsTodo.PanelGroupesChamps, aas.Data.MainData.Todos.RelationTodoGroupeChamps.GroupeChamps, "", aas.Expression('!InfosSecondaires'));
vGroupeChamps.TitreGroupe.BindData(vGroupeChamps.ParentData.Titre);
vGroupeChamps.IdGroupe.BindData(vGroupeChamps.ParentData.TimosId);
vGroupeChamps.BoutonEditionTodo.click.BindCommand(aas.Services.Browser.BootStrapClientService.ShowModal(aas.ViewName.EditionTodo, true, false, true));
//vGroupeChamps.BoutonEditionTodo.click.BindCommand(aas.Services.Browser.SystemServices.Alert(vGroupeChamps.ParentData.RelationTodoGroupeChamps.Todos.Label));
//vGroupeChamps.CollapseGroupe.click.BindCommand(aas.Services.Browser.ClientTodosService.ExpandGroup(vGroupeChamps.ParentData.TimosId));
vGroupeChamps.IsActiveIn.BindData(aas.Expression(IIF(vGroupeChamps.ParentData.Expand, 'active in', '')));
//vGroupeChamps.FaCaretClass.BindData(aas.Expression(IIF(vGroupeChamps.ParentData.Expand, 'fa-caret-down', 'fa-caret-right')));
//vGroupeChamps.SectionTitreGroupe.BindData(aas.Expression(IIF(vGroupeChamps.ParentData.Expand, 'section-titre-expanded', 'section-titre-collapsed')));

//******************************** Configuration de la PropertyGrid en lecture seule *******************************************
vGroupeChamps.GridChampsTodo.BindList(vGroupeChamps.ParentData.RelationTodoValeurChamp.TodoValeurChamp, "ValeurChamp", "LibelleChamp", "OrdreChamp");
vGroupeChamps.GridChampsTodo.TypeTableName.BindData(vGroupeChamps.ParentPath.RelationGroupeChampsChampsTimos.ChampTimos);
vGroupeChamps.GridChampsTodo.TypeTableNameColumn.BindData("LibelleConvivial");
vGroupeChamps.GridChampsTodo.TypeTableTypeColumn.BindData("AspectizeFieldType");
vGroupeChamps.GridChampsTodo.TypeTableControlTypeColumn.BindData("AspectizeControlType");
vGroupeChamps.GridChampsTodo.TypeTableFormatColumn.BindData(aas.Path.MainData.ChampTimos.FormatDate);
vGroupeChamps.GridChampsTodo.TypeTableEditModeColumn.BindData(aas.Path.MainData.ChampTimos.Editable);
vGroupeChamps.GridChampsTodo.TypeTableClassColumn.BindData(aas.Path.MainData.ChampTimos.CustomClass);
vGroupeChamps.GridChampsTodo.EnumValuesTableName.BindData(vGroupeChamps.ParentPath.ValeursPossibles.ValeursChamp);
vGroupeChamps.GridChampsTodo.EnumValuesTableOptionTextColumn.BindData("DisplayedValue");
vGroupeChamps.GridChampsTodo.EnumValuesTableOptionValueColumn.BindData("StoredValue");
vGroupeChamps.GridChampsTodo.EnumValuesTableTypeColumn.BindData("ChampTimosId");
vGroupeChamps.GridChampsTodo.OnEndRender.BindCommand(aas.Services.Browser.ClientTodosService.InitPropertyGrid(aas.Expression('GroupeChamps:' + vGroupeChamps.ParentData.TimosId + '-GridChampsTodo')));

// Configuration du controle d'édition d'un todo en modal
var vEditionTodo = Aspectize.CreateView("EditionTodo", aas.Controls.EditionTodo, "", false, aas.Data.MainData.Todos.RelationTodoGroupeChamps.GroupeChamps);
vEditionTodo.OnActivated.BindCommand(aas.Services.Browser.DataRecorder.Start(aas.Data.MainData));
//vEditionTodo.LabelToDo.BindData(vEditionTodo.ParentData.RelationTodoGroupeChamps.Todos.Label); // Ne fonctionne pas alors qu'il devrait
vEditionTodo.LabelToDo.BindData(aas.Data.MainData.Todos.Label); // Ce binding fonctionne bien
vEditionTodo.BtnCancel.click.BindCommand(aas.Services.Browser.BootStrapClientService.CloseModal(aas.ViewName.EditionTodo));
vEditionTodo.BtnCancel.click.BindCommand(aas.Services.Browser.DataRecorder.CancelRowChanges(aas.Data.MainData));
vEditionTodo.BtnSave.click.BindCommand(aas.Services.Server.TodosService.SaveTodo(
    aas.Data.MainData,
    vEditionTodo.ParentData.RelationTodoGroupeChamps.Todos.TimosId,
    vEditionTodo.ParentData.RelationTodoGroupeChamps.Todos.ElementType,
    vEditionTodo.ParentData.RelationTodoGroupeChamps.Todos.ElementId), "", false, true);
vEditionTodo.BtnSave.click.BindCommand(aas.Services.Browser.BootStrapClientService.CloseModal(aas.ViewName.EditionTodo));

// Configuration de la PropertyGrid en mode édition
vEditionTodo.GridChampsTodo.BindList(vGroupeChamps.ParentData.RelationTodoValeurChamp.TodoValeurChamp, "ValeurChamp", "LibelleChamp", "OrdreChamp");
vEditionTodo.GridChampsTodo.TypeTableName.BindData(vGroupeChamps.ParentPath.RelationGroupeChampsChampsTimos.ChampTimos);
vEditionTodo.GridChampsTodo.TypeTableNameColumn.BindData("LibelleConvivial");
vEditionTodo.GridChampsTodo.TypeTableTypeColumn.BindData("AspectizeFieldType");
vEditionTodo.GridChampsTodo.TypeTableControlTypeColumn.BindData("AspectizeControlType");
vEditionTodo.GridChampsTodo.TypeTableFormatColumn.BindData(aas.Path.MainData.ChampTimos.FormatDate);
vEditionTodo.GridChampsTodo.TypeTableEditModeColumn.BindData(aas.Path.MainData.ChampTimos.Editable);
vEditionTodo.GridChampsTodo.TypeTableClassColumn.BindData(aas.Path.MainData.ChampTimos.CustomClass);
vEditionTodo.GridChampsTodo.EnumValuesTableName.BindData(vGroupeChamps.ParentPath.ValeursPossibles.ValeursChamp);
vEditionTodo.GridChampsTodo.EnumValuesTableOptionTextColumn.BindData("DisplayedValue");
vEditionTodo.GridChampsTodo.EnumValuesTableOptionValueColumn.BindData("StoredValue");
vEditionTodo.GridChampsTodo.EnumValuesTableTypeColumn.BindData("ChampTimosId");
vEditionTodo.GridChampsTodo.EditMode.BindData(true);
vEditionTodo.GridChampsTodo.OnEndRender.BindCommand(aas.Services.Browser.ClientTodosService.InitPropertyGrid(aas.ViewName.EditionTodo.GridChampsTodo));


// Création des caractéristiques
var vCaracteristiques = Aspectize.CreateRepeatedView("Caracteristique", aas.Controls.Caracteristique, aas.Zones.GroupeChamps.RepeaterPanelCaracteristiques,
    aas.Data.MainData.Todos.RelationTodoCaracteristique.Caracteristiques, '', aas.Expression('IdGroupePourFiltre === ' + vGroupeChamps.ParentData.TimosId));
//*/
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

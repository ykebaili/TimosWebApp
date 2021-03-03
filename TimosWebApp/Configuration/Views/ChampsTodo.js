// Création de l'onglet Champs Todo
var vChampsTodo = Aspectize.CreateView("ChampsTodo", aas.Controls.ChampsTodo, "DetailTodoTabs.0:Champs", true, aas.Data.MainData.Todos);

var vTabsGroupesChamps = Aspectize.CreateRepeatedView("GroupeNavTab", aas.Controls.GroupeNavTab, aas.Zones.ChampsTodo.PanelGroupesNavTabs, aas.Data.MainData.Todos.RelationTodoGroupeChamps.GroupeChamps, "", aas.Expression('!InfosSecondaires'));
vTabsGroupesChamps.TitreGroupe.BindData(vTabsGroupesChamps.ParentData.Titre);
vTabsGroupesChamps.IdGroupe.BindData(vTabsGroupesChamps.ParentData.TimosId);

// Binding des groupes de champs
var vGroupeChamps = Aspectize.CreateRepeatedView("GroupeChamps", aas.Controls.GroupeChamps, aas.Zones.ChampsTodo.PanelGroupesChamps, aas.Data.MainData.Todos.RelationTodoGroupeChamps.GroupeChamps, "", aas.Expression('!InfosSecondaires'));
vGroupeChamps.TitreGroupe.BindData(vGroupeChamps.ParentData.Titre);
vGroupeChamps.TitreCaracterisiques.BindData(vGroupeChamps.ParentData.TitreCaracteristiques);
vGroupeChamps.IdGroupe.BindData(vGroupeChamps.ParentData.TimosId);
vGroupeChamps.IsActiveIn.BindData(aas.Expression(IIF(vGroupeChamps.ParentData.Expand, 'active in', '')));
vGroupeChamps.BoutonEditionTodo.click.BindCommand(aas.Services.Browser.BootStrapClientService.ShowModal(aas.ViewName.EditionTodo, false, false, true));
vGroupeChamps.BoutonAjouterCarac.click.BindCommand(aas.Services.Browser.ClientTodosService.AddCaracteristic(aas.Data.MainData.Todos.TimosId, vGroupeChamps.ParentData.TimosId));
vGroupeChamps.DisplayBtnAddCarac.BindData(aas.Expression(IIF(vGroupeChamps.ParentData.CanAddCaracteristiques, '', 'hidden')));
//vGroupeChamps.BoutonEditionTodo.click.BindCommand(aas.Services.Browser.SystemServices.Alert(vGroupeChamps.ParentData.RelationTodoGroupeChamps.Todos.Label));
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
var vEditionTodo = Aspectize.CreateView("EditionTodo", aas.Controls.EditionChamps, "", false, aas.Data.MainData.Todos.RelationTodoGroupeChamps.GroupeChamps);
vEditionTodo.OnActivated.BindCommand(aas.Services.Browser.DataRecorder.Start(aas.Data.MainData));
//vEditionTodo.LabelToDo.BindData(vEditionTodo.ParentData.RelationTodoGroupeChamps.Todos.Label); // Ne fonctionne pas alors qu'il devrait
vEditionTodo.LabelElementEdite.BindData(aas.Data.MainData.Todos.Label); // Ce binding fonctionne bien
vEditionTodo.BtnCancel.click.BindCommand(aas.Services.Browser.BootStrapClientService.CloseModal(aas.ViewName.EditionTodo));
vEditionTodo.BtnCancel.click.BindCommand(aas.Services.Browser.DataRecorder.CancelRowChanges(aas.Data.MainData));
vEditionTodo.BtnSave.click.BindCommand(aas.Services.Server.TodosService.SaveTodo(
    aas.Data.MainData,
    vEditionTodo.ParentData.RelationTodoGroupeChamps.Todos.TimosId,
    vEditionTodo.ParentData.RelationTodoGroupeChamps.Todos.ElementType,
    vEditionTodo.ParentData.RelationTodoGroupeChamps.Todos.ElementId), "", false, true);
vEditionTodo.BtnSave.click.BindCommand(aas.Services.Browser.BootStrapClientService.CloseModal(aas.ViewName.EditionTodo));
// Configuration de la PropertyGrid en mode édition
vEditionTodo.GridChamps.BindList(vGroupeChamps.ParentData.RelationTodoValeurChamp.TodoValeurChamp, "ValeurChamp", "LibelleChamp", "OrdreChamp", aas.Expression('!UseAutoComplete'));
vEditionTodo.GridChamps.TypeTableName.BindData(vGroupeChamps.ParentPath.RelationGroupeChampsChampsTimos.ChampTimos);
vEditionTodo.GridChamps.TypeTableNameColumn.BindData("LibelleConvivial");
vEditionTodo.GridChamps.TypeTableTypeColumn.BindData("AspectizeFieldType");
vEditionTodo.GridChamps.TypeTableControlTypeColumn.BindData("AspectizeControlType");
vEditionTodo.GridChamps.TypeTableFormatColumn.BindData(aas.Path.MainData.ChampTimos.FormatDate);
vEditionTodo.GridChamps.TypeTableEditModeColumn.BindData(aas.Path.MainData.ChampTimos.Editable);
vEditionTodo.GridChamps.TypeTableClassColumn.BindData(aas.Path.MainData.ChampTimos.CustomClass);
vEditionTodo.GridChamps.EnumValuesTableName.BindData(vGroupeChamps.ParentPath.ValeursPossibles.ValeursChamp);
vEditionTodo.GridChamps.EnumValuesTableOptionTextColumn.BindData("DisplayedValue");
vEditionTodo.GridChamps.EnumValuesTableOptionValueColumn.BindData("StoredValue");
vEditionTodo.GridChamps.EnumValuesTableTypeColumn.BindData("ChampTimosId");
vEditionTodo.GridChamps.EditMode.BindData(true);
vEditionTodo.GridChamps.OnEndRender.BindCommand(aas.Services.Browser.ClientTodosService.InitPropertyGrid(aas.ViewName.EditionTodo.GridChamps));
// Auto complete
vEditionTodo.LibelleChampAutoComplete.BindData(vEditionTodo.ParentData.LibelleChampAutoComplete);
vEditionTodo.AutoCompleteValeurChamp.OnNeedData.BindCommand(aas.Services.Server.TodosService.GetDatasList('', vEditionTodo.ParentData.IdChampAutoComplete));
vEditionTodo.AutoCompleteValeurChamp.OnItemSelected.BindCommand(aas.Services.Browser.ClientTodosService.SelectDataFromList('', vEditionTodo.ParentData.IdChampAutoComplete, vEditionTodo.ParentData.TimosId, -1));
vEditionTodo.AutoCompleteValeurChamp.Custom.BindData(false);
vEditionTodo.AutoCompleteValeurChamp.FillSelected.BindData(true);


// Création des caractéristiques
var vCaracteristiques = Aspectize.CreateRepeatedView("Caracteristique", aas.Controls.Caracteristique, aas.Zones.GroupeChamps.RepeaterPanelCaracteristiques,
    aas.Data.MainData.Todos.RelationTodoCaracteristique.Caracteristiques, '',
    aas.Expression('IdGroupePourFiltre === ' + vGroupeChamps.ParentData.TimosId + ' && !IsTemplate'));
//*/
vCaracteristiques.LibelleCarac.BindData(vCaracteristiques.ParentData.Titre);
vCaracteristiques.BoutonEditerCarac.click.BindCommand(aas.Services.Browser.BootStrapClientService.ShowModal(aas.ViewName.EditionCarac, false, false, true));
vCaracteristiques.BoutonEditerCarac.click.BindCommand(aas.Services.Browser.DataRecorder.Start(aas.Data.MainData));
vCaracteristiques.BoutonSupprimerCarac.click.BindCommand(aas.Services.Browser.ClientTodosService.DeleteCaracteristc(vCaracteristiques.ParentData.TimosId, vCaracteristiques.ParentData.ElementType));
//vCaracteristiques.DisplayBtnDeleteCarac.BindData(aas.Expression(IIF(aas.ViewName.GroupeChamps.DisplayBtnAddCarac + ' == hidden', 'hidden', '')));
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
vCaracteristiques.GridChampsCaracteristique.OnEndRender.BindCommand(aas.Services.Browser.ClientTodosService.InitPropertyGrid(aas.Expression('Caracteristique:' + vCaracteristiques.ParentData.Id + '-GridChampsCaracteristique')));
//*/

// Configuration du controle d'édition d'une Caractéristique en modal
var vEditionCarac = Aspectize.CreateView("EditionCarac", aas.Controls.EditionChamps, "", false, aas.Data.MainData.Todos.RelationTodoCaracteristique.Caracteristiques);
//vEditionCarac.OnActivated.BindCommand(aas.Services.Browser.DataRecorder.Start(aas.Data.MainData));
vEditionCarac.LabelElementEdite.BindData(aas.Data.MainData.Todos.Label); // Ce binding fonctionne bien
vEditionCarac.BtnCancel.click.BindCommand(aas.Services.Browser.BootStrapClientService.CloseModal(aas.ViewName.EditionCarac));
vEditionCarac.BtnCancel.click.BindCommand(aas.Services.Browser.DataRecorder.CancelRowChanges(aas.Data.MainData));
vEditionCarac.BtnSave.click.BindCommand(aas.Services.Browser.ClientTodosService.SaveCaracteristic(
    aas.Data.MainData,
    vEditionCarac.ParentData.TimosId,
    vEditionCarac.ParentData.ElementType,
    vEditionCarac.ParentData.RelationTodoCaracteristique.Todos.TimosId));

// Configuration de la PropertyGrid en mode édition
vEditionCarac.GridChamps.BindList(vEditionCarac.ParentData.RelationCaracValeurChamp.CaracValeurChamp, "ValeurChamp", "LibelleChamp", "OrdreChamp");
vEditionCarac.GridChamps.TypeTableName.BindData(vEditionCarac.ParentPath.RelationCaracChamp.ChampTimos);
vEditionCarac.GridChamps.TypeTableNameColumn.BindData("LibelleConvivial");
vEditionCarac.GridChamps.TypeTableTypeColumn.BindData("AspectizeFieldType");
vEditionCarac.GridChamps.TypeTableControlTypeColumn.BindData("AspectizeControlType");
vEditionCarac.GridChamps.TypeTableFormatColumn.BindData(aas.Path.MainData.ChampTimos.FormatDate);
vEditionCarac.GridChamps.TypeTableEditModeColumn.BindData(aas.Path.MainData.ChampTimos.Editable);
vEditionCarac.GridChamps.TypeTableClassColumn.BindData(aas.Path.MainData.ChampTimos.CustomClass);
vEditionCarac.GridChamps.EnumValuesTableName.BindData(vEditionCarac.ParentPath.RelationCaracValeursPossibles.ValeursChamp);
vEditionCarac.GridChamps.EnumValuesTableOptionTextColumn.BindData("DisplayedValue");
vEditionCarac.GridChamps.EnumValuesTableOptionValueColumn.BindData("StoredValue");
vEditionCarac.GridChamps.EnumValuesTableTypeColumn.BindData("ChampTimosId");
vEditionCarac.GridChamps.EditMode.BindData(true);
vEditionCarac.GridChamps.OnEndRender.BindCommand(aas.Services.Browser.ClientTodosService.InitPropertyGrid(aas.ViewName.EditionCarac.GridChamps));


//*/
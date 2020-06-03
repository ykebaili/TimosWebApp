// Création de l'onglet Champs Todo
var vChampsTodo = Aspectize.CreateView("ChampsTodo", aas.Controls.ChampsTodo, "DetailTodoTabs.0:Champs", true, aas.Data.MainData.Todos);

// Binding des groupes de champs
var vGroupeChamps = Aspectize.CreateRepeatedView("GroupeChamps", aas.Controls.GroupeChamps, aas.Zones.ChampsTodo.PanelGroupesChamps, aas.Data.MainData.Todos.RelationTodoGroupeChamps.GroupeChamps);
vGroupeChamps.TitreGroupe.BindData(vGroupeChamps.ParentData.Titre);
vGroupeChamps.BoutonEditionTodo.click.BindCommand(aas.Services.Browser.BootStrapClientService.ShowModal(aas.ViewName.EditionTodo, true, false, true));
vGroupeChamps.CollapseGroupe.click.BindCommand(aas.Services.Browser.ClientTodosService.ExpandGroup(vGroupeChamps.ParentData.TimosId));
vGroupeChamps.DisplayExpand.BindData(aas.Expression(IIF(vGroupeChamps.ParentData.Expand, '', 'hidden')));
vGroupeChamps.FaCaretClass.BindData(aas.Expression(IIF(vGroupeChamps.ParentData.Expand, 'fa-caret-down', 'fa-caret-right')));
vGroupeChamps.SectionTitreGroupe.BindData(aas.Expression(IIF(vGroupeChamps.ParentData.Expand, 'section-titre-expanded', 'section-titre-collapsed')));

//******************************** Configuration de la PropertyGrid en lecture seule *******************************************
vGroupeChamps.GridChampsTodo.BindList(vGroupeChamps.ParentData.RelationTodoValeurChamp.TodoValeurChamp, "ValeurChamp", "LibelleChamp", "OrdreChamp");
vGroupeChamps.GridChampsTodo.TypeTableName.BindData(vGroupeChamps.ParentPath.RelationGroupeChampsChampsTimos.ChampTimos);
vGroupeChamps.GridChampsTodo.TypeTableNameColumn.BindData("LibelleConvivial");
vGroupeChamps.GridChampsTodo.TypeTableTypeColumn.BindData("AspectizeFieldType");
vGroupeChamps.GridChampsTodo.TypeTableControlTypeColumn.BindData("AspectizeControlType");
vGroupeChamps.GridChampsTodo.TypeTableFormatColumn.BindData(aas.Path.MainData.ChampTimos.FormatDate);
vGroupeChamps.GridChampsTodo.EnumValuesTableName.BindData(vGroupeChamps.ParentPath.ValeursPossibles.ValeursChamp);
vGroupeChamps.GridChampsTodo.EnumValuesTableOptionTextColumn.BindData("DisplayedValue");
vGroupeChamps.GridChampsTodo.EnumValuesTableOptionValueColumn.BindData("StoredValue");
vGroupeChamps.GridChampsTodo.EnumValuesTableTypeColumn.BindData("ChampTimosId");
vGroupeChamps.OnActivated.BindCommand(aas.Services.Browser.ClientTodosService.InitPropertyGrid(aas.ViewName.GroupeChamps.GridChampsTodo, false));


// Configuration du controle d'édition d'un todo en modal
var vEditionTodo = Aspectize.CreateView("EditionTodo", aas.Controls.EditionTodo, "", false, aas.Data.MainData.Todos.RelationTodoGroupeChamps.GroupeChamps);
vEditionTodo.OnActivated.BindCommand(aas.Services.Browser.DataRecorder.Start(aas.Data.MainData));
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
vEditionTodo.GridChampsTodo.EnumValuesTableName.BindData(vGroupeChamps.ParentPath.ValeursPossibles.ValeursChamp);
vEditionTodo.GridChampsTodo.EnumValuesTableOptionTextColumn.BindData("DisplayedValue");
vEditionTodo.GridChampsTodo.EnumValuesTableOptionValueColumn.BindData("StoredValue");
vEditionTodo.GridChampsTodo.EnumValuesTableTypeColumn.BindData("ChampTimosId");
vEditionTodo.GridChampsTodo.EditMode.BindData(true);
vEditionTodo.OnActivated.BindCommand(aas.Services.Browser.ClientTodosService.InitPropertyGrid(aas.ViewName.EditionTodo.GridChampsTodo, true));

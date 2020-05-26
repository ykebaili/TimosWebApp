//  Liste des todos
var vListeTodos = Aspectize.CreateView("ListeTodos", aas.Controls.ListeTodos, aas.Zones.Home.ZoneInfo, true);
vListeTodos.OnActivated.BindCommand(aas.Services.Browser.History.PushState(aas.ViewName.ListeTodos));
vListeTodos.GridListeTodos.BindGrid(aas.Data.MainData.Todos);
vListeTodos.GridListeTodos.className.BindData('table-condensed');
vListeTodos.GridListeTodos.OnRowClick.BindCommand(aas.Services.Server.TodosService.GetTodoDetails(aas.Data.MainData.Todos.TimosId), aas.Data.MainData, true, true);
vListeTodos.GridListeTodos.OnRowClick.BindCommand(aas.Services.Browser.UIService.ShowView(aas.ViewName.DetailTodo));

// Colonnes de la liste des todos
var colLabael = vListeTodos.GridListeTodos.AddGridColumn("libelle", aas.ColumnType.Span);
colLabael.Text.BindData(vListeTodos.GridListeTodos.DataSource.Label);
colLabael.HeaderText.BindData("Libellé du todo");
var colStartDate = vListeTodos.GridListeTodos.AddGridColumn("startdate", aas.ColumnType.Span);
colStartDate.Text.BindData(vListeTodos.GridListeTodos.DataSource.StartDate, "dd/MM/yyyy");
colStartDate.HeaderText.BindData("Date début");

// Filtre et compteur
vListeTodos.ChampFiltreLabel.keyup.BindCommand(aas.Services.Browser.ClientTodosService.FiltreTodos(vListeTodos.ChampFiltreLabel.value));
vListeTodos.CompteurTodos.BindData(vListeTodos.GridListeTodos.RowCount);
vListeTodos.TotalTodos.BindData(aas.Services.Browser.DataService.Count(aas.Data.MainData.Todos));

// Vue détaillée d'un todo
var vDetailTodo = Aspectize.CreateView("DetailTodo", aas.Controls.DetailTodo, aas.Zones.Home.ZoneInfo, false, aas.Data.MainData.Todos);
vDetailTodo.OnActivated.BindCommand(aas.Services.Browser.History.PushState(aas.ViewName.DetailTodo, aas.Path.MainData.Todos, vDetailTodo.ParentData.TimosId));
vDetailTodo.LabelToDo.BindData(vDetailTodo.ParentData.Label);
vDetailTodo.DescriptionElementEdite.BindData(vDetailTodo.ParentData.ElementDescription);
vDetailTodo.DateDebutTodo.BindData(vDetailTodo.ParentData.StartDate);
vDetailTodo.InstrictionsTodo.BindData(vDetailTodo.ParentData.Instructions);
vDetailTodo.BoutonTerminerTodo.click.BindCommand(aas.Services.Server.TodosService.EndTodo(vDetailTodo.ParentData.TimosId));

// Gestion des onglets
var vDetailTodoTab = Aspectize.CreateView("DetailTodoTabs", aas.Controls.Bootstrap.BootstrapTab, aas.Zones.DetailTodo.ZoneOnglets, true);
vDetailTodoTab.className.BindData(aas.Expression(IIF(aas.Data.MainData.Todos.RelationTodoDocument.DocumentsAttendus.TimosId, 'display-documents', '')));
var vChampsTodo = Aspectize.CreateView("ChampsTodo", aas.Controls.ChampsTodo, "DetailTodoTabs.0:Champs", true, aas.Data.MainData.Todos);
vChampsTodo.BoutonEditionTodo.click.BindCommand(aas.Services.Browser.BootStrapClientService.ShowModal(aas.ViewName.EditionTodo, true, false, true));

// Configuration de la PropertyGrid en lecture seule
vChampsTodo.GridChampsTodo.BindList(vDetailTodo.ParentData.RelationTodoValeurChamp.TodoValeurChamp, "ValeurChamp", "LibelleChamp", "OrdreChamp");
vChampsTodo.GridChampsTodo.TypeTableName.BindData(vDetailTodo.ParentPath.RelationTodoDefinitionChamp.ChampTimos);
vChampsTodo.GridChampsTodo.TypeTableNameColumn.BindData("LibelleConvivial");
vChampsTodo.GridChampsTodo.TypeTableTypeColumn.BindData("AspectizeFieldType");
vChampsTodo.GridChampsTodo.TypeTableControlTypeColumn.BindData("AspectizeControlType");
vChampsTodo.GridChampsTodo.TypeTableFormatColumn.BindData(aas.Path.MainData.ChampTimos.FormatDate);
vChampsTodo.GridChampsTodo.EnumValuesTableName.BindData(vDetailTodo.ParentPath.ValeursPossibles.ValeursChamp);
vChampsTodo.GridChampsTodo.EnumValuesTableOptionTextColumn.BindData("DisplayedValue");
vChampsTodo.GridChampsTodo.EnumValuesTableOptionValueColumn.BindData("StoredValue");
vChampsTodo.GridChampsTodo.EnumValuesTableTypeColumn.BindData("ChampTimosId");
vChampsTodo.OnActivated.BindCommand(aas.Services.Browser.ClientTodosService.InitPropertyGrid(aas.ViewName.ChampsTodo.GridChampsTodo, false));


// Configuration du controle d'édition d'un todo en modal
var vEditionTodo = Aspectize.CreateView("EditionTodo", aas.Controls.EditionTodo, "", false, aas.Data.MainData.Todos);
vEditionTodo.OnActivated.BindCommand(aas.Services.Browser.DataRecorder.Start(aas.Data.MainData));
vEditionTodo.BtnCancel.click.BindCommand(aas.Services.Browser.BootStrapClientService.CloseModal(aas.ViewName.EditionTodo));
vEditionTodo.BtnCancel.click.BindCommand(aas.Services.Browser.DataRecorder.CancelRowChanges(aas.Data.MainData));
vEditionTodo.BtnSave.click.BindCommand(aas.Services.Server.TodosService.SaveTodo(aas.Data.MainData, vEditionTodo.ParentData.TimosId, vEditionTodo.ParentData.ElementType, vEditionTodo.ParentData.ElementId), "", false, true);
vEditionTodo.BtnSave.click.BindCommand(aas.Services.Browser.BootStrapClientService.CloseModal(aas.ViewName.EditionTodo));

// Configuration de la PropertyGrid en mode édition
vEditionTodo.GridChampsTodo.BindList(vDetailTodo.ParentData.RelationTodoValeurChamp.TodoValeurChamp, "ValeurChamp", "LibelleChamp", "OrdreChamp");
vEditionTodo.GridChampsTodo.TypeTableName.BindData(vDetailTodo.ParentPath.RelationTodoDefinitionChamp.ChampTimos);
vEditionTodo.GridChampsTodo.TypeTableNameColumn.BindData("LibelleConvivial");
vEditionTodo.GridChampsTodo.TypeTableTypeColumn.BindData("AspectizeFieldType");
vEditionTodo.GridChampsTodo.TypeTableControlTypeColumn.BindData("AspectizeControlType");
vEditionTodo.GridChampsTodo.TypeTableFormatColumn.BindData(aas.Path.MainData.ChampTimos.FormatDate);
vEditionTodo.GridChampsTodo.EnumValuesTableName.BindData(vDetailTodo.ParentPath.ValeursPossibles.ValeursChamp);
vEditionTodo.GridChampsTodo.EnumValuesTableOptionTextColumn.BindData("DisplayedValue");
vEditionTodo.GridChampsTodo.EnumValuesTableOptionValueColumn.BindData("StoredValue");
vEditionTodo.GridChampsTodo.EnumValuesTableTypeColumn.BindData("ChampTimosId");
vEditionTodo.GridChampsTodo.EditMode.BindData(true);
vEditionTodo.OnActivated.BindCommand(aas.Services.Browser.ClientTodosService.InitPropertyGrid(aas.ViewName.EditionTodo.GridChampsTodo, true));

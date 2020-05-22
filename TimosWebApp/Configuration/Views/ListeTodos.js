var listeTodos = Aspectize.CreateView("ListeTodos", aas.Controls.ListeTodos, aas.Zones.Home.ZoneInfo, true);
listeTodos.OnActivated.BindCommand(aas.Services.Browser.History.PushState(aas.ViewName.ListeTodos));
listeTodos.GridListeTodos.BindGrid(aas.Data.MainData.Todos);
listeTodos.GridListeTodos.className.BindData('table-condensed');
listeTodos.GridListeTodos.OnRowClick.BindCommand(aas.Services.Server.TodosService.GetTodoDetails(aas.Data.MainData.Todos.TimosId), aas.Data.MainData, true, true);
listeTodos.GridListeTodos.OnRowClick.BindCommand(aas.Services.Browser.UIService.ShowView(aas.ViewName.DetailTodo));
var cLabael = listeTodos.GridListeTodos.AddGridColumn("libelle", aas.ColumnType.Span);
cLabael.Text.BindData(listeTodos.GridListeTodos.DataSource.Label);
cLabael.HeaderText.BindData("Libellé du todo");
var cStartDate = listeTodos.GridListeTodos.AddGridColumn("startdate", aas.ColumnType.Span);
cStartDate.Text.BindData(listeTodos.GridListeTodos.DataSource.StartDate, "dd/MM/yyyy");
cStartDate.HeaderText.BindData("Date début");

listeTodos.ChampFiltreLabel.keyup.BindCommand(aas.Services.Browser.ClientTodosService.FiltreTodos(listeTodos.ChampFiltreLabel.value));
listeTodos.CompteurTodos.BindData(listeTodos.GridListeTodos.RowCount);
listeTodos.TotalTodos.BindData(aas.Services.Browser.DataService.Count(aas.Data.MainData.Todos));

// Gestion des onglets
var detailTodoTab = Aspectize.CreateView("DetailTodoTabs", aas.Controls.Bootstrap.BootstrapTab, aas.Zones.DetailTodo.ZoneOnglets, true);

var detailTodo = Aspectize.CreateView("DetailTodo", aas.Controls.DetailTodo, aas.Zones.Home.ZoneInfo, false, aas.Data.MainData.Todos);
detailTodo.OnActivated.BindCommand(aas.Services.Browser.History.PushState(aas.ViewName.DetailTodo, aas.Path.MainData.Todos, detailTodo.ParentData.TimosId));
detailTodo.LabelToDo.BindData(detailTodo.ParentData.Label);
detailTodo.DescriptionElementEdite.BindData(detailTodo.ParentData.ElementDescription);
detailTodo.DateDebutTodo.BindData(detailTodo.ParentData.StartDate);
detailTodo.InstrictionsTodo.BindData(detailTodo.ParentData.Instructions);
detailTodo.BoutonTerminerTodo.click.BindCommand(aas.Services.Server.TodosService.EndTodo(detailTodo.ParentData.TimosId));

var champsTodo = Aspectize.CreateView("ChampsTodo", aas.Controls.ChampsTodo, "DetailTodoTabs.0:Champs", true, aas.Data.MainData.Todos);
champsTodo.BoutonEditionTodo.click.BindCommand(aas.Services.Browser.BootStrapClientService.ShowModal(aas.ViewName.EditionTodo, true, false, true));


// Configuration de la PropertyGrid en lecture seule
champsTodo.GridChampsTodo.BindList(detailTodo.ParentData.RelationTodoValeurChamp.TodoValeurChamp, "ValeurChamp", "LibelleChamp", "OrdreChamp");
champsTodo.GridChampsTodo.TypeTableName.BindData(detailTodo.ParentPath.RelationTodoDefinitionChamp.ChampTimos);
champsTodo.GridChampsTodo.TypeTableNameColumn.BindData("LibelleConvivial");
champsTodo.GridChampsTodo.TypeTableTypeColumn.BindData("AspectizeFieldType");
champsTodo.GridChampsTodo.TypeTableControlTypeColumn.BindData("AspectizeControlType");
champsTodo.GridChampsTodo.TypeTableFormatColumn.BindData(aas.Path.MainData.ChampTimos.FormatDate);
champsTodo.GridChampsTodo.EnumValuesTableName.BindData(detailTodo.ParentPath.ValeursPossibles.ValeursChamp);
champsTodo.GridChampsTodo.EnumValuesTableOptionTextColumn.BindData("DisplayedValue");
champsTodo.GridChampsTodo.EnumValuesTableOptionValueColumn.BindData("StoredValue");
champsTodo.GridChampsTodo.EnumValuesTableTypeColumn.BindData("ChampTimosId");
champsTodo.OnActivated.BindCommand(aas.Services.Browser.ClientTodosService.InitPropertyGrid(aas.ViewName.ChampsTodo.GridChampsTodo, false));

// Configuration du controle d'édition d'un todo en modal
var editionTodo = Aspectize.CreateView("EditionTodo", aas.Controls.EditionTodo, "", false, aas.Data.MainData.Todos);
editionTodo.OnActivated.BindCommand(aas.Services.Browser.DataRecorder.Start(aas.Data.MainData));
editionTodo.BtnCancel.click.BindCommand(aas.Services.Browser.BootStrapClientService.CloseModal(aas.ViewName.EditionTodo));
editionTodo.BtnCancel.click.BindCommand(aas.Services.Browser.DataRecorder.CancelRowChanges(aas.Data.MainData));
editionTodo.BtnSave.click.BindCommand(aas.Services.Server.TodosService.SaveTodo(aas.Data.MainData, editionTodo.ParentData.TimosId, editionTodo.ParentData.ElementType, editionTodo.ParentData.ElementId), "", false, true);
editionTodo.BtnSave.click.BindCommand(aas.Services.Browser.BootStrapClientService.CloseModal(aas.ViewName.EditionTodo));
// Configuration de la PropertyGrid en mode édition
editionTodo.GridChampsTodo.BindList(detailTodo.ParentData.RelationTodoValeurChamp.TodoValeurChamp, "ValeurChamp", "LibelleChamp", "OrdreChamp");
editionTodo.GridChampsTodo.TypeTableName.BindData(detailTodo.ParentPath.RelationTodoDefinitionChamp.ChampTimos);
editionTodo.GridChampsTodo.TypeTableNameColumn.BindData("LibelleConvivial");
editionTodo.GridChampsTodo.TypeTableTypeColumn.BindData("AspectizeFieldType");
editionTodo.GridChampsTodo.TypeTableControlTypeColumn.BindData("AspectizeControlType");
editionTodo.GridChampsTodo.TypeTableFormatColumn.BindData(aas.Path.MainData.ChampTimos.FormatDate);
editionTodo.GridChampsTodo.EnumValuesTableName.BindData(detailTodo.ParentPath.ValeursPossibles.ValeursChamp);
editionTodo.GridChampsTodo.EnumValuesTableOptionTextColumn.BindData("DisplayedValue");
editionTodo.GridChampsTodo.EnumValuesTableOptionValueColumn.BindData("StoredValue");
editionTodo.GridChampsTodo.EnumValuesTableTypeColumn.BindData("ChampTimosId");
editionTodo.GridChampsTodo.EditMode.BindData(true);
editionTodo.OnActivated.BindCommand(aas.Services.Browser.ClientTodosService.InitPropertyGrid(aas.ViewName.EditionTodo.GridChampsTodo, true));
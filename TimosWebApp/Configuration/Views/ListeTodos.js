var listeTodos = Aspectize.CreateView("ListeTodos", aas.Controls.ListeTodos, aas.Zones.Home.ZoneInfo, true);
listeTodos.OnActivated.BindCommand(aas.Services.Browser.History.PushState(aas.ViewName.ListeTodos));
listeTodos.GridListeTodos.BindGrid(aas.Data.MainData.Todos);
listeTodos.GridListeTodos.className.BindData('table-condensed');
listeTodos.GridListeTodos.OnRowClick.BindCommand(aas.Services.Browser.UIService.ShowView(aas.ViewName.DetailTodo));
listeTodos.GridListeTodos.OnRowClick.BindCommand(aas.Services.Server.TodosService.GetTodoDetails(aas.Data.MainData.Todos.TimosId), aas.Data.MainData, true, true);
var cLabael = listeTodos.GridListeTodos.AddGridColumn("libelle", aas.ColumnType.Span);
cLabael.Text.BindData(listeTodos.GridListeTodos.DataSource.Label);
cLabael.HeaderText.BindData("Libellé");
var cStartDate = listeTodos.GridListeTodos.AddGridColumn("startdate", aas.ColumnType.Span);
cStartDate.Text.BindData(listeTodos.GridListeTodos.DataSource.StartDate, "dd/MM/yyyy");
cStartDate.HeaderText.BindData("Date début");

listeTodos.ChampFiltreLabel.keyup.BindCommand(aas.Services.Browser.ClientTodosService.FiltreTodos(listeTodos.ChampFiltreLabel.value));

var detailTodo = Aspectize.CreateView("DetailTodo", aas.Controls.DetailTodo, aas.Zones.Home.ZoneInfo, false, aas.Data.MainData.Todos);
detailTodo.OnActivated.BindCommand(aas.Services.Browser.History.PushState(aas.ViewName.DetailTodo, aas.Path.MainData.Todos, detailTodo.ParentData.TimosId));
detailTodo.LabelToDo.BindData(detailTodo.ParentData.Label);
detailTodo.DescriptionElementEdite.BindData(detailTodo.ParentData.ElementDescription);
detailTodo.DateDebutTodo.BindData(detailTodo.ParentData.StartDate);
detailTodo.InstrictionsTodo.BindData(detailTodo.ParentData.Instructions);
detailTodo.BoutonEditionTodo.click.BindCommand(aas.Services.Browser.BootStrapClientService.ShowModal(aas.ViewName.EditionTodo, true, false, true));

detailTodo.GridChampsTodo.BindList(detailTodo.ParentData.RelationTodoValeurChamp.TodoValeurChamp, "ValeurChamp", "LibelleChamp", "OrdreChamp");
detailTodo.GridChampsTodo.TypeTableName.BindData(detailTodo.ParentPath.RelationTodoDefinitionChamp.ChampTimos);
detailTodo.GridChampsTodo.TypeTableNameColumn.BindData("LibelleConvivial");
detailTodo.GridChampsTodo.TypeTableTypeColumn.BindData("AspectizeFieldType");
detailTodo.GridChampsTodo.TypeTableControlTypeColumn.BindData("AspectizeControlType");
detailTodo.GridChampsTodo.EnumValuesTableName.BindData(detailTodo.ParentPath.ValeursPossibles.ValeursChamp);
detailTodo.GridChampsTodo.EnumValuesTableOptionTextColumn.BindData("DisplayedValue");
detailTodo.GridChampsTodo.EnumValuesTableOptionValueColumn.BindData("StoredValue");
detailTodo.GridChampsTodo.EnumValuesTableTypeColumn.BindData("TimosId");
//detailTodo.OnLoad.BindCommand(aas.Services.Browser.ClientTodosService.InitPropertyGrid());

var editionTodo = Aspectize.CreateView("EditionTodo", aas.Controls.EditionTodo, "", false, aas.Data.MainData.Todos);
editionTodo.OnActivated.BindCommand(aas.Services.Browser.DataRecorder.Start(aas.Data.MainData));
editionTodo.BtnCancel.click.BindCommand(aas.Services.Browser.BootStrapClientService.CloseModal(aas.ViewName.EditionTodo));
editionTodo.BtnCancel.click.BindCommand(aas.Services.Browser.DataRecorder.CancelRowChanges(aas.Data.MainData));
editionTodo.BtnSave.click.BindCommand(aas.Services.Server.TodosService.SaveTodo(aas.Data.MainData, editionTodo.ParentData.TimosId, editionTodo.ParentData.ElementType, editionTodo.ParentData.ElementId), "", false, true);
editionTodo.BtnSave.click.BindCommand(aas.Services.Browser.BootStrapClientService.CloseModal(aas.ViewName.EditionTodo));

editionTodo.GridChampsTodo.BindList(detailTodo.ParentData.RelationTodoValeurChamp.TodoValeurChamp, "ValeurChamp", "LibelleChamp", "OrdreChamp");
editionTodo.GridChampsTodo.TypeTableName.BindData(detailTodo.ParentPath.RelationTodoDefinitionChamp.ChampTimos);
editionTodo.GridChampsTodo.TypeTableNameColumn.BindData("LibelleConvivial");
editionTodo.GridChampsTodo.TypeTableTypeColumn.BindData("AspectizeFieldType");
editionTodo.GridChampsTodo.TypeTableControlTypeColumn.BindData("AspectizeControlType");
editionTodo.GridChampsTodo.EnumValuesTableName.BindData(detailTodo.ParentPath.ValeursPossibles.ValeursChamp);
editionTodo.GridChampsTodo.EnumValuesTableOptionTextColumn.BindData("DisplayedValue");
editionTodo.GridChampsTodo.EnumValuesTableOptionValueColumn.BindData("StoredValue");
editionTodo.GridChampsTodo.EnumValuesTableTypeColumn.BindData("TimosId");
editionTodo.GridChampsTodo.EditMode.BindData(true);
//editionTodo.OnLoad.BindCommand(aas.Services.Browser.ClientTodosService.InitPropertyGrid());

//  Liste des todos
var vListeTodos = Aspectize.CreateView("ListeTodos", aas.Controls.ListeTodos, aas.Zones.Home.ZoneInfo, true);
vListeTodos.OnActivated.BindCommand(aas.Services.Browser.History.PushState(aas.ViewName.ListeTodos));
vListeTodos.GridListeTodos.BindGrid(aas.Data.MainData.Todos);
vListeTodos.GridListeTodos.className.BindData('table-condensed');
vListeTodos.GridListeTodos.OnRowClick.BindCommand(aas.Services.Server.TodosService.GetTodoDetails(aas.Data.MainData.Todos.TimosId), aas.Data.MainData, true, true);
vListeTodos.GridListeTodos.OnRowClick.BindCommand(aas.Services.Browser.UIService.ShowView(aas.ViewName.DetailTodo));
vListeTodos.GridListeTodos.PageSize.BindData(20);

// Colonnes de la liste des todos
var colStartDate = vListeTodos.GridListeTodos.AddGridColumn("startdate", aas.ColumnType.Span);
colStartDate.Text.BindData(vListeTodos.GridListeTodos.DataSource.StartDate, "dd MMM yyyy");
colStartDate.HeaderText.BindData("Démarré le");
var colLabael = vListeTodos.GridListeTodos.AddGridColumn("libelle", aas.ColumnType.Span);
colLabael.Text.BindData(vListeTodos.GridListeTodos.DataSource.Label);
colLabael.HeaderText.BindData("Libellé du todo");
/*var colAction = vListeTodos.GridListeTodos.AddGridColumn("action", aas.ColumnType.Span);
colAction.Text.BindData('>');
colAction.HeaderText.BindData('');*/

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
//vDetailTodo.BoutonTerminerTodo.click.BindCommand(aas.Services.Server.TodosService.EndTodo(vDetailTodo.ParentData.TimosId), "", true, true);
//vDetailTodo.BoutonTerminerTodo.click.BindCommand(aas.Services.Browser.ClientTodosService.ToastAlert("todo terminé"));
vDetailTodo.BoutonTerminerTodo.click.BindCommand(aas.Services.Browser.ClientTodosService.EndTodo(vDetailTodo.ParentData.TimosId));
//vDetailTodo.BoutonTerminerTodo.hidden

// Gestion des onglets
var vDetailTodoTab = Aspectize.CreateView("DetailTodoTabs", aas.Controls.Bootstrap.BootstrapTab, aas.Zones.DetailTodo.ZoneOnglets, true);
//vDetailTodoTab.className.BindData(aas.Expression(IIF(aas.Data.MainData.Todos.RelationTodoDocument.DocumentsAttendus.TimosId, 'display-documents', '')));


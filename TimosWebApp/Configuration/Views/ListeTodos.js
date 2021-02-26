//  Liste des todos
var vListeTodos = Aspectize.CreateView("ListeTodos", aas.Controls.ListeTodos, aas.Zones.Home.ZoneInfo, true);
vListeTodos.OnActivated.BindCommand(aas.Services.Browser.History.PushState(aas.ViewName.ListeTodos));
vListeTodos.GridListeTodos.BindGrid(aas.Data.MainData.Todos);
vListeTodos.GridListeTodos.className.BindData('table-condensed');
vListeTodos.GridListeTodos.OnRowClick.BindCommand(aas.Services.Server.TodosService.GetTodoDetails(aas.Data.MainData.Todos.TimosId), aas.Data.MainData, true, true);
vListeTodos.GridListeTodos.OnRowClick.BindCommand(aas.Services.Browser.UIService.ShowView(aas.ViewName.DetailTodo));
vListeTodos.GridListeTodos.PageSize.BindData(20);

// Colonnes de la liste des todos
var colEtat = vListeTodos.GridListeTodos.AddGridColumn("etattodo", aas.ColumnType.Bootstrap.BootstrapSpan);
colEtat.HeaderText.BindData("Etat");
colEtat.Text.BindData(vListeTodos.GridListeTodos.DataSource.EtatTodo);
colEtat.className.BindData(aas.Expression('gcEtatTodo' + vListeTodos.GridListeTodos.DataSource.EtatTodo));
var colStartDate = vListeTodos.GridListeTodos.AddGridColumn("startdate", aas.ColumnType.Span);
colStartDate.HeaderText.BindData("Démarré le");
colStartDate.Text.BindData(vListeTodos.GridListeTodos.DataSource.StartDate, "dd MMM yyyy");
var colLabael = vListeTodos.GridListeTodos.AddGridColumn("libelle", aas.ColumnType.Span);
colLabael.HeaderText.BindData("Libellé du todo");
colLabael.Text.BindData(vListeTodos.GridListeTodos.DataSource.Label);

// Filtre et compteur
vListeTodos.ChampFiltreLabel.keyup.BindCommand(aas.Services.Browser.ClientTodosService.FiltreTodos(vListeTodos.ChampFiltreLabel.value, vListeTodos.CheckFiltreDemarre.checked, vListeTodos.CheckFiltreTermine.checked, vListeTodos.CheckFiltreRetard.checked));
vListeTodos.CheckFiltreDemarre.CheckedChanged.BindCommand(aas.Services.Browser.ClientTodosService.FiltreTodos(vListeTodos.ChampFiltreLabel.value, vListeTodos.CheckFiltreDemarre.checked, vListeTodos.CheckFiltreTermine.checked, vListeTodos.CheckFiltreRetard.checked));
vListeTodos.CheckFiltreTermine.CheckedChanged.BindCommand(aas.Services.Browser.ClientTodosService.FiltreTodos(vListeTodos.ChampFiltreLabel.value, vListeTodos.CheckFiltreDemarre.checked, vListeTodos.CheckFiltreTermine.checked, vListeTodos.CheckFiltreRetard.checked));
vListeTodos.CheckFiltreRetard.CheckedChanged.BindCommand(aas.Services.Browser.ClientTodosService.FiltreTodos(vListeTodos.ChampFiltreLabel.value, vListeTodos.CheckFiltreDemarre.checked, vListeTodos.CheckFiltreTermine.checked, vListeTodos.CheckFiltreRetard.checked));
vListeTodos.CompteurTodos.BindData(vListeTodos.GridListeTodos.RowCount);
vListeTodos.TotalTodos.BindData(aas.Services.Browser.DataService.Count(aas.Data.MainData.Todos));

// Vue détaillée d'un todo
var vDetailTodo = Aspectize.CreateView("DetailTodo", aas.Controls.DetailTodo, aas.Zones.Home.ZoneInfo, false, aas.Data.MainData.Todos);
vDetailTodo.OnActivated.BindCommand(aas.Services.Browser.History.PushState(aas.ViewName.DetailTodo, aas.Path.MainData.Todos, vDetailTodo.ParentData.TimosId));
vDetailTodo.LabelToDo.BindData(vDetailTodo.ParentData.Label);
vDetailTodo.DescriptionElementEdite.BindData(vDetailTodo.ParentData.ElementDescription);
vDetailTodo.InstrictionsTodo.BindData(vDetailTodo.ParentData.Instructions);
vDetailTodo.DateDebutTodo.BindData(vDetailTodo.ParentData.StartDate, 'dd MMM yyyy');
vDetailTodo.DateFinTodo.BindData(aas.Expression(IIF(vDetailTodo.ParentData.EndDate, vDetailTodo.ParentData.EndDate, 'En cours...')), 'dd MMM yyyy');
vDetailTodo.DisplayBtnTerminer.BindData(aas.Expression(IIF(vDetailTodo.ParentData.EtatTodo == 2, '', 'hidden')));

//vDetailTodo.BoutonTerminerTodo.click.BindCommand(aas.Services.Server.TodosService.EndTodo(vDetailTodo.ParentData.TimosId), "", true, true);
//vDetailTodo.BoutonTerminerTodo.click.BindCommand(aas.Services.Browser.ClientTodosService.ToastAlert("todo terminé"));
vDetailTodo.BoutonTerminerTodo.click.BindCommand(aas.Services.Browser.BootStrapClientService.ShowModal(aas.ViewName.ConfirmationEndTodo, true, true, false));
vDetailTodo.SelectAction.BindList(vDetailTodo.ParentData.RelationTodoActions.Action, 'Id', 'Libelle', 'Libelle');
vDetailTodo.SelectAction.NullValueDisplay.BindData('Action à lancer');
vDetailTodo.SelectAction.DefaultIndex.BindData(0);
vDetailTodo.SelectAction.SelectedValueChanged.BindCommand(aas.Services.Browser.TestingServices.Alert(vDetailTodo.SelectAction.CurrentDisplay));

// Modale de confirmation de fin de todo
var vConfirmationEndTodo = Aspectize.CreateView("ConfirmationEndTodo", aas.Controls.ConfirmationEndTodo, "", false, aas.Data.MainData.Todos);
vConfirmationEndTodo.LabelToDo.BindData(vConfirmationEndTodo.ParentData.Label);
vConfirmationEndTodo.BtnNon.click.BindCommand(aas.Services.Browser.BootStrapClientService.CloseModal(aas.ViewName.ConfirmationEndTodo));
vConfirmationEndTodo.BtnOui.click.BindCommand(aas.Services.Browser.ClientTodosService.EndTodo(vDetailTodo.ParentData.TimosId, vDetailTodo.ParentData.Label));
//vConfirmationEndTodo.BtnOui.click.BindCommand(aas.Services.Browser.BootStrapClientService.CloseModal(aas.ViewName.ConfirmationEndTodo));

// Gestion des onglets
var vDetailTodoTab = Aspectize.CreateView("DetailTodoTabs", aas.Controls.Bootstrap.BootstrapTab, aas.Zones.DetailTodo.ZoneOnglets, true);
//vDetailTodoTab.className.BindData(aas.Expression(IIF(aas.Data.MainData.Todos.RelationTodoDocument.DocumentsAttendus.TimosId, 'display-documents', '')));

// Configuration des Actions
var vExecutionAction = Aspectize.CreateView("ExecutionAction", aas.Controls.ExecutionAction, "", false, aas.Data.MainData.Todos.RelationTodoActions.Action);
//vExecutionAction.Select1.BindList(vExecutionAction.ParentData, '', '')

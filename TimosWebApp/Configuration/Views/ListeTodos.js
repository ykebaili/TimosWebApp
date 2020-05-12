var listeTodos = Aspectize.CreateView("ListeTodos", aas.Controls.ListeTodos, aas.Zones.Home.ZoneInfo, true);
listeTodos.OnActivated.BindCommand(aas.Services.Browser.History.PushState(aas.ViewName.ListeTodos));
listeTodos.GridListeTodos.BindGrid(aas.Data.MainData.Todos);
listeTodos.GridListeTodos.className.BindData('table-condensed');
listeTodos.GridListeTodos.OnRowClick.BindCommand(aas.Services.Browser.UIService.ShowView(aas.ViewName.DetailTodo));
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
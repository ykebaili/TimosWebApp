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

// Selection d'une Action globale
vListeTodos.SelectAction.BindList(aas.Data.MainData.Action, 'Id', 'Libelle', 'Libelle', aas.Expression('IsGlobale'));
vListeTodos.SelectAction.NullValueDisplay.BindData('Actions');
vListeTodos.SelectAction.DefaultIndex.BindData(0);
vListeTodos.SelectAction.SelectedValueChanged.BindCommand(aas.Services.Browser.ClientTodosService.PrepareAction(
    vListeTodos.SelectAction.CurrentValue,
        aas.Data.MainData.Todos.ElementType,
        aas.Data.MainData.Todos.ElementId));

// Vue détaillée d'un todo
var vDetailTodo = Aspectize.CreateView("DetailTodo", aas.Controls.DetailTodo, aas.Zones.Home.ZoneInfo, false, aas.Data.MainData.Todos);
vDetailTodo.OnActivated.BindCommand(aas.Services.Browser.History.PushState(aas.ViewName.DetailTodo, aas.Path.MainData.Todos, vDetailTodo.ParentData.TimosId));
vDetailTodo.LabelToDo.BindData(vDetailTodo.ParentData.Label);
vDetailTodo.DescriptionElementEdite.BindData(vDetailTodo.ParentData.ElementDescription);
vDetailTodo.InstrictionsTodo.BindData(vDetailTodo.ParentData.Instructions);
vDetailTodo.DateDebutTodo.BindData(vDetailTodo.ParentData.StartDate, 'dd MMM yyyy');
vDetailTodo.DateFinTodo.BindData(aas.Expression(IIF(vDetailTodo.ParentData.EndDate, vDetailTodo.ParentData.EndDate, 'En cours...')), 'dd MMM yyyy');
vDetailTodo.DisplayBtnTerminer.BindData(aas.Expression(IIF(vDetailTodo.ParentData.EtatTodo == 2, '', 'hidden')));

// Terminer Todo
vDetailTodo.BoutonTerminerTodo.click.BindCommand(aas.Services.Browser.BootStrapClientService.ShowModal(aas.ViewName.ConfirmationEndTodo, true, true, false));

// Selection d'une Action spécifique à un Todo
vDetailTodo.SelectAction.BindList(vDetailTodo.ParentData.RelationTodoActions.Action, 'Id', 'Libelle', 'Libelle', aas.Expression('!IsGlobale'));
vDetailTodo.SelectAction.NullValueDisplay.BindData('Actions');
vDetailTodo.SelectAction.DefaultIndex.BindData(0);
vDetailTodo.SelectAction.SelectedValueChanged.BindCommand(aas.Services.Browser.ClientTodosService.PrepareAction(
    vDetailTodo.SelectAction.CurrentValue,
    aas.Data.MainData.Todos.ElementType,
    aas.Data.MainData.Todos.ElementId));

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
var vExecutionAction = Aspectize.CreateView("ExecutionAction", aas.Controls.ExecutionAction, "", false, aas.Data.MainData.Action);
vExecutionAction.OnActivated.BindCommand(aas.Services.Browser.DataRecorder.Start(aas.Data.MainData));
vExecutionAction.BtnCancel.click.BindCommand(aas.Services.Browser.BootStrapClientService.CloseModal(aas.ViewName.ExecutionAction));
vExecutionAction.BtnCancel.click.BindCommand(aas.Services.Browser.DataRecorder.CancelRowChanges(aas.Data.MainData));
vExecutionAction.BtnSave.click.BindCommand(aas.Services.Browser.ClientTodosService.ExecuteAction(
    aas.Data.MainData,
    vExecutionAction.ParentData.Id,
    aas.Data.MainData.Todos.ElementType,
    aas.Data.MainData.Todos.ElementId));
vExecutionAction.LibelleAction.BindData(vExecutionAction.ParentData.Libelle);
vExecutionAction.LibelleTodo.BindData(aas.Data.MainData.Todos.Label);
vExecutionAction.InstructionsAction.BindData(vExecutionAction.ParentData.Instructions);

vExecutionAction.LabelVarText1.BindData(vExecutionAction.ParentData.LBLT1);
vExecutionAction.LabelVarText2.BindData(vExecutionAction.ParentData.LBLT2);
vExecutionAction.LabelVarText3.BindData(vExecutionAction.ParentData.LBLT3);
vExecutionAction.LabelVarText4.BindData(vExecutionAction.ParentData.LBLT4);
vExecutionAction.LabelVarText5.BindData(vExecutionAction.ParentData.LBLT5);

vExecutionAction.VarText1.value.BindData(vExecutionAction.ParentData.VALT1);
vExecutionAction.VarText2.value.BindData(vExecutionAction.ParentData.VALT2);
vExecutionAction.VarText3.value.BindData(vExecutionAction.ParentData.VALT3);
vExecutionAction.VarText4.value.BindData(vExecutionAction.ParentData.VALT4);
vExecutionAction.VarText5.value.BindData(vExecutionAction.ParentData.VALT5);

vExecutionAction.SelectVarText1.BindList(vExecutionAction.ParentData.RelationActionValeursVariable.ValeursVariable, "Value", "Display", "Value", aas.Expression('IdVariable === "T1"'));
vExecutionAction.SelectVarText1.SelectedValue.BindData(vExecutionAction.ParentData.VALT1);
vExecutionAction.SelectVarText2.BindList(vExecutionAction.ParentData.RelationActionValeursVariable.ValeursVariable, "Value", "Display", "Value", aas.Expression('IdVariable === "T2"'));
vExecutionAction.SelectVarText2.SelectedValue.BindData(vExecutionAction.ParentData.VALT2);
vExecutionAction.SelectVarText3.BindList(vExecutionAction.ParentData.RelationActionValeursVariable.ValeursVariable, "Value", "Display", "Value", aas.Expression('IdVariable === "T3"'));
vExecutionAction.SelectVarText3.SelectedValue.BindData(vExecutionAction.ParentData.VALT3);
vExecutionAction.SelectVarText4.BindList(vExecutionAction.ParentData.RelationActionValeursVariable.ValeursVariable, "Value", "Display", "Value", aas.Expression('IdVariable === "T4"'));
vExecutionAction.SelectVarText4.SelectedValue.BindData(vExecutionAction.ParentData.VALT4);
vExecutionAction.SelectVarText5.BindList(vExecutionAction.ParentData.RelationActionValeursVariable.ValeursVariable, "Value", "Display", "Value", aas.Expression('IdVariable === "T5"'));
vExecutionAction.SelectVarText5.SelectedValue.BindData(vExecutionAction.ParentData.VALT5);

vExecutionAction.DisplayVarText1.BindData(aas.Expression(IIF(vExecutionAction.SelectVarText1.CurrentValue, 'hidden', '')));
vExecutionAction.DisplaySelectVarText1.BindData(aas.Expression(IIF(vExecutionAction.SelectVarText1.CurrentValue, '', 'hidden')));
vExecutionAction.DisplayVarText2.BindData(aas.Expression(IIF(vExecutionAction.SelectVarText2.CurrentValue, 'hidden', '')));
vExecutionAction.DisplaySelectVarText2.BindData(aas.Expression(IIF(vExecutionAction.SelectVarText2.CurrentValue, '', 'hidden')));
vExecutionAction.DisplayVarText3.BindData(aas.Expression(IIF(vExecutionAction.SelectVarText3.CurrentValue, 'hidden', '')));
vExecutionAction.DisplaySelectVarText3.BindData(aas.Expression(IIF(vExecutionAction.SelectVarText3.CurrentValue, '', 'hidden')));
vExecutionAction.DisplayVarText4.BindData(aas.Expression(IIF(vExecutionAction.SelectVarText4.CurrentValue, 'hidden', '')));
vExecutionAction.DisplaySelectVarText4.BindData(aas.Expression(IIF(vExecutionAction.SelectVarText4.CurrentValue, '', 'hidden')));
vExecutionAction.DisplayVarText5.BindData(aas.Expression(IIF(vExecutionAction.SelectVarText5.CurrentValue, 'hidden', '')));
vExecutionAction.DisplaySelectVarText5.BindData(aas.Expression(IIF(vExecutionAction.SelectVarText5.CurrentValue, '', 'hidden')));


vExecutionAction.LabelVarInt1.BindData(vExecutionAction.ParentData.LBLN1);
vExecutionAction.LabelVarInt2.BindData(vExecutionAction.ParentData.LBLN2);
vExecutionAction.LabelVarInt3.BindData(vExecutionAction.ParentData.LBLN3);

vExecutionAction.VarInt1.value.BindData(vExecutionAction.ParentData.VALN1);
vExecutionAction.VarInt2.value.BindData(vExecutionAction.ParentData.VALN2);
vExecutionAction.VarInt3.value.BindData(vExecutionAction.ParentData.VALN3);

vExecutionAction.SelectVarInt1.BindList(vExecutionAction.ParentData.RelationActionValeursVariable.ValeursVariable, "Value", "Display", "Value", aas.Expression('IdVariable === "N1"'));
vExecutionAction.SelectVarInt1.SelectedValue.BindData(vExecutionAction.ParentData.VALN1);
vExecutionAction.SelectVarInt2.BindList(vExecutionAction.ParentData.RelationActionValeursVariable.ValeursVariable, "Value", "Display", "Value", aas.Expression('IdVariable === "N2"'));
vExecutionAction.SelectVarInt2.SelectedValue.BindData(vExecutionAction.ParentData.VALN2);
vExecutionAction.SelectVarInt3.BindList(vExecutionAction.ParentData.RelationActionValeursVariable.ValeursVariable, "Value", "Display", "Value", aas.Expression('IdVariable === "N3"'));
vExecutionAction.SelectVarInt3.SelectedValue.BindData(vExecutionAction.ParentData.VALN3);

vExecutionAction.DisplayVarInt1.BindData(aas.Expression(IIF(vExecutionAction.SelectVarInt1.CurrentValue, 'hidden', '')));
vExecutionAction.DisplaySelectVarInt1.BindData(aas.Expression(IIF(vExecutionAction.SelectVarInt1.CurrentValue, '', 'hidden')));
vExecutionAction.DisplayVarInt2.BindData(aas.Expression(IIF(vExecutionAction.SelectVarInt2.CurrentValue, 'hidden', '')));
vExecutionAction.DisplaySelectVarInt2.BindData(aas.Expression(IIF(vExecutionAction.SelectVarInt2.CurrentValue, '', 'hidden')));
vExecutionAction.DisplayVarInt3.BindData(aas.Expression(IIF(vExecutionAction.SelectVarInt3.CurrentValue, 'hidden', '')));
vExecutionAction.DisplaySelectVarInt3.BindData(aas.Expression(IIF(vExecutionAction.SelectVarInt3.CurrentValue, '', 'hidden')));

vExecutionAction.LabelVarDate1.BindData(vExecutionAction.ParentData.LBLD1);
vExecutionAction.LabelVarDate2.BindData(vExecutionAction.ParentData.LBLD2);
vExecutionAction.LabelVarDate3.BindData(vExecutionAction.ParentData.LBLD3);

vExecutionAction.VarDate1.Value.BindData(vExecutionAction.ParentData.VALD1);
vExecutionAction.VarDate2.Value.BindData(vExecutionAction.ParentData.VALD2);
vExecutionAction.VarDate3.Value.BindData(vExecutionAction.ParentData.VALD3);

vExecutionAction.LabelVarBool1.BindData(vExecutionAction.ParentData.LBLB1);
vExecutionAction.LabelVarBool2.BindData(vExecutionAction.ParentData.LBLB2);
vExecutionAction.LabelVarBool3.BindData(vExecutionAction.ParentData.LBLB3);

vExecutionAction.VarBool1.checked.BindData(vExecutionAction.ParentData.VALB1);
vExecutionAction.VarBool2.checked.BindData(vExecutionAction.ParentData.VALB2);
vExecutionAction.VarBool3.checked.BindData(vExecutionAction.ParentData.VALB3);
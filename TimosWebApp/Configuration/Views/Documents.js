var vDocumentsAttendus = Aspectize.CreateView("DocumentsAttendusTodo", aas.Controls.DocumentsAttendus, "DetailTodoTabs.1:Documents", false, aas.Data.MainData.Todos);

var vDocumentAttendu = Aspectize.CreateRepeatedView("DocumentAttendu", aas.Controls.DocumentAttendu, aas.Zones.DocumentsAttendusTodo.PanelDocumentsAttendus, aas.Data.MainData.Todos.RelationTodoDocument.DocumentsAttendus); // possible de trier et filtrer
vDocumentAttendu.UploaderDocument.MultipleFiles.BindData(true);
vDocumentAttendu.UploaderDocument.OnFileSelected.BindCommand(aas.Services.Server.TodosService.UploadDocuments(vDocumentAttendu.UploaderDocument.SelectedFile, aas.Data.MainData.Todos.TimosId, vDocumentAttendu.ParentData.TimosId), aas.Data.MainData, true, true);
vDocumentAttendu.LibelleDocument.BindData(vDocumentAttendu.ParentData.Libelle);
vDocumentAttendu.LienDocument.click.BindCommand(aas.Services.Browser.ClientTodosService.UploadDocument(vDocumentAttendu.ParentData.TimosId, aas.ViewName.DocumentAttendu.UploaderDocument));

vDocumentAttendu.GridFichiers.BindGrid(vDocumentAttendu.ParentData.RelationFichiers.FichiersAssocies);
var cNomFichier = vDocumentAttendu.GridFichiers.AddGridColumn("NomFichier", aas.ColumnType.Span);
cNomFichier.HeaderText.BindData("Nom du fichier");
cNomFichier.Text.BindData(vDocumentAttendu.GridFichiers.DataSource.NomFichier);


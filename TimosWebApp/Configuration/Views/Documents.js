// Création de l'onglet Documents
var vDocumentsAttendus = Aspectize.CreateView("DocumentsAttendusTodo", aas.Controls.DocumentsAttendus, "DetailTodoTabs.1:Documents", false, aas.Data.MainData.Todos);

// Détails d'un document attendu
var vDocumentAttendu = Aspectize.CreateRepeatedView("DocumentAttendu", aas.Controls.DocumentAttendu, aas.Zones.DocumentsAttendusTodo.PanelDocumentsAttendus, aas.Data.MainData.Todos.RelationTodoDocument.DocumentsAttendus); // possible de trier et filtrer
vDocumentAttendu.LibelleDocument.BindData(vDocumentAttendu.ParentData.Libelle);
vDocumentAttendu.NombreMin.BindData(vDocumentAttendu.ParentData.NombreMin);
vDocumentAttendu.DateLastUpload.BindData(vDocumentAttendu.ParentData.DateLastUpload);
// Gestion de l'upload de fichiers
vDocumentAttendu.BoutonUploadFichier.click.BindCommand(aas.Services.Browser.ClientTodosService.UploadDocument(vDocumentAttendu.ParentData.TimosId, aas.ViewName.DocumentAttendu.UploaderDocument));
vDocumentAttendu.UploaderDocument.MultipleFiles.BindData(true);
vDocumentAttendu.UploaderDocument.OnFileSelected.BindCommand(aas.Services.Server.TodosService.UploadDocuments(vDocumentAttendu.UploaderDocument.SelectedFile, aas.Data.MainData.Todos.TimosId, vDocumentAttendu.ParentData.TimosId, vDocumentAttendu.ParentData.IdCategorie), aas.Data.MainData, true, true);

vDocumentAttendu.GridFichiers.BindGrid(vDocumentAttendu.ParentData.RelationFichiers.FichiersAttaches);
var colNomFichier = vDocumentAttendu.GridFichiers.AddGridColumn("NomFichier", aas.ColumnType.Span);
colNomFichier.HeaderText.BindData("Nom du fichier");
colNomFichier.Text.BindData(vDocumentAttendu.GridFichiers.DataSource.NomFichier);
vDocumentAttendu.GridFichiers.HideHeadersIfNoData.BindData(true);

/*var colActionSupprimer = vDocumentAttendu.GridFichiers.AddGridColumn("ActionSupprimer", aas.ColumnType.Button);
colActionSupprimer.HeaderText.BindData("");
colActionSupprimer.Text.BindData("Supprimer");*/

var colActionSupprimer = vDocumentAttendu.GridFichiers.AddGridColumn("ActionSupprimer", aas.ColumnType.Link);
colActionSupprimer.HeaderText.BindData("");
colActionSupprimer.Text.BindData("Supprimer");


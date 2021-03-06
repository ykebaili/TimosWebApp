﻿// Création de l'onglet Documents
var vDocumentsAttendus = Aspectize.CreateView("DocumentsAttendusTodo", aas.Controls.DocumentsAttendus, "DetailTodoTabs.1:Documents", false, aas.Data.MainData.Todos);

// Détails d'un document attendu
var vDocumentAttendu = Aspectize.CreateRepeatedView("DocumentAttendu", 
    aas.Controls.DocumentAttendu, aas.Zones.DocumentsAttendusTodo.PanelDocumentsAttendus,
    aas.Data.MainData.Todos.RelationTodoDocument.DocumentsAttendus, '', 
    aas.Expression('TimosId > 0'));
vDocumentAttendu.LibelleDocument.BindData(vDocumentAttendu.ParentData.Libelle);

// Gestion de l'upload de fichiers
vDocumentAttendu.BoutonUploadFichier.click.BindCommand(aas.Services.Browser.ClientTodosService.UploadDocument(vDocumentAttendu.ParentData.TimosId, aas.ViewName.DocumentAttendu.UploaderDocument));
vDocumentAttendu.UploaderDocument.MultipleFiles.BindData(true);
vDocumentAttendu.UploaderDocument.OnFileSelected.BindCommand(aas.Services.Server.TodosService.UploadDocuments(vDocumentAttendu.UploaderDocument.SelectedFile, aas.Data.MainData.Todos.TimosId, vDocumentAttendu.ParentData.TimosId, vDocumentAttendu.ParentData.Libelle, vDocumentAttendu.ParentData.IdCategorie), aas.Data.MainData, true, true);

vDocumentAttendu.GridFichiers.BindGrid(vDocumentAttendu.ParentData.RelationFichiers.FichiersAttaches);
var colNomFichier = vDocumentAttendu.GridFichiers.AddGridColumn("NomFichier", aas.ColumnType.Span);
colNomFichier.HeaderText.BindData("Nom du fichier");
colNomFichier.Text.BindData(vDocumentAttendu.GridFichiers.DataSource.NomFichier);
vDocumentAttendu.GridFichiers.HideHeadersIfNoData.BindData(true);

// Visualiser (télécharger) un fihcier
var colActionVisualiser = vDocumentAttendu.GridFichiers.AddGridColumn("ActionVisualiser", aas.ColumnType.TimosLink);
colActionVisualiser.BtnClasse.BindData("btn-info");
colActionVisualiser.IconButton.BindData("fas fa-eye");
colActionVisualiser.HeaderText.BindData("");
colActionVisualiser.Text.BindData("Visualiser");
colActionVisualiser.Href.BindData(aas.Expression('TodosService.DownloadDocument.bin.cmd.ashx?strKeyFile=' + vDocumentAttendu.GridFichiers.DataSource.TimosKey + '&strFileName=' + vDocumentAttendu.GridFichiers.DataSource.NomFichier));

// Supprimer un fichier
var colActionSupprimer = vDocumentAttendu.GridFichiers.AddGridColumn("ActionSupprimer", aas.ColumnType.TimosButton);
colActionSupprimer.BtnClasse.BindData("btn-danger");
colActionSupprimer.IconButton.BindData("fas fa-trash");
colActionSupprimer.HeaderText.BindData("");
colActionSupprimer.Text.BindData("Supprimer");
// Suppression sans confirmation
colActionSupprimer.Click.BindCommand(aas.Services.Browser.ClientTodosService.DeleteDocument(vDocumentAttendu.GridFichiers.DataSource.TimosKey, vDocumentAttendu.GridFichiers.DataSource.NomFichier));
// Suppression avec confirmation
//colActionSupprimer.Click.BindCommand(aas.Services.Browser.BootStrapClientService.ShowModal(aas.ViewName.ConfirmationSupprimerFichier, true, true, false));

// Confirmation supprimer fichier
var vConfirmationSupprimerFichier = Aspectize.CreateView("ConfirmationSupprimerFichier", aas.Controls.ConfirmationSupprimerFichier, "", false, aas.Data.MainData.Todos.RelationTodoDocument.DocumentsAttendus.RelationFichiers.FichiersAttaches);
vConfirmationSupprimerFichier.NomFichier.BindData(vDocumentAttendu.GridFichiers.DataSource.NomFichier);
vConfirmationSupprimerFichier.BtnNon.click.BindCommand(aas.Services.Browser.BootStrapClientService.CloseModal(aas.ViewName.ConfirmationSupprimerFichier));
vConfirmationSupprimerFichier.BtnOui.click.BindCommand(aas.Services.Browser.ClientTodosService.DeleteDocument(vDocumentAttendu.GridFichiers.DataSource.TimosKey, vDocumentAttendu.GridFichiers.DataSource.NomFichier));


var vListeExports = Aspectize.CreateView("ListeExports", aas.Controls.ListeExports, aas.Zones.Home.ZoneInfo, false);
vListeExports.OnActivated.BindCommand(aas.Services.Browser.History.PushState(aas.ViewName.ListeExports));
vListeExports.OnLoad.BindCommand(aas.Services.Server.ExportService.GetListeExportsForCurrentUser(), aas.Data.MainData, true, true);

var vResumeExport = Aspectize.CreateRepeatedView("ResumeExport", aas.Controls.ResumeExport, aas.Zones.ListeExports.PanelResumesExports, aas.Data.MainData.Export);
vResumeExport.LibelleExport.BindData(vResumeExport.ParentData.Libelle);
vResumeExport.DescriptionExport.BindData(vResumeExport.ParentData.Description);
vResumeExport.DataDate.BindData(vResumeExport.ParentData.DataDate, "dd/MM/yyyy HH:mm");

vResumeExport.BoutonCalculer.click.BindCommand(aas.Services.Server.ExportService.GetDataSetExport(vResumeExport.ParentData.Id), "", false, true);
vResumeExport.BoutonVoirExport.click.BindCommand(aas.Services.Server.ExportService.GetExportForDisplay(vResumeExport.ParentData.Id, vResumeExport.ParentData.Libelle, vResumeExport.ParentData.Description), aas.Data.MainData, true, true);
vResumeExport.BoutonVoirExport.click.BindCommand(aas.Services.Browser.UIService.ShowView(aas.ViewName.ConsultationExport));

// Consultation d'un Export
var vConsultationExport = Aspectize.CreateView("ConsultationExport", aas.Controls.ConsultationExport, aas.Zones.Home.ZoneInfo, false, aas.Data.MainData.Export);
vConsultationExport.OnActivated.BindCommand(aas.Services.Browser.History.PushState(aas.ViewName.ConsultationExport, aas.Path.MainData.Export, vConsultationExport.ParentData.Id));
vConsultationExport.LibelleExport.BindData(vConsultationExport.ParentData.Libelle);
vConsultationExport.DescriptionExport.BindData(vConsultationExport.ParentData.Description);
vConsultationExport.DataDate.BindData(vConsultationExport.ParentData.DataDate, "dd/MM/yyyy HH:mm");
vConsultationExport.LinkExportExcel.Text.BindData('<i class="far fa-file-excel"></i> Exporter vers Excel');
vConsultationExport.LinkExportExcel.ToolTip.BindData("Exporter les données au format Excel sans mise en forme");
vConsultationExport.LinkExportExcel.LinkClassName.BindData("link-export-excel");
vConsultationExport.LinkExportExcel.Href.BindData(aas.Expression('ExportService.GetExportForExcel.bin.cmd.ashx?keyExport=' + vConsultationExport.ParentData.Id + '&strLibelle=' + vConsultationExport.ParentData.Libelle));
vConsultationExport.ChampFiltreDatas.keyup.BindCommand(aas.Services.Browser.ClientExportService.FiltreDatas(vConsultationExport.ChampFiltreDatas.value));

// Grid données de l'export
vConsultationExport.GridExportDatas.BindGrid(vConsultationExport.ParentData.RelationExportDatas.ExportDatas);
vConsultationExport.GridExportDatas.className.BindData('table-condensed');
vConsultationExport.GridExportDatas.PageSize.BindData(100);
vConsultationExport.GridExportDatas.HideHeadersIfNoData.BindData(true);

var colVal1 = vConsultationExport.GridExportDatas.AddGridColumn("val1", aas.ColumnType.Span);
colVal1.HeaderText.BindData(vConsultationExport.ParentData.COL1);
colVal1.Text.BindData(vConsultationExport.GridExportDatas.DataSource.VAL1);

var colVal2 = vConsultationExport.GridExportDatas.AddGridColumn("val2", aas.ColumnType.Span);
colVal2.HeaderText.BindData(vConsultationExport.ParentData.COL2);
colVal2.Text.BindData(vConsultationExport.GridExportDatas.DataSource.VAL2);

var colVal3 = vConsultationExport.GridExportDatas.AddGridColumn("val3", aas.ColumnType.Span);
colVal3.HeaderText.BindData(vConsultationExport.ParentData.COL3);
colVal3.Text.BindData(vConsultationExport.GridExportDatas.DataSource.VAL3);

var colVal4 = vConsultationExport.GridExportDatas.AddGridColumn("val4", aas.ColumnType.Span);
colVal4.HeaderText.BindData(vConsultationExport.ParentData.COL4);
colVal4.Text.BindData(vConsultationExport.GridExportDatas.DataSource.VAL4);

var colVal5 = vConsultationExport.GridExportDatas.AddGridColumn("val5", aas.ColumnType.Span);
colVal5.HeaderText.BindData(vConsultationExport.ParentData.COL5);
colVal5.Text.BindData(vConsultationExport.GridExportDatas.DataSource.VAL5);

var colVal6 = vConsultationExport.GridExportDatas.AddGridColumn("val6", aas.ColumnType.Span);
colVal6.HeaderText.BindData(vConsultationExport.ParentData.COL6);
colVal6.Text.BindData(vConsultationExport.GridExportDatas.DataSource.VAL6);

var colVal7 = vConsultationExport.GridExportDatas.AddGridColumn("val7", aas.ColumnType.Span);
colVal7.HeaderText.BindData(vConsultationExport.ParentData.COL7);
colVal7.Text.BindData(vConsultationExport.GridExportDatas.DataSource.VAL7);

var colVal8 = vConsultationExport.GridExportDatas.AddGridColumn("val8", aas.ColumnType.Span);
colVal8.HeaderText.BindData(vConsultationExport.ParentData.COL8);
colVal8.Text.BindData(vConsultationExport.GridExportDatas.DataSource.VAL8);

var colVal9 = vConsultationExport.GridExportDatas.AddGridColumn("val9", aas.ColumnType.Span);
colVal9.HeaderText.BindData(vConsultationExport.ParentData.COL9);
colVal9.Text.BindData(vConsultationExport.GridExportDatas.DataSource.VAL9);

var colVal10 = vConsultationExport.GridExportDatas.AddGridColumn("val10", aas.ColumnType.Span);
colVal10.HeaderText.BindData(vConsultationExport.ParentData.COL10);
colVal10.Text.BindData(vConsultationExport.GridExportDatas.DataSource.VAL10);

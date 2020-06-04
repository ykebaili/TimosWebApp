
var app = newApplication();

app.Directories = "Bootstrap, BootstrapDateTimePicker";
app.SecurityEnabled = true;
app.SecurityServicesConfiguration = "SecurityService";
app.DisplayExceptionEnabled = true;
app.DisplayExceptionServiceName = "DisplayCustomExceptionService";

app.AddAuthorizationRole(aas.Roles.Anonymous, aas.Enum.AccessControl.Forbidden);
app.AddAuthorizationRole(aas.Roles.Registered, aas.Enum.AccessControl.ReadWrite);
app.OnApplicationStartCommand = 'MyInitialisationService.InitTimos';

var ctxData = newContextData();

ctxData.Name = "MainData";
ctxData.NameSpaceList = "TimosWebApp";
ctxData.IsProfile = true;

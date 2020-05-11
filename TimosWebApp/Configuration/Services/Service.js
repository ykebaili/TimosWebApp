
var DataService = Aspectize.ConfigureNewService("DataService", aas.ConfigurableServices.DataBaseService);

DataService.DataBaseType = aas.ConfigurableServices.DataBaseService.DBMS.XML;
DataService.ConnectionString = "~\Data\Data.xml";
DataService.Trace = false;
DataService.BuildNewTableOnSave = false;
DataService.EnsureAuthenticationOnWrite = true;

var securityService = Aspectize.ConfigureNewService("SecurityService", aas.ConfigurableServices.SecurityServices);

securityService.protocol = aas.ConfigurableServices.SecurityServices.AuthenticationProtocol.PasswordExchange;
securityService.algorithm = aas.ConfigurableServices.SecurityServices.Algorithm.MD5;
securityService.AuthenticationServiceName = "AuthenticationService";
securityService.UserProfileServiceName = "AuthenticationService";
//securityService.OnAuthenticationRequiredCommand = "ClientInscriptionService.DisplayLogin";
securityService.LoginViewName = "Login";

var MailService = Aspectize.ConfigureNewService("MailService", aas.ConfigurableServices.AspectizeSMTPService);

MailService.Host = "";
MailService.Port = 25;
MailService.Ssl = true;
MailService.Login = "";
MailService.Password = "";
MailService.Expediteur = "";
MailService.ExpediteurDisplay = "";


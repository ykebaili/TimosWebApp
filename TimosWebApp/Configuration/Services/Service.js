
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

var TimosInitialisationService = Aspectize.ConfigureNewService("TimosInitialisationService", aas.ConfigurableServices.InitialisationService);
TimosInitialisationService.TimosServerURL = "tcp://127.0.0.1:8160";

var DbLogException = Aspectize.ConfigureNewService("DbLogExceptionService", aas.ConfigurableServices.DBLogException);
DbLogException.MailServiceName = "MailService";
DbLogException.MailTo = "ykebaili@futurocom.com";

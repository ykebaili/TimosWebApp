var home = Aspectize.CreateView("Home", aas.Controls.Home);

home.Login.click.BindCommand(aas.Services.Browser.UIService.ShowView(aas.ViewName.Login));
home.Logout.click.BindCommand(aas.Services.Server.AuthenticationService.LogoutUser());
home.Logout.click.BindCommand(aas.Services.Browser.SecurityServices.SignOut());
home.Logout.click.BindCommand(aas.Services.Browser.UIService.Refresh());

home.UserName.BindData(aas.Data.MainData.User.Name);
home.DisplayAuthenticated.BindData(aas.Expression(IIF(aas.Data.MainData.User.IsAuthentificated, '', 'hidden')));
home.DisplayUnauthenticated.BindData(aas.Expression(IIF(aas.Data.MainData.User.IsAuthentificated, 'hidden', '')));
home.Home.click.BindCommand(aas.Services.Browser.UIService.ShowView(aas.ViewName.ListeTodos));
home.MesTodos.click.BindCommand(aas.Services.Browser.UIService.ShowView(aas.ViewName.ListeTodos));
home.Admin.click.BindCommand(aas.Services.Browser.UIService.ShowView(aas.ViewName.Administration));
home.DisplayAdminNav.BindData(aas.Expression(IIF(aas.Data.MainData.User.IsAdministrator, '', 'hidden')));
home.Exports.click.BindCommand(aas.Services.Browser.UIService.ShowView(aas.ViewName.ListeExports));

var vLogin = Aspectize.CreateView("Login", aas.Controls.Login);
vLogin.AddAuthorizationRole(aas.Roles.Anonymous, aas.Enum.AccessControl.ReadWrite);
//vLogin.BtnLogin.click.BindCommand(aas.Services.Browser.SecurityServices.Authenticate(vLogin.TxtEmail.value, vLogin.TxtPwd.value, vLogin.CheckBoxRememberMe.checked));
vLogin.BtnLogin.click.BindCommand(aas.Services.Browser.ClientAuthenticationService.AuthenticateRadiusEtape1(vLogin.TxtEmail.value, vLogin.TxtPwd.value, vLogin.CheckBoxRememberMe.checked));
vLogin.OnLoad.BindCommand(aas.Services.Browser.Keyboard.BindEnterKeyToButtonClick(aas.ViewName.Login.TxtPwd, aas.ViewName.Login.BtnLogin));

var vChallenge = Aspectize.CreateView("Challenge", aas.Controls.Challenge);
vChallenge.AddAuthorizationRole(aas.Roles.Anonymous, aas.Enum.AccessControl.ReadWrite);
vChallenge.OnLoad.BindCommand(aas.Services.Browser.Keyboard.BindEnterKeyToButtonClick(aas.ViewName.Challenge.TxtOTP, aas.ViewName.Challenge.BtnValider));
vChallenge.BtnValider.click.BindCommand(aas.Services.Browser.ClientAuthenticationService.AuthenticateRadiusEtape2(vChallenge.TxtOTP.value));


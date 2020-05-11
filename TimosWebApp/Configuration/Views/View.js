var home = Aspectize.CreateView("Home", aas.Controls.Home);

home.Login.click.BindCommand(aas.Services.Browser.UIService.ShowView(aas.ViewName.Login));
home.Logout.click.BindCommand(aas.Services.Browser.SecurityServices.SignOut());
home.Logout.click.BindCommand(aas.Services.Browser.UIService.Refresh());
home.UserName.BindData(aas.Data.MainData.User.Name);
home.DisplayAuthenticated.BindData(aas.Expression(IIF(aas.Data.MainData.User.IsAuthentificated, '', 'hidden')));
home.DisplayUnauthenticated.BindData(aas.Expression(IIF(aas.Data.MainData.User.IsAuthentificated, 'hidden', '')));
home.Home.click.BindCommand(aas.Services.Browser.UIService.ShowView(aas.ViewName.ListeTodos));

var login = Aspectize.CreateView("Login", aas.Controls.Login);
login.AddAuthorizationRole(aas.Roles.Anonymous, aas.Enum.AccessControl.ReadWrite);
login.BtnLogin.click.BindCommand(aas.Services.Browser.SecurityServices.Authenticate(login.TxtEmail.value, login.TxtPwd.value, login.CheckBoxRememberMe.checked));
//login.BtnLogin.click.BindCommand(aas.Services.Browser.ClientInscriptionService.AfterLogin(aas.ViewName.Login.TxtPwd));


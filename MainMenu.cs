using Cascade.Bootstrap.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Core.Navigation.Models;
using Orchard.Core.Title.Models;
using Orchard.Localization;
using Orchard.UI.Navigation;
using System;
using System.Linq;


namespace Cascade.Bootstrap
{
    public class MainMenu : IMenuProvider
    {
        private readonly IContentManager _contentManager;
        private readonly IOrchardServices _orchardServices;

        public MainMenu(IContentManager contentManager, IOrchardServices orchardServices)
        {
            _contentManager = contentManager;
            _orchardServices = orchardServices;
        }

        public Localizer T { get; set; }
        public void GetMenu(IContent menu, NavigationBuilder builder)
        {
            var workContext = _orchardServices.WorkContext;
            var bootstrapSettings = workContext.CurrentSite.As<BootstrapThemeSettingsPart>();


            if (menu.As<TitlePart>().Title == "Main Menu")
            {
                var menuParts = _contentManager.Query<MenuPart, MenuPartRecord>().Where(x => x.MenuId == menu.Id).List();
                var itemCount = menuParts.Select(x => GetFirstInteger(x.MenuPosition)).Max() + 1;

                //do we want to display admin menu?
                if (bootstrapSettings.ShowLogInLinksInMenu)
                {
                    if (_orchardServices.WorkContext.CurrentUser != null)
                    {
                        var adminAccess = _orchardServices.Authorizer.Authorize(Orchard.Security.StandardPermissions.AccessAdminPanel);

                        builder.Add(T(FirstWord(_orchardServices.WorkContext.CurrentUser.UserName, bootstrapSettings)), itemCount.ToString(), item => item.Url("#").AddClass("menuUserName"));

                        // HACK: for CBCA, prevent Members from changing the password because it's a shared login
                        if(adminAccess || bootstrapSettings.Swatch != "cbca")
                            builder.Add(T("Change Password"), itemCount.ToString() + ".1", item => item.Action("ChangePassword", "Account", new { area = "Orchard.Users" }));

                        builder.Add(T("Sign Out"), itemCount.ToString() + ".2", item => item.Action("LogOff", "Account", new { area = "Orchard.Users", ReturnUrl = _orchardServices.WorkContext.HttpContext.Request.RawUrl }));
                        if (adminAccess)
                        {
                            builder.Add(T("Dashboard"), itemCount.ToString() + ".3", item => item.Action("Index", "Admin", new { area = "Dashboard" }));

                            // HACK: for CBCA, add a Help menu item to the menu
                            if (bootstrapSettings.Swatch == "cbca")
                            {
                                builder.Add(T("Help"), itemCount.ToString() + ".4", item => item.Url("Help"));
                            }
                        }
                    }

                    else if (bootstrapSettings.ShowLogInLinksInMenuWhenLoggedIn)
                    {
                        builder.Add(T("Sign In"), itemCount.ToString(), item => item.Action("LogOn", "Account", new { area = "Orchard.Users", ReturnUrl = (_orchardServices.WorkContext.HttpContext.Request.QueryString["ReturnUrl"] ?? _orchardServices.WorkContext.HttpContext.Request.RawUrl) }));
                    }

                }
            }
        }

        private int GetFirstInteger(string pos)
        {
            int result = 0;
            if (!String.IsNullOrWhiteSpace(pos))
            {
                var ints = pos.Split('.');
                if (ints != null && ints.Length > 0)
                    result = Int32.Parse(ints[0]);
            }
            return result;
        }

        // Make user name display more compact for CBCA menu
        private string FirstWord(string name, BootstrapThemeSettingsPart bootstrapSettings)
        {
            var len = name.IndexOf(' ');
            if (len > 0 && bootstrapSettings.Swatch == "cbca")
                return name.Substring(0, len);
            return name;
        }

        public string MenuName { get { return "Main Menu"; } }
    }
}
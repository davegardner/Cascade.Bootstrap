using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
using Cascade.Bootstrap.Models;

namespace Cascade.Bootstrap.Handlers {
    public class BootstrapThemeSettingsPartHandler : ContentHandler {
        public BootstrapThemeSettingsPartHandler() {
            T = NullLocalizer.Instance;
            Filters.Add(new ActivatingFilter<BootstrapThemeSettingsPart>("Site"));
            Filters.Add(new TemplateFilterForPart<BootstrapThemeSettingsPart>("CascadeBootstrapThemeSettings", "Parts/CascadeBootstrapThemeSettings", "CascadeBootstrapTheme"));
        }

        public Localizer T { get; set; }

        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            if (context.ContentItem.ContentType != "Site")
                return;

            base.GetItemMetadata(context);

            // Constructor for GroupInfo is nuts
            var group = new GroupInfo(T("CascadeBootstrapTheme"));
            group.Name = T("Cascade Bootstrap");

            context.Metadata.EditorGroupInfo.Add(group);

        }
    }
}
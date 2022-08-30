using Topomatic.ApplicationPlatform.Plugins;
using Topomatic.Plt.Templates.Common;
using TutorialFields.Profile;

namespace TutorialFields
{
    partial class Module : PluginInitializator
    {
        // Команда вызова процесса регистрации тега продольного профиля
        [cmd("provide_tutorial_profile_fields")]
        private void ProvideProfileFields(TemplateProcessor templateProcessor)
        {
            var provider = new TutorialProfileFieldProvider();
            provider.Provide(templateProcessor);
        }

        // Команда вызова процесса регистрации тега поперечного профиля
        [cmd("provide_tutorial_crossection_fields")]
        private void ProvideCrossectionFields(TemplateProcessor templateProcessor)
        {
            var provider = new TutorialCrossectionFieldProvider();
            provider.Provide(templateProcessor);
        }
    }
}
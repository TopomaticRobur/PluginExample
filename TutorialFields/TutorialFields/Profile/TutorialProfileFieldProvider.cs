using Topomatic.ComponentModel;
using Topomatic.Plt.Templates;
using Topomatic.Plt.Templates.Common;
using TutorialFields.Properties;

namespace TutorialFields.Profile
{
    class TutorialProfileFieldProvider : TemplateFieldProvider
    {
        public override void Provide(TemplateProcessor templateProcessor)
        {
            // Регистрация тега продольного профиля
            var profileFieldDescriptor = templateProcessor.RegisterFieldAlias(
                "TutorialProfileField_MaxMinElevation",
                TypeExplorer.GetSerializableString(typeof(TutorialProfileField)),
                Resources.sTutorialProfileField);
            profileFieldDescriptor.Add("Precision");
        }
    }
}

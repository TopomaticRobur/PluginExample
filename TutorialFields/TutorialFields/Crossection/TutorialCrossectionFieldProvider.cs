using Topomatic.ComponentModel;
using Topomatic.Plt.Templates;
using Topomatic.Plt.Templates.Common;
using TutorialFields.Profile;
using TutorialFields.Properties;

namespace TutorialFields
{
    internal class TutorialCrossectionFieldProvider : TemplateFieldProvider
    {
        public override void Provide(TemplateProcessor templateProcessor)
        {
            // Регистрация тега поперечного профиля
            var crossectionFieldDescriptor = templateProcessor.RegisterFieldAlias(
                "TutorialCrossectionField_MaxMinElevation",
                TypeExplorer.GetSerializableString(typeof(TutorialCrossectionField)),
                Resources.sTutorialCrossectionField);
            crossectionFieldDescriptor.Add("Precision");
        }
    }
}
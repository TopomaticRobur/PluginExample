using System.IO;
using Topomatic.Alg;
using Topomatic.Alg.Model;
using Topomatic.Alg.Runtime.ServiceClasses;
using Topomatic.ApplicationPlatform;
using Topomatic.ApplicationPlatform.Core;
using Topomatic.ApplicationPlatform.Plugins;
using Topomatic.Tables;
using Topomatic.Tables.Export;
using TutorialSheets.Sheet;

namespace TutorialSheets
{
    partial class Module : PluginInitializator
    {
        // Команда создания ведомости
        [cmd("generate_tutorial_sheet")]
        private UserSheet GenerateTutorialSheet(object[] args)
        {
            var m = args[0] as IProjectModel;
            if (m != null)
            {
                var am = m.LockRead() as AlignmentModel;
                if (am != null)
                {
                    var name = ApplicationHost.Current.Plugins.Execute("getname", new object[] { m }) as string;
                    TemplateSheet sht = new TutorialSheet(name, am.Alignment);
                    sht.TemplateRelativePath = Path.Combine("Alg", "TutorialSheet");
                    sht.TemplateFileName = Path.Combine(sht.TemplatesPath, "TutorialSheet.xml");
                    return sht;
                }
            }
            return null;
        }
        
        // Команда вызова мастера создания ведомости
        [cmd("tutorial_sheet")]
        private void TutorialSheet()
        {
            using (var reciver = ActiveAlignmentReciver<Alignment>.CreateReciver(true))
            {
                var alignment = reciver.Alignment;
                if (alignment != null)
                {
                    ApplicationHost.Current.Plugins.Execute(TableConsts.TABLES_SHEET_FUNCTION,
                        new object[] { "generate_tutorial_sheet", reciver.ProjectModel });
                }
            }
        }
    }
}
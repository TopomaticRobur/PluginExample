using System;
using Topomatic.Alg.Runtime.ServiceClasses;
using Topomatic.ApplicationPlatform.Plugins;
using Topomatic.Plt.Templates.Common;

namespace TutorialFields
{
    public class ModulePluginHost : PluginHostInitializator
    {
        protected override Type[] GetTypes()
        {
            return new Type[] { typeof(Module) };
        }

        public override void Initialize(PluginFactory factory)
        {
            base.Initialize(factory);
            factory.RegisterTask(AlgCoreTools.TASK_PLT_FIELDS,
                $"{TemplateDwgGenerator.ID_PRF_RAIL}:provide_tutorial_profile_fields");
            factory.RegisterTask(AlgCoreTools.TASK_PLT_FIELDS,
                $"{TemplateDwgGenerator.ID_CRS_RAIL}:provide_tutorial_crossection_fields");
        }
    }
}

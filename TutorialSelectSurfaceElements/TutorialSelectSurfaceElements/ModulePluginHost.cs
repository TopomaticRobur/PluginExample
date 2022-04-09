using System;
using Topomatic.ApplicationPlatform.Plugins;

namespace TutorialSelectSurfaceElements
{
    public class ModulePluginHost : PluginHostInitializator
    {
        protected override Type[] GetTypes()
        {
            return new Type[] { typeof(Module) };
        }
    }
}

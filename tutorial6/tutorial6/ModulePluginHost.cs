using System;

namespace tutorial6
{
    public class ModulePluginHost : Topomatic.ApplicationPlatform.Plugins.PluginHostInitializator
    {
        //тут мы возвращаем типы всех модулей, которые хотим подключить, в нашем примере только один тип
        protected override Type[] GetTypes()
        {
            return new Type[] { typeof(Module) };
        }
    }
}

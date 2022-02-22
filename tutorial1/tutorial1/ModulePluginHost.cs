using System;
using Topomatic.ApplicationPlatform.Plugins;
using Topomatic.Controls.Dialogs;

namespace tutorial1
{
    public class ModulePluginHost : Topomatic.ApplicationPlatform.Plugins.PluginHostInitializator
    {
        //тут мы возвращаем типы всех модулей, которые хотим подключить, в нашем примере только один тип
        protected override Type[] GetTypes()
        {
            return new Type[] { typeof(Module) };
        }

        //этот метод будет вызван в момент старта программного комплекса
        //при первой инициализации вашего модуля
        //после этого модуль будет закэширован
        public override void Initialize(PluginFactory factory)
        {
            base.Initialize(factory);
            MessageDlg.Show("Привет Робур");
        }
    }
}

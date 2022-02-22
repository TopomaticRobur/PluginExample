using System.Collections.Generic;
using Topomatic.ApplicationPlatform.Plugins;

namespace tutorial5
{
    partial class Module : Topomatic.ApplicationPlatform.Plugins.PluginInitializator
    {
        //Используем статический список для хранения наших данных в качестве модели
        private static List<DataValue> m_Values = new List<DataValue>();

        [cmd("test_dlg_and_table")]
        public void TestDlgAndTable()
        {
            //Просто вызываем метод у нашего диалога
            TestSimpleDialog.Execute(m_Values);
        }
    }
}

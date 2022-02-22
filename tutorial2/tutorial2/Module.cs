using Topomatic.ApplicationPlatform.Plugins;
using Topomatic.Controls.Dialogs;

namespace tutorial2
{
    partial class Module : Topomatic.ApplicationPlatform.Plugins.PluginInitializator
    {
        [cmd("test_cmd")]
        public void ShowPrms(string prms)
        {
            MessageDlg.Show(prms);
        }

        [cmd("test_flags_cmd")]
        public bool IsVisible(string prms)
        {
            return !prms.Equals("Выключить");
        }
    }
}

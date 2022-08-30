using Topomatic.Tables.Export;
using TutorialSheets.Properties;

namespace TutorialSheets.Sheet
{
    partial class TutorialSheetFrame : UserSheetWizardFrame
    {
        public TutorialSheetFrame()
        {
            InitializeComponent();
        }

        public override void OnInitialize(UserSheet sheet)
        {
            base.OnInitialize(sheet);
            var sht = sheet as TutorialSheet;
            if (sht != null)
            {
                cbUserSetting.Checked = sht.UserSetting;
            }
        }

        public override bool OnFinallize(UserSheet sheet)
        {
            var sht = sheet as TutorialSheet;
            if (sht != null)
            {
                sht.UserSetting = cbUserSetting.Checked;
            }
            return base.OnFinallize(sheet);
        }

        public override string Title
        {
            get
            {
                return Resources.sTutorialFrame;
            }
        }
    }
}

using System.ComponentModel;
using System.Diagnostics;
using TutorialFields.Properties;

namespace TutorialFields
{
    sealed class SRDisplayNameAttribute : DisplayNameAttribute
    {
        public SRDisplayNameAttribute(string displayName) : base(Resources.ResourceManager.GetString(displayName))
        {
            var s = Resources.ResourceManager.GetString(displayName);
            if (string.IsNullOrEmpty(s)) Debug.Fail("DisplayName category not found");
        }
    }
}

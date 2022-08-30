namespace TutorialSheets.Sheet
{
    partial class TutorialSheetFrame
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TutorialSheetFrame));
            this.cbUserSetting = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cbUserSetting
            // 
            resources.ApplyResources(this.cbUserSetting, "cbUserSetting");
            this.cbUserSetting.Name = "cbUserSetting";
            this.cbUserSetting.UseVisualStyleBackColor = true;
            // 
            // TutorialSheetFrame
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbUserSetting);
            this.Name = "TutorialSheetFrame";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbUserSetting;
    }
}

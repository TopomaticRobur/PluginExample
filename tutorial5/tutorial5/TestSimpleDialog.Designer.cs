namespace tutorial5
{
    partial class TestSimpleDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new Topomatic.Controls.Dialogs.EditTableFrame();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(353, 228);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(272, 228);
            // 
            // dividerLine
            // 
            this.dividerLine.Location = new System.Drawing.Point(0, 220);
            this.dividerLine.Size = new System.Drawing.Size(440, 2);
            // 
            // panel1
            // 
            this.panel1.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.ReadOnlyMode = false;
            this.panel1.Selected = -1;
            this.panel1.SelectedCount = 0;
            this.panel1.Size = new System.Drawing.Size(416, 202);
            this.panel1.TabIndex = 101;
            this.panel1.Wrapper = null;
            // 
            // TestSimpleDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 263);
            this.Controls.Add(this.panel1);
            this.Name = "TestSimpleDialog";
            this.Text = "TestSimpleDialog";
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.dividerLine, 0);
            this.Controls.SetChildIndex(this.panel1, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private Topomatic.Controls.Dialogs.EditTableFrame panel1;
    }
}
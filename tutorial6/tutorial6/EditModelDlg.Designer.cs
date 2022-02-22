namespace tutorial6
{
    partial class EditModelDlg
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
            this.label1 = new System.Windows.Forms.Label();
            this.tbStringValue = new System.Windows.Forms.TextBox();
            this.cbBooleanValue = new System.Windows.Forms.CheckBox();
            this.tbDoubleValue = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbIntValue = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbArrayValues = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(312, 224);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(231, 224);
            // 
            // dividerLine
            // 
            this.dividerLine.Location = new System.Drawing.Point(0, 216);
            this.dividerLine.Size = new System.Drawing.Size(399, 2);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 13);
            this.label1.TabIndex = 101;
            this.label1.Text = "Строковое значение";
            // 
            // tbStringValue
            // 
            this.tbStringValue.Location = new System.Drawing.Point(148, 15);
            this.tbStringValue.Name = "tbStringValue";
            this.tbStringValue.Size = new System.Drawing.Size(239, 20);
            this.tbStringValue.TabIndex = 102;
            // 
            // cbBooleanValue
            // 
            this.cbBooleanValue.AutoSize = true;
            this.cbBooleanValue.Location = new System.Drawing.Point(15, 93);
            this.cbBooleanValue.Name = "cbBooleanValue";
            this.cbBooleanValue.Size = new System.Drawing.Size(201, 17);
            this.cbBooleanValue.TabIndex = 103;
            this.cbBooleanValue.Text = "Флаг состояния, истина или ложь";
            this.cbBooleanValue.UseVisualStyleBackColor = true;
            // 
            // tbDoubleValue
            // 
            this.tbDoubleValue.Location = new System.Drawing.Point(312, 41);
            this.tbDoubleValue.Name = "tbDoubleValue";
            this.tbDoubleValue.Size = new System.Drawing.Size(75, 20);
            this.tbDoubleValue.TabIndex = 105;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(147, 13);
            this.label2.TabIndex = 104;
            this.label2.Text = "Число с плавающей точкой";
            // 
            // tbIntValue
            // 
            this.tbIntValue.Location = new System.Drawing.Point(312, 67);
            this.tbIntValue.Name = "tbIntValue";
            this.tbIntValue.Size = new System.Drawing.Size(75, 20);
            this.tbIntValue.TabIndex = 107;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 106;
            this.label3.Text = "Целое число";
            // 
            // tbArrayValues
            // 
            this.tbArrayValues.Location = new System.Drawing.Point(15, 129);
            this.tbArrayValues.Multiline = true;
            this.tbArrayValues.Name = "tbArrayValues";
            this.tbArrayValues.Size = new System.Drawing.Size(372, 81);
            this.tbArrayValues.TabIndex = 109;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 113);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(144, 13);
            this.label4.TabIndex = 108;
            this.label4.Text = "Строки с разделителем \";\"";
            // 
            // EditModelDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(399, 259);
            this.Controls.Add(this.tbArrayValues);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbIntValue);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbDoubleValue);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbBooleanValue);
            this.Controls.Add(this.tbStringValue);
            this.Controls.Add(this.label1);
            this.Name = "EditModelDlg";
            this.Text = "Редактор тестовой модели";
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.dividerLine, 0);
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.tbStringValue, 0);
            this.Controls.SetChildIndex(this.cbBooleanValue, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.tbDoubleValue, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.tbIntValue, 0);
            this.Controls.SetChildIndex(this.label4, 0);
            this.Controls.SetChildIndex(this.tbArrayValues, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbStringValue;
        private System.Windows.Forms.CheckBox cbBooleanValue;
        private System.Windows.Forms.TextBox tbDoubleValue;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbIntValue;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbArrayValues;
        private System.Windows.Forms.Label label4;
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Topomatic.Cad.Foundation;
using Topomatic.Controls.Dialogs;

namespace tutorial6
{
    partial class EditModelDlg : SimpleDlg
    {
        private Model m_Model;

        public EditModelDlg()
        {
            InitializeComponent();
        }

        protected override void DoInit()
        {
            //При инициализации диалога назначаем все значения элементам управления
            base.DoInit();
            tbStringValue.Text = m_Model.StringValue;
            tbDoubleValue.Text = ValueConverter.FloatToStr(m_Model.DoubleValue);
            tbIntValue.Text = m_Model.IntValue.ToString();
            cbBooleanValue.Checked = m_Model.BooleanValue;
            for (int i = 0; i < m_Model.ArrayValues.Count; i++)
            {
                tbArrayValues.Text += m_Model.ArrayValues[i];
                if (i < m_Model.ArrayValues.Count)
                    tbArrayValues.Text += ';';
            }
        }

        protected override bool DoCommit()
        {
            //При нажатии клавиши ОК присваиваем все значения
            m_Model.StringValue = tbStringValue.Text;
            m_Model.DoubleValue = ValueConverter.StrToFloat(tbDoubleValue.Text);
            m_Model.IntValue = int.Parse(tbIntValue.Text);
            m_Model.BooleanValue = cbBooleanValue.Checked;
            m_Model.ArrayValues.Clear();
            m_Model.ArrayValues.AddRange(tbArrayValues.Text.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
            return base.DoCommit();
        }

        //Статическая функция для вызова диалога
        public static bool Execute(Model model)
        {
            using (var dlg = new EditModelDlg())
            {
                dlg.m_Model = model;
                return dlg.ShowDialog() == DialogResult.OK;
            }
        }
    }
}

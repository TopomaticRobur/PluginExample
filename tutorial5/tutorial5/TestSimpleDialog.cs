using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace tutorial5
{
    //Наследуем наш диалог от Topomatic.Controls.Dialogs.SimpleDlg
    partial class TestSimpleDialog : Topomatic.Controls.Dialogs.SimpleDlg
    {

        public TestSimpleDialog()
        {
            InitializeComponent();
        }

        protected override void DoInit()
        {
            base.DoInit();
            //на инициализации нам не нужно ничего делать
        }

        protected override bool DoCommit()
        {
            var wrapper = (TableWrapper)panel1.Wrapper;
            //при нажатии ОК проверяем, были ли изменения в таблице
            if (wrapper.IsChanged)
                //Если были, то заполняем данные модели
                wrapper.AcceptChanges();
            return base.DoCommit();
        }

        //Для удобства использования реализуем статический метод, который принимает данные и показывает диалог
        public static bool Execute(List<DataValue> values)
        {
            using (var dlg = new TestSimpleDialog())
            {
                //из-за особенностей реализации необходимо инициализировать фрэйм до вызова ShowDialog()
                dlg.panel1.Wrapper = new TableWrapper(values);
                return dlg.ShowDialog() == DialogResult.OK;
            }
        }

    }
}

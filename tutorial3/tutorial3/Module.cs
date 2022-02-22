using System;
using System.Text;
using Topomatic.ApplicationPlatform.Plugins;
using Topomatic.Cad.Foundation;
using Topomatic.Cad.View.Hints;
using Topomatic.Controls.Dialogs;

namespace tutorial3
{
    partial class Module : Topomatic.ApplicationPlatform.Plugins.PluginInitializator
    {
        [cmd("test_select_objects")]
        public void TestPickObject()
        {
            var cadView = CadView;
            if (cadView != null)
            {
                var selection = cadView.SelectionSet;
                //Предлагаем способ выбора
                string select = "Выбрать по очереди";
                if (CadCursors.GetUserSelect(cadView, ref select, null, "Укажите способ выбора", "Выбрать по очереди", "Выбрать несколько"))
                {
                    //При выборе объектов, можно указать какие именно объекты на экране будут доступны для выбора
                    //Для нашего случая доступны все объекты
                    Predicate<object> match = delegate (object obj)
                    {
                        return true;
                    };
                    switch (select)
                    {
                        case "Выбрать по очереди":
                            {
                                //Функция возвращает выбраный элемент, не помещая его в SelectionSet
                                var obj = selection.PickOneObjectAtScreen(match, "Укажите элемент");
                                while (obj != null)
                                {
                                    MessageDlg.Show(obj.ToString());
                                    obj = selection.PickOneObjectAtScreen(match, "Укажите элемент");
                                }
                            }
                            break;
                        case "Выбрать несколько":
                            {
                                //При вызове функции все выбранные элементы будут помещены в SelectionSet
                                if (selection.SelectObjectsAtScreen(match, "Выберите элементы"))
                                {
                                    var builder = new StringBuilder();
                                    foreach (var obj in selection)
                                    {
                                        builder.AppendLine(obj.ToString());
                                    }
                                    MessageDlg.Show(builder.ToString());
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        [cmd("test_select_points")]
        public void TestPickPoint()
        {
            var cadView = CadView;
            if (cadView != null)
            {
                Vector3D pos;
                while (CadCursors.GetPoint(cadView, out pos, "Укажите точку"))
                {
                    //для вывода значений можно использовать методы класса ValueConverter
                    MessageDlg.Show(String.Format("X:{0}, Y:{1}", ValueConverter.CoordinateToStr(pos.X), 
                        ValueConverter.CoordinateToStr(pos.Y)));
                    bool show_length = true;
                    if (CadCursors.GetBoolean(cadView, ref show_length, null, "Измерить длину"))
                    {
                        if (show_length)
                        {
                            double length = 0.0;
                            if (CadCursors.GetLength(cadView, pos, ref length, "Укажите вторую точку") == Topomatic.Cad.View.GetPointResult.Accept)
                            {
                                MessageDlg.Show(ValueConverter.LengthToStr(length));
                            }
                        }
                    }
                }
            }
        }
    }
}

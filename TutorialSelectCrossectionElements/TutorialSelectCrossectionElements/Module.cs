using System;
using System.Linq;
using System.Windows.Forms;
using Topomatic.Alg;
using Topomatic.Alg.Runtime.ServiceClasses;
using Topomatic.ApplicationPlatform;
using Topomatic.ApplicationPlatform.Plugins;
using Topomatic.Cad.Foundation;
using Topomatic.Cad.View;
using Topomatic.Cad.View.Hints;
using Topomatic.Controls.Dialogs;
using Topomatic.Crs.Design;
using Topomatic.Crs.Templates;

namespace TutorialSelectCrossectionElements
{
    partial class Module : PluginInitializator
    {
        //метод принимает на вход комманду и окно, и возвращает True если окно устраивает
        protected override bool ValidateWindow(string cmd, IDocumentWindow window)
        {
            switch (cmd)
            {
                case "select_construction_nodes_by_code":
                case "get_construction_contours_by_code":
                case "calculate_construction_contours_intersection":
                    //выбираем окно поперечников
                    return window.UID.Equals(Consts.CrossWindow, StringComparison.InvariantCultureIgnoreCase);
                default:
                    //по умолчанию мы выбираем текущее окно
                    return base.ValidateWindow(cmd, window);
            }
        }

        [cmd("select_construction_nodes_by_code")]
        private void SelectConstructionNodesByCode()
        {
            //Получаем модель текущей трассы. Если текущей является не трасса, то активируется последняя активная трасса
            Alignment alignment;
            using (var receiver = ActiveAlignmentReciver<Alignment>.CreateReciver(false))
            {
                alignment = receiver.Alignment;
                if (alignment == null)
                {
                    MessageDlg.Show("Необходимо сделать трассу активной.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    return;
                }
                var cadview = CadView;
                if (cadview == null) return;

                // Выбор точек по коду
                var code = 0;
                var getPointRes = CadCursors.GetInteger(cadview, ref code, "Введите код точки:");
                if (getPointRes == GetPointResult.Cancel) return;

                var ss = CadView.SelectionSet;
                ss.Clear();
                ss.SelectAll();
                ss.FilterSelected(o =>
                {
                    var wrapper = o as CrsDesignEntityWrapper;
                    if (wrapper == null) return false;

                    var node = wrapper.Component as CrsNode;
                    if (node == null) return false;

                    return node.Code.Equals(code);
                });
            }
        }

        [cmd("get_construction_contours_by_code")]
        private void GetConstructionContoursByCode()
        {
            //Получаем модель текущей трассы. Если текущей является не трасса, то активируется последняя активная трасса
            Alignment alignment;
            using (var receiver = ActiveAlignmentReciver<Alignment>.CreateReciver(false))
            {
                alignment = receiver.Alignment;
                if (alignment == null)
                {
                    MessageDlg.Show("Необходимо сделать трассу активной.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    return;
                }

                //Переключаемся на вкладку поперечников и указываем код для поиска контуров
                IDocumentWindow window;
                if (!ApplicationHost.Current.ActiveProject.TryGetWindow(Consts.CrossWindow, out window)) return;
                window.Activate();

                var cadview = CadView;
                if (cadview == null) return;

                var code = 0;
                var getPointRes = CadCursors.GetInteger(cadview, ref code, "Введите код контура:");
                if (getPointRes == GetPointResult.Cancel) return;

                // Получение контуров по коду и расчёт их площади
                var corridor = alignment.Corridor;
                var index = receiver.Manager.CurrentSection;
                var context = corridor[index];
                if (context == null) return;

                var contours = context.FindContour(code);
                if (contours == null || !contours.Any())
                {
                    MessageDlg.Show($"Контуры с кодом {code} не найдены.");
                    return;
                }

                var commonArea = 0.0;
                foreach (CrsContour contour in contours)
                    commonArea += CadLibrary.PolygonArea(contour.AsVectorList());

                MessageDlg.Show($"Общая площадь контуров с кодом {code} = {ValueConverter.AreaToStr(commonArea)}");
            }
        }

        [cmd("calculate_construction_contours_intersection")]
        private void CalculateConstructionContoursIntersection()
        {
            //Получаем модель текущей трассы. Если текущей является не трасса, то активируется последняя активная трасса
            Alignment alignment;
            using (var receiver = ActiveAlignmentReciver<Alignment>.CreateReciver(false))
            {
                alignment = receiver.Alignment;
                if (alignment == null)
                {
                    MessageDlg.Show("Необходимо сделать трассу активной.", MessageBoxButtons.OK,
                        MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    return;
                }

                var cadview = CadView;
                if (cadview == null) return;

                //Получаем контуры поверхности земли и проектной линии и указываем тип рассчёта
                var corridor = alignment.Corridor;

                var index = receiver.Manager.CurrentSection;
                var context = corridor[index];
                if (context == null) return;

                var eg = context.GetEgContour().AsVectorList();
                if (eg == null || eg.Count == 0)
                {
                    MessageDlg.Show("Поверхность земли отсутствует.", MessageBoxButtons.OK, MessageBoxIcon.Information,
                        MessageBoxDefaultButton.Button1);
                    return;
                }

                var red = context.GetRedLineContour().AsVectorList();
                if (red == null || red.Count == 0)
                {
                    MessageDlg.Show("Проектная линия отсутствует.", MessageBoxButtons.OK, MessageBoxIcon.Information,
                        MessageBoxDefaultButton.Button1);
                    return;
                }

                var userSelect = "Насыпи";
                if (!CadCursors.GetUserSelect(cadview, ref userSelect, null, "Рассчитать площадь: ", "Насыпи",
                    "Выемки")) return;

                //Рассчитываем площадь пересечения
                var area = 0.0;
                if (userSelect.Equals("Насыпи", StringComparison.OrdinalIgnoreCase))
                {
                    eg.Add(eg[eg.Count - 1] + new Vector2D(0.0, 100.0));
                    eg.Add(eg[0] + new Vector2D(0.0, 100.0));

                    red.Add(red[red.Count - 1] + new Vector2D(0.0, -100.0));
                    red.Add(red[0] + new Vector2D(0.0, -100.0));
                }
                else
                {
                    eg.Add(eg[eg.Count - 1] + new Vector2D(0.0, -100.0));
                    eg.Add(eg[0] + new Vector2D(0.0, -100.0));

                    red.Add(red[red.Count - 1] + new Vector2D(0.0, 100.0));
                    red.Add(red[0] + new Vector2D(0.0, 100.0));
                }

                var intersections = new PolygonOperation().Intersection(eg, red);
                foreach (var intersection in intersections)
                    area += CadLibrary.PolygonArea(intersection);

                MessageDlg.Show($"Площадь {userSelect.ToLower()}: {ValueConverter.AreaToStr(area)}");
            }
        }
    }
}
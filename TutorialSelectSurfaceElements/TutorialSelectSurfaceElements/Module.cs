using System;
using Topomatic.ApplicationPlatform.Plugins;
using Topomatic.Cad.Foundation;
using Topomatic.Cad.View;
using Topomatic.Controls.Dialogs;
using Topomatic.Sfc.Layer;
using Topomatic.Sfc.Layer.Wrappers;

namespace TutorialSelectSurfaceElements
{
    partial class Module : PluginInitializator
    {
        [cmd("calculate_average_elevation")]
        private void CalculateAverageElevation()
        {
            //Находим активный SurfaceLayer
            var cadview = CadView;
            var actSfcLayer = SurfaceLayer.GetSurfaceLayer(cadview);
            if (actSfcLayer == null) return;
            
            //Выбираем точки поверхности
            if (actSfcLayer.SelectedPointsCount == 0)
            {
                var res = actSfcLayer.SelectPoints(null, "Выберите точки поверхности:");
                if (res == GetPointResult.Cancel) return;
            }
            if (actSfcLayer.SelectedPointsCount == 0) return;

            //Обращаемся к выбранным точкам и получаем сумму отметок
            var sfc = actSfcLayer.Surface;
            var sum = 0.0;
            foreach (var selectedPoint in actSfcLayer.GetSelectedPoints())
            {
                var point = sfc.Points[selectedPoint];
                sum += point.Vertex.Elevation;
            }

            //Вычисляем среднюю отметку и отображаем её в диалоговом окне
            var average = sum / actSfcLayer.SelectedPointsCount;
            MessageDlg.Show($"Средняя отметка: {average}");
        }

        [cmd("define_steepest_grade")]
        private void DefineSteepestGrade()
        {
            //Находим активный SurfaceLayer
            var cadview = CadView;
            var actSfcLayer = SurfaceLayer.GetSurfaceLayer(cadview);
            if (actSfcLayer == null) return;
            
            //Выбираем структурную линию
            var strLine = actSfcLayer.SelectOneStructureLine(null, "Выберите структурную линию:");
            if (strLine == null) return;

            //Обращаемся к узлам структурной линии, находим соответствующие им точки поверхности
            //и высчитываем уклоны сегментов по пути определяя максимальный уклон
            var sfc = actSfcLayer.Surface;
            var maxIncline = 0.0;
            for (var i = 0; i < strLine.Count - 1; i++)
            {
                var stPoint = sfc.Points[strLine[i].Index];
                var endPoint = sfc.Points[strLine[i + 1].Index];
                var delta = Math.Abs(stPoint.Vertex.Elevation - endPoint.Vertex.Elevation);

                var stPos = stPoint.Vertex.Pos;
                var endPos = endPoint.Vertex.Pos;
                var length = (endPos - stPos).Length;
                
                if (ValueConverter.CompValues(length, 0.0) != 0)
                {
                    var incline = delta / length;
                    if (incline > maxIncline) maxIncline = incline;
                }
            }

            MessageDlg.Show($"Максимальный уклон: {maxIncline}");
        }

        [cmd("get_horizontal_line_area")]
        private void GetHorizontalLineArea()
        {
            //Находим активный SurfaceLayer
            var cadview = CadView;
            var actSfcLayer = SurfaceLayer.GetSurfaceLayer(cadview);
            if (actSfcLayer == null) return;

            //Включаем возможность выбор горизонталей
            var initSelectState = SurfaceLayer.SurfaceSelectionSet.IsHorizontalSelectable;
            SurfaceLayer.SurfaceSelectionSet.IsHorizontalSelectable = true;

            //Получаем указатель на текущий набор объектов
            var ss = actSfcLayer.SelectionSet;

            //Получаем указатель на слой горизонталей
            var layer = actSfcLayer.Surface.Style.HorizontalsStyle.GetLayer();

            //Выбираем горизонталь на плане
            var linear_object = ss.PickOneObjectAtScreen(obj =>
            {
                if (obj is SurfaceObjectWrapper layered_object)
                {
                    return layered_object.LayerID == layer.ObjectID;
                }
                return false;
            }, "Выберите замкнутую горизонталь") as ILinearObject;

            //Восстанавливаем состояние флага возможности выбора горизонталей
            SurfaceLayer.SurfaceSelectionSet.IsHorizontalSelectable = initSelectState;
            if (linear_object == null) return;

            //Извлекаем полилинию из горизонтали и получаем её площадь
            var poly = new Polyline3D();
            linear_object.GetPolyline(poly);

            //Проверяем замкнута ли горизонталь
            var startVec = poly[0];
            var endVec = poly[poly.Count - 1];
            if (startVec.EqualEps(endVec))
            {
                var area = poly.GetArea2D();
                MessageDlg.Show($"Площадь горизонтали: {area}");
            }
            else MessageDlg.Show($"Горизонталь не замкнута.");
        }
    }
}
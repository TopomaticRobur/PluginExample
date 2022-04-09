using System;
using System.Collections.Generic;
using System.Linq;
using Topomatic.ApplicationPlatform.Plugins;
using Topomatic.Cad.Foundation;
using Topomatic.Cad.View;
using Topomatic.Cad.View.Hints;
using Topomatic.Dwg;
using Topomatic.Dwg.Entities;
using Topomatic.Sfc;
using Topomatic.Sfc.Layer;

namespace TutorialEditSurfaceElements
{
    partial class Module : PluginInitializator
    {

        [cmd("create_hill_or_pit")]
        private void CreateHillOrPit()
        {
            //Находим активный SurfaceLayer
            var cadview = CadView;
            if (cadview == null) return;

            var layer = SurfaceLayer.GetSurfaceLayer(cadview);
            if (layer == null) return;
            var sfc = layer.Surface;

            //Выбираем структурную линию и получаем список её двумерных координат
            var strLine = layer.SelectOneStructureLine(i => sfc.StructureLines[i].IsClosed,
                "Выберите замкнутую структурную линию:");
            if (strLine == null) return;

            var poly = new List<Vector3D>();
            strLine.ToPolyline(poly);
            var poly2d = poly.Select(v => v.Pos).ToList();

            //Делегат для динамической отрисовки контура будущей структурной линии
            DrawCursorEvent ondraw = delegate (CadPen pen, Vector3D vertex)
            {
                double s, o;
                //находим смещение точки по линии
                if (CadLibrary.PosToPolylineStaOffset(poly2d, vertex.Pos, out o, out s))
                {
                    var offs_line = new List<Vector3D>();
                    poly.Offset(o, offs_line);

                    //Создаём примитив и рисуем его
                    var entity = new DwgPolyline();
                    entity.Prepare(sfc.Situation);
                    foreach (var v in offs_line)
                    {
                        entity.Add(new BugleVector2D(v.Pos));
                    }

                    entity.Color = CadColor.Green;
                    PaintEntityEventArgs.PaintEntity(entity, pen);
                }
            };

            //Подписываемся на событие динамической отрисовки примитивов
            cadview.DynamicDraw += ondraw;

            Vector3D point;
            GetPointResult res;
            try
            {
                //Указываем курсором точку на плане определяющую сторону и величину смещения
                //и закладываем возможность задать смещение с клавиатуры
                res = CadCursors.GetPoint(cadview, out point, "Укажите смещение или:", "Задать величину смещения");

            }
            finally
            {
                //Отписываемся от события, так как динамическая отрисовка нам больше не требуется
                cadview.DynamicDraw -= ondraw;
            }

            
            double sta;
            var off = 0.0;
            if (res == GetPointResult.Accept)
            {
                CadLibrary.PosToPolylineStaOffset(poly2d, point.Pos, out off, out sta);
            }
            else if (res == GetPointResult.UserCmd)
            {
                if (CadCursors.GetDouble(cadview, ref off, "Задайте смещение:") == GetPointResult.Cancel) return;
            }
            else if (res == GetPointResult.Cancel) return;

            var delta = 0.0;
            var incline = 0.0;

            //Указываем величину приращения отметок смещённой структурной линии
            //и закладываем возможность задать уклон откоса
            //В случае если пользователь просто нажмёт Enter то функция вернёт UserCmd
            res = CadCursors.GetDouble(cadview, ref delta, "Укажите разницу высот или:", "Задать уклон");

            if (res == GetPointResult.UserCmd && cadview.LastUserCmd != "")
            {
                if (CadCursors.GetDouble(cadview, ref incline, "Укажите уклон:") == GetPointResult.Cancel) return;
                delta = incline * Math.Abs(off);
            }
            else if (res == GetPointResult.Cancel) return;

            //Определяем эквидистанту в соответствии с параметрами указанными пользователем
            var offsetPoly = new List<Vector3D>();
            poly.Offset(off, offsetPoly);
            offsetPoly.Remove(offsetPoly[offsetPoly.Count - 1]);

            //Фиксируем момент начала внесения изменений в поверхность
            sfc.BeginUpdate("Построить насыпь или котлован");

            //Создаём точки поверхности и добавляем их в структурную линию
            try
            {
                var offsetStrLine = new StructureLine();
                var editor = new PointEditor(sfc);
                foreach (var polylinePoint in offsetPoly)
                {
                    var sfcPoint = new SurfacePoint(polylinePoint + new Vector3D(0.0, 0.0, delta));
                    sfcPoint.IsSituation = false;
                    var sfcPointIndex = editor.Add(sfcPoint);
                    offsetStrLine.Add(sfcPointIndex);
                }

                offsetStrLine.IsClosed = true;
                sfc.StructureLines.Add(offsetStrLine);
            }
            finally
            {
                //Фиксируем окончание внесения изменений в поверхность
                sfc.EndUpdate();
            }

            //Очищаем список выбранных элементов поверхности
            //и сообщаем CadView о необходиомсти обновить отображение его содержимого
            layer.SelectionSet.Clear();
            cadview.Unlock();
            cadview.Invalidate();
        }

        [cmd("change_point_semantic_code")]
        private void ChangePointSemanticCode()
        {
            //Находим активный SurfaceLayer
            var cadview = CadView;
            if (cadview == null) return;

            var layer = SurfaceLayer.GetSurfaceLayer(cadview);
            if (layer == null) return;

            //Выбираем точку поверхности
            int ind;
            var res = layer.PickOnePoint(null, out ind, "Выберите точку поверхности:");
            if (res == GetPointResult.Cancel) return;

            //Задаём отметку
            var elev = 0.0;
            res = CadCursors.GetDouble(cadview, ref elev, "Задайте отметку центра:");
            if (res == GetPointResult.Cancel) return;

            //Фиксируем момент начала внесения изменений в поверхность
            var sfc = layer.Surface;
            sfc.BeginUpdate("Преобразование точки поверхности в пункт гос. геод. сети");

            try
            {
                //Задаём точке код объекта и присваиваем значение свойству 
                var info = sfc.Points.GetExtensiveInformation(ind);
                info.Code = 1000;

                var tags = new Dictionary<string, string>();
                info.Semantic.GetStringTags(tags);
                if (tags.ContainsKey("CENTER")) info.Semantic["CENTER"] = elev;

                sfc.RefreshPointSign(info);
            }
            finally
            {
                //Фиксируем окончание внесения изменений в поверхность
                sfc.EndUpdate();
            }
                                
            //Очищаем список выбранных элементов поверхности
            //и сообщаем CadView о необходиомсти обновить отображение его содержимого
            layer.SelectionSet.Clear();
            cadview.Unlock();
            cadview.Invalidate();
        }
    }
}
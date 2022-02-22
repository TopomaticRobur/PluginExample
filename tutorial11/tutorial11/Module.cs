using Topomatic.ApplicationPlatform.Plugins;
using Topomatic.Cad.Foundation;
using Topomatic.Cad.View;
using Topomatic.Cad.View.Hints;
using Topomatic.Dwg;
using Topomatic.Dwg.Entities;
using Topomatic.Dwg.Layer;

namespace tutorial11
{
    partial class Module : Topomatic.ApplicationPlatform.Plugins.PluginInitializator
    {
        //функция создает экземпляр редактора модели и возвращает его
        [cmd("draw_test_block")]
        private void DrawBlock()
        {
            //находим текущий слой чертежа
            var cadView = CadView;
            var layer = DrawingLayer.GetDrawingLayer(cadView);
            if (layer != null)
            {
                //определяем имя нашего блока
                const string BLOCK_NAME = "test_block";
                var drawing = layer.Drawing;
                //начинаем групповое изменение чертежа
                drawing.BeginUpdate();
                try
                {
                    //находим блок с нужным именем
                    var block = drawing.Blocks[BLOCK_NAME];
                    if (block == null)
                    {
                        //если такого блока нет, создаем его
                        block = drawing.Blocks.Add(BLOCK_NAME);
                        //рисуем внутри блока круг
                        var circle = block.AddCircle(new Vector3D(0, 0, 0), 1.0);
                        circle.Color = CadColor.ByBlock;
                        //и квадрат
                        var line = block.AddPolyline(new Vector2D[] { new Vector2D(-1.0, -1.0), new Vector2D(-1.0, 1.0), new Vector2D(1.0, 1.0), new Vector2D(1.0, -1.0) });
                        line.Color = CadColor.ByBlock;
                        line.Closed = true;
                    }
                    Vector3D block_pos;
                    while (CadCursors.GetPoint(cadView, out block_pos, "Выберите положение блока"))
                    {
                        //вставляем блок в указанных точках чертежа
                        drawing.ActiveSpace.AddInsert(block_pos, new Vector3D(1.0), 0.0, BLOCK_NAME);
                        cadView.Unlock();
                        cadView.Invalidate();
                    }
                }
                finally
                {
                    //заканчиваем групповое изменение чертежа
                    drawing.EndUpdate();
                }
            }
        }

        [cmd("draw_test_linetype")]
        private void DrawLineType()
        {
            var cadView = CadView;
            //находим текущий слой чертежа
            var layer = DrawingLayer.GetDrawingLayer(cadView);
            if (layer != null)
            {
                //определяем имя типа линии
                const string LINETYPE_NAME = "test_linetype";
                var drawing = layer.Drawing;
                //начинаем групповое изменение чертежа
                drawing.BeginUpdate();
                try
                {
                    //находим тип линии с нужным именем
                    var linetype = drawing.Linetypes[LINETYPE_NAME];
                    if (linetype == null)
                    {
                        //если такого типа линии нет, создаем его шаблон и добавляем в таблицу типов линий
                        var pattern = new LinetypePattern();
                        //шаблон линии состоит из штриха в 1 еденицу чертежа
                        pattern.Add().DashDotLength = 1;
                        //и промежутка в 0.5 едениц чертежа
                        pattern.Add().DashDotLength = -0.5;
                        //добавляем линию в таблицу линий
                        linetype = drawing.Linetypes.Add(LINETYPE_NAME, "Тестовый шаблон линии", pattern);
                    }
                    //создаем объект полилинии
                    var polyline = new DwgPolyline();
                    //вызываем функцию подготовки примитива
                    polyline.Prepare(drawing);
                    //назначаем тип линии
                    polyline.Linetype = linetype;

                    //создаем процедуру динамической отрисовки, для удобства ввода
                    DrawCursorEvent dynamic_draw = delegate (CadPen pen, Vector3D vertex)
                    {
                        if (polyline.Count > 0)
                        {
                            //отрисовываем примитив
                            PaintEntityEventArgs.PaintEntity(polyline, pen);
                            //рисуем линию от последней точки полилинии до текущей позиции курсорва
                            pen.DrawLine(polyline[polyline.Count - 1].Vertex, vertex.Pos);
                        }
                    };

                    //назначаем процедуру динамической отрисовки
                    cadView.DynamicDraw += dynamic_draw;
                    try
                    {
                        Vector3D pos;
                        while (CadCursors.GetPoint(cadView, out pos, "Укажите точку в линии"))
                        {
                            //добавлеям точку в полилинию
                            polyline.Add(new BugleVector2D(pos.Pos));
                        }
                        //добавляем полилинию на чертёж, если в ней есть точки
                        if (polyline.Count > 0)
                            drawing.ActiveSpace.Add(polyline);
                    }
                    finally
                    {
                        //убираем процедуру динамической отрисовки
                        cadView.DynamicDraw -= dynamic_draw;
                    }
                }
                finally
                {
                    //заканчиваем групповое изменение чертежа
                    drawing.EndUpdate();
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using Topomatic.ApplicationPlatform.Plugins;
using Topomatic.Cad.Foundation;
using Topomatic.Cad.View;
using Topomatic.Cad.View.Hints;
using Topomatic.Controls.Dialogs;

namespace tutorial4
{
    partial class Module : Topomatic.ApplicationPlatform.Plugins.PluginInitializator
    {
        [cmd("test_dynamic_render")]
        public void TestDynamicRender()
        {
            var cadView = CadView;
            if (cadView != null)
            {
                //список выбранных точек
                var positions = new List<Vector2D>();
                //делегат для динамической отрисовки
                DrawCursorEvent dynamic_draw = delegate (CadPen pen, Vector3D vertex)
                {
                    //если есть точки в списке, нужно их нарисовать и нарисовать линию
                    //от последней выбранной точки, до текущего положения курсора
                    if (positions.Count > 0)
                    {
                        pen.Color = Color.Lime;
                        pen.BeginDraw();
                        try
                        {
                            for (int i = 1; i < positions.Count; i++)
                            {
                                pen.DrawLine(positions[i - 1], positions[i]);
                            }
                            pen.DrawLine(positions[positions.Count - 1], vertex.Pos);
                        }
                        finally
                        {
                            pen.EndDraw();
                        }
                    }
                };
                //подписываемся на событие отрисовки
                cadView.DynamicDraw += dynamic_draw;
                try
                {
                    Vector3D pos;
                    //просим пользователя указать несколько точек
                    while (CadCursors.GetPoint(cadView, out pos, "Укажите точку"))
                    {
                        positions.Add(pos.Pos);
                    }
                    if (positions.Count > 0)
                    {
                        //считаем площадь многоугольника
                        MessageDlg.Show(String.Format("Площадь: {0}", CadLibrary.PolygonArea(positions)));
                    }
                }
                finally
                {
                    //отписываемся от события отрисовки
                    cadView.DynamicDraw -= dynamic_draw;
                }
            }
        }
    }
}

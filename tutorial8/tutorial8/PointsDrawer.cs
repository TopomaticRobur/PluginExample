using System.Drawing;
using Topomatic.Cad.Foundation;

namespace tutorial8
{
    //Сервисный класс для отрисовки точек
    static class PointsDrawer
    {
        //Функция рисующая список точек при помощи pen
        public static void PaintPoints(CadPen pen, Points points, bool enabled)
        {
            //рисуем нашу линию жёлтым цветом
            pen.Color = Color.Yellow;
            //Начинаем рисовать
            pen.BeginDraw();
            try
            {
                //для отрисовки используем возможность нарисовать массив нескольких точек
                //Для этого вызываем начало отрисовки массива
                pen.BeginArray();
                for (int i = 0; i < points.Count; i++)
                {
                    //Добавляем точки
                    pen.Vertex(points[i]);
                }
                //Заканчиваем отрисовку массива, в виде линии
                pen.EndArray(ArrayMode.Polyline);
            }
            finally
            {
                //заканчиваем рисовать
                pen.EndDraw();
            }
            //в каждой точке пишем номер оранжевым цветом
            pen.Color = Color.Orange;
            //начинаем рисовать
            pen.BeginDraw();
            try
            {
                var font = FontManager.Current.DefaultFont;
                for (int i = 0; i < points.Count; i++)
                {
                    //пишем номер точки, высотой в 2 еденицы чертежа
                    font.DrawString(i.ToString(), pen, points[i], 0.0, 2.0);
                }
            }
            finally
            {
                //заканчиваем рисовать
                pen.EndDraw();
            }
        }
    }
}

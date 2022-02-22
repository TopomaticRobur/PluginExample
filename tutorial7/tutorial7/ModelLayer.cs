using System;
using System.Drawing;
using Topomatic.ApplicationPlatform;
using Topomatic.Cad.Foundation;
using Topomatic.Cad.View;

namespace tutorial7
{
    class ModelLayer : CadViewLayer
    {

        //для удобства определяем статический метод, позволяющий получить наш слой с видового экрана
        public static ModelLayer GetModelLayer(CadView cadView)
        {
            //сначала проверяем, есть ли наш слой сразу в самом видовом экране
            //это возможно, если видовой экран создан отдельно и слой расположен прямо на видовом экране
            var layer = cadView[ModelLayer.ID] as ModelLayer;
            if (layer == null)
            {
                //теперь проверяем не находится ли слой в составе нескольких слоёв модели
                //это наиболее распространённая ситуация
                //для этого мы получаем слой, который содержит внутри все слои всех моделей
                var multi = cadView[Consts.ModelsLayer] as MultiLayer;
                if (multi != null)
                {
                    //после этого получаем текущий слой активной модели
                    var active = multi.ResolveActive();
                    //проверяем его на соответствие нашему слою
                    layer = active as ModelLayer;
                    if (layer == null)
                    {
                        //кроме того возможен вариант, что у нашей модели несколько слоев
                        //в этом случае они объединяются внутри общего слоя модели, который и будет являться активным
                        var compound = active as CompoundLayer;
                        if (compound != null)
                        {
                            layer = compound[ModelLayer.ID] as ModelLayer;
                        }
                    }
                }
            }
            //если наш слой найден, но он заблокирован на редактирование, то мы не можем его вернуть
            if ((layer != null) && (!layer.ResolveEnable()))
            {
                return null;
            }
            return layer;
        }

        //наша модель
        private Model m_Model;

        //класс, отвечающий за выделение объектов
        private SelectionSet m_SelectionSet;

        public ModelLayer()
        {
            //в качестве класса, отвечающего за выделение объектов мы используем заглушку которая реализует его по умолчанию, в этом случае он не выделяет ничего
            m_SelectionSet = new DefaultSelectionSet(this);
        }

        //Guid нашего слоя
        public static readonly Guid ID = new Guid("{36C745EB-2111-4D44-B4A1-9BE0B7DBD730}");

        //Возвращаем в качестве LayerId ID объявленный выше
        public override Guid LayerGuid
        {
            get
            {
                return ID;
            }
        }

        //Возвращаем нашу заглушку
        public override SelectionSet SelectionSet
        {
            get
            {
                return m_SelectionSet;
            }
        }

        public override string Name
        {
            get
            {
                return "Слой тестовой модели";
            }
        }

        public Model Model
        {
            get
            {
                return m_Model;
            }
            set
            {
                m_Model = value;
            }
        }

        //Рассчитываем границы слоя
        protected override bool OnGetLimits(out BoundingBox2D limits)
        {
            //Если есть точки в модели
            if (m_Model.Points.Count > 0)
            {
                //Создаем рамку вокруг первой точки
                limits = new BoundingBox2D(m_Model.Points[0], m_Model.Points[0]);
                for (int i = 1; i < m_Model.Points.Count; i++)
                {
                    //и добавляем в нее все остальные точки
                    limits.AddPoint(m_Model.Points[i]);
                }
                return true;
            }
            //если точек в модели нет, возвращаем пустую рамку
            limits = BoundingBox2D.Empty;
            return false;
        }

        protected override void OnGetSnapObjects(ObjectSnapEventArgs e)
        {
            //Поскольку мы не реализуем привязки, то здесь мы не делаем ничего
        }

        protected override void OnPaint(CadPen pen)
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
                for (int i = 0; i < m_Model.Points.Count; i++)
                {
                    //Добавляем точки
                    pen.Vertex(m_Model.Points[i]);
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
                for (int i = 0; i < m_Model.Points.Count; i++)
                {
                    //пишем номер точки, высотой в 2 еденицы чертежа
                    font.DrawString(i.ToString(), pen, m_Model.Points[i], 0.0, 2.0);
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

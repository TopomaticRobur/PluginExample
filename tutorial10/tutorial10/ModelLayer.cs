using System;
using Topomatic.ApplicationPlatform;
using Topomatic.Cad.Foundation;
using Topomatic.Cad.View;

namespace tutorial10
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
            //в качестве класса, отвечающего за выделение объектов мы используем ModelLayerSelectionSet
            m_SelectionSet = new ModelLayerSelectionSet(this);
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
            bool init = false;
            limits = BoundingBox2D.Empty;
            for (int i = 0; i < m_Model.Count; i++)
            {
                var points = m_Model[i];
                for (int j = 0; j < points.Count; j++)
                {
                    var v = points[j];
                    if (init)
                    {
                        limits.AddPoint(v);
                    }
                    else
                    {
                        limits = new BoundingBox2D(v, v);
                        init = true;
                    }
                }
            }
            return init;
        }

        protected override void OnGetSnapObjects(ObjectSnapEventArgs e)
        {
            for (int i = 0; i < m_Model.Count; i++)
            {
                //возвращаем реализации интерфейса IObjectDisjoiner
                e.SnapObjects.Add(m_Model[i]);
            }
        }

        //Отрисовка модели
        protected override void OnPaint(CadPen pen)
        {
            for (int i = 0; i < m_Model.Count; i++)
            {
                PointsDrawer.PaintPoints(pen, m_Model[i], ResolveEnable());
            }
        }

        //Отрисовка подсвеченных объектов
        protected override void OnHilightObject(CadPen pen, object obj)
        {
            base.OnHilightObject(pen, obj);
            //Если объект это наши точки
            var points = obj as Points;
            if (points != null)
            {
                //То включаем режим подсветки
                pen.DrawingMode = DrawingMode.Highlight;
                pen.HighlightMode = HighlightMode.DoubleBlack;
                //Рисуем точки
                PointsDrawer.PaintPoints(pen, points, ResolveEnable());
                pen.Reset();
            }
        }

        //Динамическая отрисовка слоя
        protected override void OnDynamicDraw(CadPen pen, Vector3D location)
        {
            base.OnDynamicDraw(pen, location);
            //Если есть выбранные элементы
            if (m_SelectionSet.Count > 0)
            {
                //Необходимо нарисовать все выбранные элементы
                foreach (var obj in m_SelectionSet)
                {
                    var points = obj as Points;
                    if (points != null)
                    {
                        //Рисуем элементы подсвеченными
                        pen.HighlightMode = HighlightMode.Black;
                        PointsDrawer.PaintPoints(pen, points, ResolveEnable());
                        pen.Reset();
                    }
                }
            }
        }
    }
}

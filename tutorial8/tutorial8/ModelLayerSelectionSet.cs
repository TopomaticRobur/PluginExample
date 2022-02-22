using System;
using System.Collections;
using System.Collections.Generic;
using Topomatic.Cad.Foundation;
using Topomatic.Cad.View;
using Topomatic.Cad.View.Hints;

namespace tutorial8
{

    class ModelLayerSelectionSet : SelectionSet
    {
        //Список выделенных объектов
        private List<object> m_Selected = new List<object>();

        public ModelLayerSelectionSet(ModelLayer layer) 
            : base(layer)
        {
        }

        //Количество выделенных объектов
        public override int Count
        {
            get
            {
                return m_Selected.Count;
            }
        }

        //Метод очищает выделенные объекты
        public override void Clear()
        {
            m_Selected.Clear();
        }

        //Метод удалет выделенные объекты из модели
        public override void Erase()
        {
            var model_layer = (ModelLayer)Layer;
            for (int i = 0; i < m_Selected.Count; i++)
            {
                model_layer.Model.Remove((Points)m_Selected[i]);
            }
            Clear();
        }
        
        //Возвращаем выделенные объекты
        public override IEnumerator GetEnumerator()
        {
            return m_Selected.GetEnumerator();
        }

        //Последний опрошенный элемент списка, необходим для работы GetObjectsAtPoint
        private int m_LastSearchIndex = 0;

        //Метод возвращает список объектов находящихся в выделенной точке и расстояние до них
        //В данном примере функция реализована с поддержкой таймаута
        public override IEnumerable<KeyValuePair<double, object>> GetObjectsAtPoint(Vector3D point, Predicate<object> match, int waitTimeOut)
        {
            //Создаем список объектов
            var list = new List<KeyValuePair<double, object>>();
            var model_layer = (ModelLayer)Layer;
            //Проверяем видимый ли слой и есть ли у него модель
            if (model_layer.ResolveVisible() && (model_layer.Model != null))
            {
                //Запоминаем время начала работы функции
                var t = Environment.TickCount;
                int result = -1;
                var model = model_layer.Model;
                //Формируем вокруг точки прямоугольник зависящий от текущего масштаба видового экрана
                var rect = Layer.CadView.MakeSearchRectangle(point);
                //Создаем контекст отрисовки, для проверки попадания объекта в рамку
                var dc = new NullDeviceContext(Layer.CadView, true);
                dc.BeginRender();
                try
                {
                    //Назначаем в качестве рамки полученный ранее прямоугольник
                    dc.SetClipRect(rect);
                    var count = model.Count;
                    //Поиск начинаем не с начала списка, а с последнего опрошенного элемента
                    var lastSearch = m_LastSearchIndex;
                    for (int i = count - 1; i >= 0; i--)
                    {
                        var index = (i + lastSearch) % count;
                        if (result == -1)
                        {
                            //Если пока элемены не найдены, то запоминаем текущий элемент списка в качестве последнего опрошенного
                            m_LastSearchIndex = index;
                        }
                        var obj = model[index];
                        //Проверяем объект на соответствие нашему предикату
                        if ((match == null) || (match(obj)))
                        {
                            //Рисуем точки
                            PointsDrawer.PaintPoints(dc.Pen, obj, true);
                            //Если нарисованные точки пересекают рамку или находятся в ней
                            if (dc.Found)
                            {
                                //То запоминаем расстояние и текущий индекс элемента
                                var distance = Math.Sqrt(dc.DistanceSquared);
                                result = index;
                                //В качестве последнего опрошенного ставим следующий элемент
                                m_LastSearchIndex = index + 1;
                                //Добавляем расстояние и объект в список
                                list.Add(new KeyValuePair<double, object>(distance, obj));
                            }
                            //Выставляем флаг обратно
                            dc.Found = false;
                        }
                        //Если наш таймаут не 0 и время выполнения функции превышено - выходим из функции
                        if ((waitTimeOut != 0) && (Environment.TickCount - t > waitTimeOut))
                        {
                            break;
                        }
                    }
                }
                finally
                {
                    dc.EndRender();
                }
            }
            //Возвращаем список объектов
            return list;
        }

        //Метод возвращает список объектов внутри рамки выделения
        //Возможны два типа рамки:
        //  Topomatic.Cad.View.Hints.FrameSelectType.Contains - объект должен целиком находится внтури рамки
        //  Topomatic.Cad.View.Hints.FrameSelectType.Intersects - объект либо нахдится целиком внутри рамки, либо пересекает её
        public override void GetObjectsByFrame(FrameSelectType mode, RectangleD rect, Predicate<object> match, Action<object> action)
        {
            var model_layer = (ModelLayer)Layer;
            //Проверяем видимый ли слой и есть ли у него модель
            if (model_layer.ResolveVisible() && (model_layer.Model != null))
            {
                var model = model_layer.Model;
                //Проверяем тип рамки
                if (mode == Topomatic.Cad.View.Hints.FrameSelectType.Contains)
                {
                    //используем FullDeviceContext для проверки попадания объекта в рамку целиком
                    var dc = new FullDeviceContext(Layer.CadView);
                    dc.BeginRender();
                    try
                    {
                        //устанавливаем рамку
                        dc.SetClipRect(rect);
                        for (int i = 0; i < model.Count; i++)
                        {
                            var obj = model[i];
                            //Проверяем объект на соответствие нашему предикату
                            if ((match == null) || (match(model)))
                            {
                                //Устанавливаем флаг полностью нарисован
                                dc.FullDrawn = true;
                                //Рисуем объект
                                PointsDrawer.PaintPoints(dc.Pen, obj, true);
                                //Если объект полностью нарисован и нарисован полностью
                                if ((dc.FullDrawn) && (dc.Drawn))
                                {
                                    if (action != null)
                                    {
                                        //То выполняем метод для этого объекта
                                        action(obj);
                                    }
                                }
                                //Сбрасываем флаг отрисовки
                                dc.Drawn = false;
                            }
                        }
                    }
                    finally
                    {
                        dc.EndRender();
                    }
                }
                if (mode == Topomatic.Cad.View.Hints.FrameSelectType.Intersects)
                {
                    //используем NullDeviceContext для проверки попадания объекта в рамку или пересечения с ней
                    var dc = new NullDeviceContext(Layer.CadView, false);
                    dc.BeginRender();
                    try
                    {
                        //устанавливаем рамку
                        dc.SetClipRect(rect);
                        for (int i = 0; i < model.Count; i++)
                        {
                            var obj = model[i];
                            //Проверяем объект на соответствие нашему предикату
                            if ((match == null) || (match(model)))
                            {
                                //Рисуем объект
                                PointsDrawer.PaintPoints(dc.Pen, obj, true);
                                //Если нарисованные точки пересекают рамку или находятся в ней
                                if (dc.Found)
                                {
                                    if (action != null)
                                    {
                                        //То выполняем метод для этого объекта
                                        action(obj);
                                    }
                                }
                                //Сбрасываем флаг отрисовки
                                dc.Found = false;
                            }
                        }
                    }
                    finally
                    {
                        dc.EndRender();
                    }
                }
            }
        }

        //Метод зарезервирован для будущего использования и пока не поддерживается. Реализация не требуется.
        public override void GetObjectsByPolygon(FrameSelectType mode, List<Vector2D> pointsList, Predicate<object> match, Action<object> action)
        {
            throw new NotSupportedException();
        }

        //Возвращаем список всех элементов, которые можно выделить
        public override IEnumerable GetSelectable()
        {
            var model_layer = (ModelLayer)Layer;
            for (int i = 0; i < model_layer.Model.Count; i++)
            {
                yield return model_layer.Model[i];
            }
        }

        //Проверяем включен ли наш объект. В нашем случае всегда включен если включен слой
        public override bool IsEnable(object obj)
        {
            if (IsOwned(obj))
                return true;
            return Layer.ResolveEnable();
        }

        //Проверям принадлежит ли объект нашему SelectionSet
        public override bool IsOwned(object obj)
        {
            //Приводим объект к типу
            var points = obj as Points;
            if (points != null)
            {
                //И проверяем что модель равна модели нашего слоя
                return ((ModelLayer)Layer).Model == points.Owner;
            }
            return false;
        }

        //Проверяем, выделен ли объект
        public override bool IsSelected(object obj)
        {
            return m_Selected.Contains(obj);
        }

        //Выделяем или снимаем выделение с объекта
        public override void Select(object item, bool bFlag)
        {
            //Провеяем выделен ли наш объект
            var selected = IsSelected(item);
            if (bFlag)
            {
                //Если нужно выделить и он не выделен - выделяем
                if (!selected)
                    m_Selected.Add(item);
            }
            else
            {
                //Если нужно убрать выделение и он выделен - убираем выделение
                if (selected)
                    m_Selected.Remove(item);
            }
        }
    }
}

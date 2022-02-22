using System;
using System.Collections.Generic;
using System.ComponentModel;
using Topomatic.Cad.Foundation;
using Topomatic.FoundationClasses;
using Topomatic.FoundationClasses.Undo;
using Topomatic.Stg;

namespace tutorial10
{
    //Класс для хранения точек в нашей модели
    //поддерживаем интерфейсы IStgSerializable для сохранения и IOwned для определения владельца списка точек
    //Также поддерживаем интерфейс IObjectDisjoiner для привязок
    class Points : UpdatableObject, IStgSerializable, IOwned, IObjectDisjoiner
    {
        //Поддержка цепочки родителей
        private Model m_Owner;

        //Список структур с поддержкой истории изменений
        private BaseTransactableList<Vector2D> m_Points;

        public Points(Model owner)
        {
            m_Owner = owner;
            m_Points = new BaseTransactableList<Vector2D>(this);
        }

        //Метод для добавления точки в список
        public void AddPoint(Vector2D point)
        {
            m_Points.Add(point);
        }

        //Метод для удаления точки из списка
        public void RemovePoint(int index)
        {
            m_Points.RemoveAt(index);
        }

        //Метод для получения точки из списка
        public Vector2D this[int index]
        {
            get
            {
                return m_Points[index];
            }
            set
            {
                m_Points[index] = value;
            }
        }

        //Количество точек в списке
        //Свойство декорировано атрибутом DisplayName для отображения в инспекторе объектов
        [DisplayName("Количество точек")]
        public int Count
        {
            get
            {
                return m_Points.Count;
            }
        }

        //Владелец нашего списка точек, у нас это наша модель
        //Свойство декорировано атрибутом Browsable чтобы исключить отображение в инспекторе объектов
        [Browsable(false)]
        public object Owner
        {
            get
            {
                return m_Owner;
            }
            set
            {
                //Владелец назначается один раз на конструкторе объекта, назначение отдельно недопустимо
                throw new NotSupportedException();
            }
        }

        //Реализация загрузки
        public void LoadFromStg(StgNode node)
        {
            //обратите внимание, загрузку мы производим во внутренний список
            //это позволяет избежать записи значений в историю изменений
            m_Points.InnerList.Clear();
            //При загрузке массива указывается тип составляющих массив значений
            var array = node.GetArray("Values", StgType.Node);
            for (int i = 0; i < array.Count; i++)
            {
                m_Points.InnerList.Add(Vector2D.LoadFromStg(array.GetNode(i)));
            }
        }

        //Реализация сохранения
        public void SaveToStg(StgNode node)
        {
            //Сохраняем значения в узел
            //Сохраняем массив с указанием типа значений
            var array = node.AddArray("Values", StgType.Node);
            for (int i = 0; i < m_Points.Count; i++)
            {
                m_Points[i].SaveToStg(array.AddNode());
            }
        }

        //Дополнительно перекрываем метод ToString() для отображения типа объекта в инстпекторе объектов
        public override string ToString()
        {
            return "Точки модели";
        }

        //Привязка  КОНЕЧНАЯ ТОЧКА
        public void GetEndPoint(ObjectsDisjointerArgs e, IList<Vector3D> list)
        {
            for (int i = 0; i < m_Points.Count; i++)
            {
                list.Add(m_Points[i]);
            }
        }

        //Привязка  ЦЕНТРАЛЬНАЯ ТОЧКА
        public void GetCenterPoint(ObjectsDisjointerArgs e, IList<Vector3D> list)
        {
            //Do nothing
        }

        //Привязка  СЕРЕДИНА ОТРЕЗКА
        public void GetMiddlePoint(ObjectsDisjointerArgs e, IList<Vector3D> list)
        {
            for (int i = 1; i < m_Points.Count; i++)
            {
                list.Add((m_Points[i - 1] + m_Points[i]) * 0.5);
            }
        }

        //Привязка  УЗЕЛ
        public void GetNodePoint(ObjectsDisjointerArgs e, IList<Vector3D> list)
        {
            //Do nothing
        }

        //Привязка  КВАДРАНТ
        public void GetQuadrantPoint(ObjectsDisjointerArgs e, IList<Vector3D> list)
        {
            //Do nothing
        }

        //Привязка  ТОЧКА ВСТАВКИ
        public void GetInsertionPoint(ObjectsDisjointerArgs e, IList<Vector3D> list)
        {
            //Do nothing
        }

        //Привязка  БЛИЖАЙШАЯ ТОЧКА, КАСАТЕЛЬНАЯ и т.п.
        public void GetSegments(ObjectsDisjointerArgs e, IList<ArcSegment> arcList, IList<LineSegment> lineList)
        {
            for (int i = 1; i < m_Points.Count; i++)
            {
                lineList.Add( new LineSegment() { StartPoint = m_Points[i - 1], EndPoint = m_Points[i] } );
            }
        }
    }
}

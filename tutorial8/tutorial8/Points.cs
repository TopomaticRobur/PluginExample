using System;
using System.Collections.Generic;
using System.ComponentModel;
using Topomatic.Cad.Foundation;
using Topomatic.FoundationClasses;
using Topomatic.Stg;

namespace tutorial8
{
    //Класс для хранения точек в нашей модели
    //поддерживаем интерфесы IStgSerializable для сохранения и IOwned для определения владельца списка точек
    class Points : IStgSerializable, IOwned
    {
        private Model m_Owner;

        private List<Vector2D> m_Points = new List<Vector2D>();

        public Points(Model owner)
        {
            m_Owner = owner;
        }

        //Метод для добавления точки в список
        public void AddPoint(Vector2D point)
        {
            m_Points.Add(point);
        }

        //Метод для получения точки из списка
        public Vector2D this[int index]
        {
            get
            {
                return m_Points[index];
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
            m_Points.Clear();
            //При загрузке массива указывается тип составляющих массив значений
            var array = node.GetArray("Values", StgType.Node);
            for (int i = 0; i < array.Count; i++)
            {
                m_Points.Add(Vector2D.LoadFromStg(array.GetNode(i)));
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
    }
}

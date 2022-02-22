using Topomatic.FoundationClasses;
using Topomatic.FoundationClasses.Undo;
using Topomatic.Stg;

namespace tutorial10
{
    //Класс реализующий структуру данных нашей модели
    //для поддержки интерфейса IStateController наследуем от StateControllerObject
    class Model : StateControllerObject, IStgSerializable
    {
        private bool m_ReadOnly = false;

        //Список объектов точек с поддержкой истории изменений
        private BaseTransactableList<Points> m_Points;

        public Model()
        {
            m_Points = new BaseTransactableList<Points>(this);
        }

        //Метод для добавления списка точек в модель
        public Points Add()
        {
            var p = new Points(this);
            m_Points.Add(p);
            return p;
        }

        //Метод для получения списка точек из модели
        public Points this[int index]
        {
            get
            {
                return m_Points[index];
            }
        }

        //Метод для удаления списка точек из модели
        public bool Remove(Points points)
        {
            return m_Points.Remove(points);
        }

        //Количество списков в модели
        public int Count
        {
            get
            {
                return m_Points.Count;
            }
        }

        //флаг только для чтения
        public override bool ReadOnly
        {
            get
            {
                return m_ReadOnly;
            }

            set
            {
                m_ReadOnly = value;
            }
        }

        //Загрузка из узла
        public void LoadFromStg(StgNode node)
        {
            m_Points.InnerList.Clear();
            //При загрузке массива указывается тип составляющих массив значений
            var array = node.GetArray("Points", StgType.Node);
            for (int i = 0; i < array.Count; i++)
            {
                var p = new Points(this);
                p.LoadFromStg(array.GetNode(i));
                m_Points.InnerList.Add(p);
            }
        }

        //Сохранение в узел
        public void SaveToStg(StgNode node)
        {
            //Сохраняем значения в узел
            //Сохраняем массив с указанием типа значений
            var array = node.AddArray("Points", StgType.Node);
            for (int i = 0; i < m_Points.Count; i++)
            {
                m_Points[i].SaveToStg(array.AddNode());
            }
        }
    }
}

using System.Collections.Generic;
using Topomatic.Cad.Foundation;
using Topomatic.FoundationClasses;
using Topomatic.Stg;

namespace tutorial7
{
    //Класс реализующий структуру данных нашей модели
    //для поддержки интерфейса IStateController наследуем от StateControllerObject
    class Model : StateControllerObject, IStgSerializable
    {
        private bool m_ReadOnly = false;

        public List<Vector2D> Points = new List<Vector2D>();

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
            Points.Clear();
            //При загрузке массива указывается тип составляющих массив значений
            var array = node.GetArray("Points", StgType.Node);
            for (int i = 0; i < array.Count; i++)
            {
                Points.Add(Vector2D.LoadFromStg(array.GetNode(i)));
            }
        }

        //Сохранение в узел
        public void SaveToStg(StgNode node)
        {
            //Сохраняем значения в узел
            //Сохраняем массив с указанием типа значений
            var array = node.AddArray("Points", StgType.Node);
            for (int i = 0; i < Points.Count; i++)
            {
                Points[i].SaveToStg(array.AddNode());
            }
        }
    }
}

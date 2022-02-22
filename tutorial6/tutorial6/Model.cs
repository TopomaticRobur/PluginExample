using System.Collections.Generic;
using Topomatic.FoundationClasses;
using Topomatic.Stg;

namespace tutorial6
{
    //Класс реализующий структуру данных нашей модели
    //для поддержки интерфейса IStateController наследуем от StateControllerObject
    class Model : StateControllerObject, IStgSerializable
    {
        private bool m_ReadOnly = false;

        //Строковое значение
        public string StringValue = "Строка";

        //Булево значение
        public bool BooleanValue = false;

        //Значение с плавающей точкой
        public double DoubleValue = 10.5;

        //Целое значение
        public int IntValue = 10;

        //Список строковых значений
        public List<string> ArrayValues = new List<string>();

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
            //Все значения загружаем с указанием значения по умолчанию
            BooleanValue = node.GetBoolean("BooleanValue", false);
            StringValue = node.GetString("StringValue", "Строка");
            DoubleValue = node.GetDouble("DoubleValue", 10.5);
            IntValue = node.GetInt32("IntValue", 10);
            ArrayValues.Clear();
            //При загрузке массива указывается тип составляющих массив значений
            var array = node.GetArray("ArrayValues", StgType.String);
            for (int i = 0; i < array.Count; i++)
            {
                ArrayValues.Add(array.GetString(i));
            }
        }

        //Сохранение в узел
        public void SaveToStg(StgNode node)
        {
            //Сохраняем значения в узел
            node.AddBoolean("BooleanValue", BooleanValue);
            node.AddString("StringValue", StringValue);
            node.AddDouble("DoubleValue", DoubleValue);
            node.AddInt32("IntValue", IntValue);
            //Сохраняем массив с указанием типа значений
            var array = node.AddArray("ArrayValues", StgType.String);
            for (int i = 0; i < ArrayValues.Count; i++)
            {
                array.AddString(ArrayValues[i]);
            }
        }
    }
}

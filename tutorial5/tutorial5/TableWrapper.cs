using System.Collections.Generic;
using System.ComponentModel;
using Topomatic.Alg.Runtime.Wrappers;
using Topomatic.Cad.Foundation.Design;
using Topomatic.ComponentModel;

namespace tutorial5
{
    //Перечисление из нескольких значений
    enum DataEnum
    {
        Value1,
        Value2,
        Value3
    }

    //Структура данных, которая будет хранится в нашей модели
    struct DataValue
    {
        public double Length;

        public string TextValue;

        public DataEnum Value;
    }

    //Конвертер перечисления DataEnum в строковое представление и обратно
    class DataEnumConverter : BaseEnumConverter
    {
        public DataEnumConverter()
        {
            Dictionary[DataEnum.Value1] = "Значение1";
            Dictionary[DataEnum.Value2] = "Значение2";
            Dictionary[DataEnum.Value3] = "Значение3";
        }
    }

    //Класс реализующий обработку содержимого таблицы
    //Для простоты реализации, наследуем его от SimpleChangeTrackingWrapper
    class TableWrapper : SimpleChangeTrackingWrapper, IActivator
    {
        //класс реализующий представление строки таблицы
        public class TableRow
        {
            //Указатель на нашу таблицу, необходим, чтобы при изменении свойств выставить св-во IsChanged 
            private SimpleChangeTrackingWrapper m_Wrapper;

            private double m_Length;

            private string m_TextValue;

            private DataEnum m_Value;

            public TableRow(SimpleChangeTrackingWrapper wrapper, DataValue value)
            {
                m_Wrapper = wrapper;
                m_Length = value.Length;
                m_TextValue = value.TextValue;
                m_Value = value.Value;
            }

            //Свойство длина
            //Обратите внимание, что оно декорировано атрибутом Length - который позволяет выводить содержимое с точностью длин
            [DisplayName("Длина, м"), Category("Верхний заголовок|Нижний заголовок"), Length]
            public double Length
            {
                get
                {
                    return m_Length;
                }
                set
                {
                    m_Length = value;
                    //Предупреждаем таблицу, о наличии изменений
                    m_Wrapper.IsChanged = true;
                }
            }

            //Стоковое свойство
            [DisplayName("Строка"), Category("Верхний заголовок|Нижний заголовок")]
            public string TextValue
            {
                get
                {
                    return m_TextValue;
                }
                set
                {
                    m_TextValue = value;
                    //Предупреждаем таблицу, о наличии изменений
                    m_Wrapper.IsChanged = true;
                }
            }

            //Свойство перечисление
            [DisplayName("Перечисление"), Category("Верхний заголовок|Другой заголовок")]
            //Наш конвертер назначен с помощью атрибута PropertyTypeonverterAttribute
            [PropertyTypeConverter(typeof(DataEnumConverter))]
            public DataEnum Value
            {
                get
                {
                    return m_Value;
                }
                set
                {
                    m_Value = value;
                    //Предупреждаем таблицу, о наличии изменений
                    m_Wrapper.IsChanged = true;
                }
            }
        }

        //Сохраняем ссылку на данные модели
        private List<DataValue> m_Data;

        //При создании таблицы, заполняем её содержимое данными из модели
        public TableWrapper(List<DataValue> data)
        {
            m_Data = data;
            this.Items = new List<object>(m_Data.Count);
            for (int i = 0; i < m_Data.Count; i++)
            {
                this.Items.Add(new TableRow(this, m_Data[i]));
            }
        }

        //Считаем что наша таблица всегда доступна для редактирования
        public override bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        //При вызове метода заполняем модель по даннм нашей таблицы
        public override void AcceptChanges()
        {
            m_Data.Clear();
            for (int i = 0; i < this.Items.Count; i++)
            {
                var row = (TableRow)this.Items[i];
                m_Data.Add(new DataValue() { Length = row.Length, TextValue = row.TextValue, Value = row.Value });
            }
        }

        //Реализация интерфеса IActivator

        //Всегда можем создавать экземпляр строки таблицы
        public bool CanCreateInstance
        {
            get
            {
                return true;
            }
        }

        //Создаем новый экземпляр строки таблицы
        public object CreateInstance()
        {
            return new TableRow(this, new DataValue());
        }
    }
}

using System.IO;
using System.Windows.Forms;
using Topomatic.ApplicationPlatform.Core;
using Topomatic.Stg;

namespace tutorial6
{
    //Класс реализующий редактор нашей модели
    class Editor : ModelEditor
    {
        //Реализация загрузки модели по указанному пути, должна вернуть реализацию класса нашей модели
        public override object LoadFromFile(string fullpath)
        {
            //создаем экземпляр класса модели
            var model = new Model();
            //если fullpath null - то необходимо просто вернуть экземпляр класса модели, без загрузки данных
            if (fullpath != null)
            {
                //создаем файловый поток
                using (var stream = new FileStream(fullpath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    //создаем документ для работы с Topomatic.Stg
                    var document = new StgDocument();
                    //загружаем документ из потока в бинарном виде
                    document.LoadFromStreamAsBinary(stream);
                    //загружаем данные нашей модели из документа
                    model.LoadFromStg(document.Body);
                }
            }
            //всегда возвращаем экземпляр модели
            return model;
        }

        //Реализация сохранения модели по указанному пути
        public override void SaveToFile(object model, string fullpath)
        {
            //в качестве параметра model приходит наша модель данных
            var m = model as Model;
            if (m != null)
            {
                //создаем файловый поток
                using (var stream = new FileStream(fullpath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    //создаем документ для работы с Topomatic.Stg
                    var document = new StgDocument();
                    //сохраняем данные нашей модели в документ
                    m.SaveToStg(document.Body);
                    //сохраняем документ в потока в бинарном виде
                    document.SaveToStreamAsBinary(stream);
                }
            }
        }

        //Реализация открытия модели по команде "open"
        public override IEditorResult Open(IProjectModel model)
        {
            var cursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                //В нашем поросто возвращаем реализацию интерфеса IEditorResult
                return new EditorResult();
            }
            finally
            {
                Cursor.Current = cursor;
            }
        }

        //Реализация интерфейса IEditorResult
        private class EditorResult : IEditorResult
        {
            private bool m_Opened;

            public EditorResult()
            {
                m_Opened = true;
            }

            //Необходимо реализовать флаг, показывающий открыта модель или нет
            public bool Opened
            {
                get
                {
                    return m_Opened;
                }
            }

            //Необходимо реализовать метод закрытия модели
            public void Close()
            {
                //В нашем случае мы просто управляем флагом и все
                m_Opened = false;
            }

            //И метод перезагрузки модели
            public void Reload()
            {
                //Здесь нам ничего не нужно делать
            }
        }
    }
}

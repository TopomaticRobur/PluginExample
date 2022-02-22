using System.IO;
using Topomatic.ApplicationPlatform.Core;
using Topomatic.Cad.View;
using Topomatic.Stg;

namespace tutorial7
{
    //Класс реализующий редактор нашей модели
    class Editor : PlanModelEditor
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

        protected override CadViewLayer CreatePlanLayer(IProjectModel model)
        {
            var m = model.LockRead() as Model;
            if (m != null)
            {
                return new ModelLayer() { Model = m };
            }
            return null;
        }

        protected override void ReloadModel(IProjectModel model, EditorResult editorResult)
        {
            var m = model.LockRead() as Model;
            if (m != null)
            {
                var layer = (ModelLayer)editorResult.PlanLayer;
                layer.Model = m;
            }
        }

        protected override void RemovePlanLayer(IProjectModel model, CadViewLayer layer)
        {
            var model_layer = layer as ModelLayer;
            if (model_layer != null)
            {
                model_layer.Model = null;
            }
        }
    }
}

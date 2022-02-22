using System;
using System.Diagnostics;
using Topomatic.ApplicationPlatform;
using Topomatic.ApplicationPlatform.Core;
using Topomatic.ApplicationPlatform.Plugins;

namespace tutorial6
{
    partial class Module : Topomatic.ApplicationPlatform.Plugins.PluginInitializator
    {
        //функция создает экземпляр редактора модели и возвращает его
        [cmd("create_testmodel_editor")]
        public Editor CreateEditor()
        {
            return new Editor();
        }

        //функция создает новую модель и возвращает её
        [cmd("create_testmodel")]
        private object CreateModel(object[] args)
        {
            //получаем текущий активный проект
            var project = ApplicationHost.Current.ActiveProject as ModelProject;
            Debug.Assert(project != null);
            if (project != null)
            {
                //запускаем групповое изменение свойств
                project.TransactionManager.BeginUpdate();
                try
                {
                    string folder;
                    if ((args != null) && (args.Length > 0) && (args[0] != null))
                    {
                        //В качестве аргумента приходит либо идентификатор каталога внутри проекта
                        folder = args[0].ToString();
                    }
                    else
                    {
                        //либо мы создаем  этот идентификатор самостоятельно
                        folder = PluginCoreOps.FindModelPathId(PluginCoreOps.CreateFolder(new string[] { "Модели", "Тестовые модели" }));
                    }
                    //создаем модель с помощью команды "mkitem"
                    //в неё мы передаем идентификатор каталога и тип нашей модели
                    return ApplicationHost.Current.Plugins.Execute("mkitem", new object[] { folder, "testmodel" });
                }
                finally
                {
                    project.TransactionManager.EndUpdate();
                }
            }
            else
            {
                throw new OperationCanceledException();
            }
        }

        //функция позволяет редактировать содержимое нашей модели
        [cmd("edit_testmodel")]
        private void OpenModel(string pathid)
        {
            //получаем IProjectModel используя идентификатор модели
            var project_model = PluginCoreOps.FindModel(pathid);
            if (project_model != null)
            {
                //вызываем блокировку модели на редактирование
                project_model.LockWrite();
                try
                {
                    //получаем класс нашей модели
                    var model = project_model.LockRead() as Model;
                    if (model != null)
                    {
                        //вызываем диалог редактирования нашей модели
                        if (EditModelDlg.Execute(model))
                            project_model.Modified = true;
                    }
                }
                finally
                {
                    //снимаем блокировку модели на редактирование
                    project_model.UnlockWrite();
                }
            }
        }

        public override void Initialize(PluginFactory factory)
        {
            base.Initialize(factory);
            //Регестрируем нашу модель в проекте
            factory.RegisterModelEditor("testmodel", 
                new ModelEditorInfo("Тестовая модель|*.testmodelx", 
                ".testmodelx", 
                "testmodel", 
                "Тестовая модель", 
                "create_testmodel_editor"));
        }
    }
}

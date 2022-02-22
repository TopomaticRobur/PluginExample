using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using Topomatic.ApplicationPlatform;
using Topomatic.ApplicationPlatform.Core;
using Topomatic.ApplicationPlatform.Plugins;
using Topomatic.Cad.Foundation;
using Topomatic.Cad.View;
using Topomatic.Cad.View.Hints;

namespace tutorial9
{
    partial class Module : Topomatic.ApplicationPlatform.Plugins.PluginInitializator
    {
        //функция создает экземпляр редактора модели и возвращает его
        [cmd("create_pointsmodel_editor")]
        public Editor CreateEditor()
        {
            return new Editor();
        }

        //функция создает новую модель и возвращает её
        [cmd("create_pointsmodel")]
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
                    return ApplicationHost.Current.Plugins.Execute("mkitem", new object[] { folder, "pointsmodel" });
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
        [cmd("edit_pointsmodel")]
        public void EditModel()
        {
            var cadView = CadView;
            if (cadView != null)
            {
                var model_layer = ModelLayer.GetModelLayer(cadView);
                if (model_layer != null)
                {
                    //список выбранных точек
                    var positions = new List<Vector2D>();
                    //делегат для динамической отрисовки
                    DrawCursorEvent dynamic_draw = delegate (CadPen pen, Vector3D vertex)
                    {
                        //если есть точки в списке, нужно их нарисовать и нарисовать линию
                        //от последней выбранной точки, до текущего положения курсора
                        if (positions.Count > 0)
                        {
                            pen.Color = Color.Lime;
                            pen.BeginDraw();
                            try
                            {
                                for (int i = 1; i < positions.Count; i++)
                                {
                                    pen.DrawLine(positions[i - 1], positions[i]);
                                }
                                pen.DrawLine(positions[positions.Count - 1], vertex.Pos);
                            }
                            finally
                            {
                                pen.EndDraw();
                            }
                        }
                    };
                    //подписываемся на событие отрисовки
                    cadView.DynamicDraw += dynamic_draw;
                    try
                    {
                        Vector3D pos;
                        //просим пользователя указать несколько точек
                        while (CadCursors.GetPoint(cadView, out pos, "Укажите точку"))
                        {
                            positions.Add(pos.Pos);
                        }
                        //если точки заданы, то изменяем нашу модель
                        if (positions.Count > 0)
                        {
                            //получаем её со слоя
                            var model = model_layer.Model;
                            //добавляем новые точки
                            var points = model.Add();
                            for (int i = 0; i < positions.Count; i++)
                            {
                                points.AddPoint(positions[i]);
                            }
                            //выставляем флаг модификации вручную
                            var p = PluginCoreOps.FindModel(model);
                            if (p != null)
                                p.Modified = true;
                            //обновляем видовой экран
                            cadView.Unlock();
                            cadView.Invalidate();
                        }
                    }
                    finally
                    {
                        //отписываемся от события отрисовки
                        cadView.DynamicDraw -= dynamic_draw;
                    }
                }
            }
        }

        public override void Initialize(PluginFactory factory)
        {
            base.Initialize(factory);
            //Регестрируем нашу модель в проекте
            factory.RegisterModelEditor("pointsmodel",
                new ModelEditorInfo("Тестовая модель с точками|*.pointsmodelx",
                ".pointsmodelx",
                "pointsmodel",
                "Тестовая модель с точками",
                "create_pointsmodel_editor"));
        }
    }
}

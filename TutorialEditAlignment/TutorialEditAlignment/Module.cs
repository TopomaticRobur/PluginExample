using System;
using System.Windows.Forms;
using Topomatic.Alg;
using Topomatic.Alg.Plan;
using Topomatic.Alg.Prf;
using Topomatic.Alg.Runtime.ServiceClasses;
using Topomatic.ApplicationPlatform;
using Topomatic.ApplicationPlatform.Plugins;
using Topomatic.Cad.Foundation;
using Topomatic.Cad.View;
using Topomatic.Cad.View.Hints;
using Topomatic.Controls.Dialogs;
using Topomatic.FoundationClasses;

namespace TutorialEditAlignment
{
    partial class Module : PluginInitializator
    {
        //метод принимает на вход комманду и окно, и возвращает True если окно устраивает
        protected override bool ValidateWindow(string cmd, IDocumentWindow window)
        {
            switch (cmd)
            {
                case "define_stationing_at_point":
                case "define_point_at_stationing":
                case "add_vertex_to_alignment":
                    //выбираем окно плана             
                    return window.UID.Equals(Consts.PlanWindow, StringComparison.InvariantCultureIgnoreCase);
                case "smooth_peak":
                    //выбираем окно профиля
                    return window.UID.Equals(Consts.ProfileWindow, StringComparison.InvariantCultureIgnoreCase);
                default:
                    //по умолчанию мы выбираем текущее окно
                    return base.ValidateWindow(cmd, window);
            }
        }

        [cmd("define_stationing_at_point")]
        private void DefineStationingAtPoint()
        {
            var cadview = CadView;
            if (cadview == null) return;

            //Выбираем трассу
            object select;
            var res = cadview.SelectionSet.SelectOneObjectAtScreen(
                o => o is IWrapped && ((IWrapped)o).WrappedObject is Alignment,
                out select, "Выберите подобъект:");
            cadview.SelectionSet.Clear();
            if (res != GetPointResult.Accept) return;

            var alignment = ((IWrapped)select).WrappedObject as Alignment;
            var compound = alignment.Plan.CompoundLine;
            var stationing = alignment.Stationing;

            //Указываем точку, в которой требуется определить пикетаж
            Vector3D point;
            var getPointRes = CadCursors.GetPoint(cadview, out point, "Укажите точку определения пикетажа:");
            if (!getPointRes) return;

            //Определяем расстояние от начала пути и величину смещения до указанной точки
            double station, offset;
            var convertRes = compound.PosToStaOffset(point.Pos, out station, out offset);
            if (!convertRes)
            {
                MessageDlg.Show("Не удалось определить расстояние от начала пути в указанной точке.",
                    MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                return;
            }

            //Определяем пикетаж на полученном расстоянии от начала пути и выводим данные на экран
            var pkPlus = stationing.StationToString(station);
            var msg = string.Join(Environment.NewLine,
                $"ПК{pkPlus}",
                $"Расстояние: {ValueConverter.FloatToStr(station, 3)}",
                $"Смещение: {ValueConverter.FloatToStr(offset, 3)}");
            MessageDlg.Show(msg, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        }

        [cmd("define_point_at_stationing")]
        private void DefinePointAtStationing()
        {
            var cadview = CadView;
            if (cadview == null) return;

            //Выбираем трассу
            object select;
            var res = cadview.SelectionSet.SelectOneObjectAtScreen(
                o => o is IWrapped && ((IWrapped)o).WrappedObject is Alignment,
                out select, "Выберите подобъект:");
            cadview.SelectionSet.Clear();
            if (res != GetPointResult.Accept) return;

            var alignment = ((IWrapped)select).WrappedObject as Alignment;
            var compound = alignment.Plan.CompoundLine;
            var stationing = alignment.Stationing;

            //Вводим пикетаж и определяем расстояние от начала пути
            var pkPlus = "0+00.00";
            res = CadCursors.GetString(cadview, ref pkPlus, "Введите пикетаж в формате ПК+:");
            if (res == GetPointResult.Cancel) return;

            double station;
            if (!stationing.TryStringToStation(pkPlus, out station))
            {
                MessageDlg.Show("Указанный пикетаж не найден.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                return;
            }

            //Вводим смещение
            var offset = 0.0;
            res = CadCursors.GetDouble(cadview, ref offset, "Введите смещение:");
            if (res == GetPointResult.Cancel) return;

            //Определяем координаты по задданым значениям и выводим значения на экран
            Vector2D point;
            var vectorFound = compound.StaOffsetToPos(station, offset, out point);
            if (!vectorFound)
            {
                MessageDlg.Show("Не удалось определить координаты.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                return;
            }

            var msg = string.Join(Environment.NewLine,
                $"X: {ValueConverter.FloatToStr(point.X, 3)}",
                $"Y: {ValueConverter.FloatToStr(point.Y, 3)}",
                $"Расстояние: {ValueConverter.FloatToStr(station, 3)}");
            MessageDlg.Show(msg, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        }

        [cmd("add_vertex_to_alignment")]
        private void AddVertexToAlignment()
        {
            //Получаем модель текущей трассы. Если текущей является не трасса, то активируется последняя активная трасса
            Alignment alignment;
            using (var receiver = ActiveAlignmentReciver<Alignment>.CreateReciver(false))
            {
                alignment = receiver.Alignment;
                if (alignment == null)
                {
                    MessageDlg.Show("Последний активный подобъект не найден.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    return;
                }

                var cadview = CadView;
                if (cadview == null) return;

                var plan = alignment.Plan;
                var compound = plan.CompoundLine;

                //Указываем однорадиусную кривую
                Vector3D point;
                var getPointRes = CadCursors.GetPoint(cadview, out point, "Укажите однорадиусную кривую:");
                if (!getPointRes) return;

                double station, offset;
                var res = compound.PosToStaOffset(point.Pos, out station, out offset);
                if (!res)
                {
                    MessageDlg.Show("Указанная точка лежит вне плана линии.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    return;
                }

                //Получаем ближайшую вершину угла к указанной точке и добавляем в неё элемент
                var vertex = plan.SearchNearest(station);

                if (vertex.Count != 1)
                {
                    MessageDlg.Show("Необходимо указать однорадиусную кривую.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    return;
                }

                vertex.BeginTransaction();
                var result = true;
                try
                {
                    var angle = Math.Abs(vertex.Beta);
                    var item1 = vertex[0];
                    var item2 = new PlanLine.Vertex.VertexItem { R = item1.R, L2 = item1.L2, K = angle * 0.5 };
                    item1.L2 = 0.0;
                    item1.K = angle * 0.5;
                    vertex[0] = item1;
                    vertex.Add(item2);
                    result = PlanLineSolver.PlanVertexesValid(plan);
                }
                finally
                {
                    if (result)
                    {
                        vertex.Commit();
                    }
                    else
                    {
                        vertex.Rollback();
                        MessageDlg.Show("При разбивке кривой произошла ошибка.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    }
                }
            }
        }
        
        [cmd("smooth_peak")]
        private void SmoothPeak()
        {
            //Получаем модель текущей трассы. Если текущей является не трасса, то активируется последняя активная трасса
            Alignment alignment;
            using (var receiver = ActiveAlignmentReciver<Alignment>.CreateReciver(false))
            {
                alignment = receiver.Alignment;
                if (alignment == null)
                {
                    MessageDlg.Show("Необходимо сделать трассу активной.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                    return;
                }

                var cadview = CadView;
                if (cadview == null) return;

                //Выбираем рамкой вершину красного профиля, которую требуется сгладить
                RectangleD rectangle;
                FrameSelectType ft;
                if (CadCursors.GetFrame(cadview, out rectangle, out ft, "Выберите вершину:") != GetPointResult.Accept)
                    return;
                var bounds = rectangle.ToBoundingBox();
                bounds = cadview.UnProjectBox(bounds);

                //Получаем красный профиль и находим указанную вершину на этом профиле
                var profile = alignment.Transitions[0].RedProfile;
                for (int i = 1; i < profile.Count - 1; i++)
                {
                    var curNode = profile[i];

                    //Если это выбранная вершина, то предлагаем пользователю ввести длину сглаживающего сегмента,
                    //высчитываем положение новых вершин, добавляем их в профиль и удаляем исходную вершину
                    if (bounds.Contains(curNode.Position) != ContainmentType.Disjoint)
                    {
                        var length = 0.0;
                        var res = CadCursors.GetDouble(cadview, ref length, "Укажите длину сглаживающего сегмента:");
                        if (res != GetPointResult.Accept) return;

                        var prevNode = profile[i - 1];
                        var nextNode = profile[i + 1];

                        var prevLength = curNode.Station - prevNode.Station;
                        var curLength = nextNode.Station - curNode.Station;

                        if (prevLength < length * 0.5 || curLength < length * 0.5)
                        {
                            MessageDlg.Show("Необходимо указать меньшее значение длины сегмента сглаживания.", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                            return;
                        }

                        var prevIncline = (curNode.Elevation - prevNode.Elevation) / prevLength;
                        var curIncline = (nextNode.Elevation - curNode.Elevation) / curLength;

                        var startStation = prevNode.Station + (prevLength - length * 0.5);
                        var startElevation = prevNode.Elevation + prevIncline * (prevLength - length * 0.5);
                        var startNode = new ProjectNode(startStation, startElevation, 0, 0, ProjectNodeFlags.UseRadius);

                        var endStation = curNode.Station + length * 0.5;
                        var endElevation = curNode.Elevation + curIncline * length * 0.5;
                        var endNode = new ProjectNode(endStation, endElevation, 0, 0, ProjectNodeFlags.UseRadius);

                        profile.BeginUpdate();
                        try
                        {
                            profile.Add(startNode);
                            profile.Add(endNode);
                            profile.Remove(curNode);
                        }
                        finally
                        {
                            profile.EndUpdate();
                        }

                        return;
                    }
                }
            }
        }
    }
}
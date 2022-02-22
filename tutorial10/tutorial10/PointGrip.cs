using Topomatic.Cad.Foundation;
using Topomatic.Cad.View;

namespace tutorial10
{
    //Реализация грипа для перемещения вершины
    class PointGrip : Grip
    {
        private Points m_Points;

        private int m_Index;

        //Конструктор
        public PointGrip(CadView cadview, Points points, int index) 
            : base(cadview)
        {
            m_Index = index;
            m_Points = points;
            this.Location = m_Points[index];
            //Дополнительный грип для удаления вершины
            var g = new ClickGrip();
            g.Click += delegate
            {
                //Удаляем вершину
                m_Points.RemovePoint(m_Index);
                //Обновляем видовой экран
                CadView.Unlock();
                CadView.Invalidate();
            };
            //Добавляем дополнительный грип
            this.AddGrip("Удалить", "remove_point", g);
        }

        //Реализация применения изменений
        public override void OnMove(Vector3D vertex)
        {
            base.OnMove(vertex);
            //Назначаем нашей точке новое положение
            m_Points[m_Index] = vertex.Pos;
        }

        
        //Реализация динамической отрисовки
        public override void OnDynamicRender(DeviceContext dc, Vector3D position)
        {
            base.OnDynamicRender(dc, position);
            //Если наша точка не первая, то рисуем линию до предыдущей точки
            if (m_Index > 0)
            {
                AuxiliaryDrawer.DrawAuxiliaryLine3d(dc, m_Points[m_Index - 1], position);
            }
            //Если наша точка не последняя, то рисуем линию до следующей точки
            if (m_Index < m_Points.Count - 1)
            {
                AuxiliaryDrawer.DrawAuxiliaryLine3d(dc, m_Points[m_Index + 1], position);
            }
        }

    }
}

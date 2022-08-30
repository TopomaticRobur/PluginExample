using System;
using Topomatic.Alg;
using Topomatic.Alg.Prf;
using Topomatic.Cad.Foundation;
using Topomatic.Dwg;
using Topomatic.Dwg.Entities;
using Topomatic.Plt.Templates.Prf;

namespace TutorialFields.Profile
{
    class TutorialProfileField : PrfField
    {
        private int _precision;
        /// <summary>
        /// Количество знаков после запятой
        /// </summary>
        [SRDisplayName("sTutorialFieldPrecisionProperty")]
        public int Precision
        {
            get => _precision;
            set => _precision = value > 0 ? value : 0;
        }
        
        public TutorialProfileField() : base()
        {
            Precision = 2;
        }

        protected override void OnDrawField()
        {
            base.OnDrawField();

            // Получаем активную ось профиля и её линии земли и проектного профиля
            var transition = this.DataManager["ActiveTransition"] as Transition;
            if (transition == null) return;

            var drawing = Drawing;
            var egProfile = transition.EgProfile;
            var redProfile = transition.RedProfile;

            // Рассчитываем и отрисовываем минимальные и максимальные отметки линий земли и проектного профиля
            if (egProfile.Count > 0)
            {
                var minEgElev = double.MaxValue;
                var minEgElevSta = 0.0;
                var maxEgElev = double.MinValue;
                var maxEgElevSta = 0.0;
                for (var i = 0; i < egProfile.Count; i++)
                {
                    var node = egProfile[i];

                    // Проверяем, попадает ли узел в текущий разрыв
                    if (!AlignmentValueConverter.StationInLimits(node.Station, StartSta, EndSta, true, true))
                        continue;

                    if (node.Elevation < minEgElev)
                    {
                        minEgElev = node.Elevation;
                        minEgElevSta = node.Station;
                    }
                    if (node.Elevation > maxEgElev)
                    {
                        maxEgElev = node.Elevation;
                        maxEgElevSta = node.Station;
                    }
                }

                var minEgEnt = DrawElevation(drawing, minEgElev, minEgElevSta, CadColor.Green);

                if (ValueConverter.CompValues(minEgElev, maxEgElev) != 0)
                {
                    var maxEgEnt = DrawElevation(drawing, maxEgElev, maxEgElevSta, CadColor.Green);
                    minEgEnt.Content = $"E(L): {minEgEnt.Content}";
                    maxEgEnt.Content = $"E(H): {maxEgEnt.Content}";
                }
            }

            if (redProfile.Count > 0)
            {
                var minRedElev = double.MaxValue;
                var minRedElevSta = 0.0;
                var maxRedElev = double.MinValue;
                var maxRedElevSta = 0.0;
                foreach (var node in redProfile)
                {
                    if (node.Elevation < minRedElev)
                    {
                        minRedElev = node.Elevation;
                        minRedElevSta = node.Station;
                    }
                    if (node.Elevation > maxRedElev)
                    {
                        maxRedElev = node.Elevation;
                        maxRedElevSta = node.Station;
                    }
                }

                var minRedEnt = DrawElevation(drawing, minRedElev, minRedElevSta, CadColor.Red);

                if (ValueConverter.CompValues(minRedElev, maxRedElev) != 0)
                {
                    var maxRedEnt = DrawElevation(drawing, maxRedElev, maxRedElevSta, CadColor.Red);
                    minRedEnt.Content = $"R(L): {minRedEnt.Content}";
                    maxRedEnt.Content = $"R(H): {maxRedEnt.Content}";
                }
            }
        }

        /// <summary>
        /// Отрисовка текстового примитива
        /// </summary>
        /// <param name="value"></param>
        /// <param name="station"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        private DwgText DrawElevation(Drawing drawing, double value, double station, CadColor color)
        {
            var textStyle = drawing.ActiveStyle;

            // Рассчитываем положение примитива с учётом масштаба макета
            var scaleStation = ScaleStation(station);
            var position = new Vector3D(scaleStation, 0.0, 0.0);

            // Генерируем динамический ключ для макета и создаём примитив
            var key = GenerateSimpleKey(this, scaleStation, "elevation");
            BeginMockup(key, position.Pos);
            try
            {
                var ent = drawing.ActiveSpace.AddText(ValueConverter.FloatToStr(value, Precision), position, textStyle.Height, textStyle.Ratio, Math.PI * 0.5, textStyle.Oblique);
                ent.Color = color;
                ent.Justify = this.DefaultTextJustify;
                return ent;
            }
            finally{ EndMockup(); }
        }
    }
}

using System;
using Topomatic.Alg;
using Topomatic.Cad.Foundation;
using Topomatic.Dwg;
using Topomatic.Dwg.Entities;
using Topomatic.Plt.Templates.Crs;

namespace TutorialFields.Profile
{
    class TutorialCrossectionField : CrsField
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
        
        public TutorialCrossectionField() : base()
        {
            Precision = 2;
        }

        protected override void OnDrawField()
        {
            base.OnDrawField();

            // Получаем активную ось профиля и её линии земли и конструкции проектного поперечника
            var alignment = DataManager["Alignment"] as Alignment;
            if (alignment == null) return;

            var drawing = Drawing;
            var corridor = alignment.Corridor;
            var sectionId = this.SectionIndex;
            var context = corridor[sectionId];
            var egContour = context.GetEgContour();
            var redContour = context.GetRedLineContour();

            // Рассчитываем и отрисовываем минимальные и максимальные отметки линий земли и конструкции проектного поперечника
            if (egContour.Count > 0)
            {
                var minEgElev = double.MaxValue;
                var minEgElevOffset = 0.0;
                var maxEgElev = double.MinValue;
                var maxEgElevOffset = 0.0;
                for (var i = 0; i < egContour.Count; i++)
                {
                    var node = egContour[i];
                    if (node.Y < minEgElev)
                    {
                        minEgElev = node.Y;
                        minEgElevOffset = node.X;
                    }
                    if (node.Y > maxEgElev)
                    {
                        maxEgElev = node.Y;
                        maxEgElevOffset = node.X;
                    }
                }

                var minEgEnt = DrawElevation(drawing, minEgElev, minEgElevOffset, CadColor.Green);

                if (ValueConverter.CompValues(minEgElev, maxEgElev) != 0)
                {
                    var maxEgEnt = DrawElevation(drawing, maxEgElev, maxEgElevOffset, CadColor.Green);
                    minEgEnt.Content = $"E(L): {minEgEnt.Content}";
                    maxEgEnt.Content = $"E(H): {maxEgEnt.Content}";
                }
            }

            if (redContour.Count > 0)
            {
                var minRedElev = double.MaxValue;
                var minRedElevOffset = 0.0;
                var maxRedElev = double.MinValue;
                var maxRedElevOffset = 0.0;
                for (var i = 0; i < redContour.Count; i++)
                {
                    var node = redContour[i];
                    if (node.Y < minRedElev)
                    {
                        minRedElev = node.Y;
                        minRedElevOffset = node.X;
                    }
                    if (node.Y > maxRedElev)
                    {
                        maxRedElev = node.Y;
                        maxRedElevOffset = node.X;
                    }
                }

                var minRedEnt = DrawElevation(drawing, minRedElev, minRedElevOffset, CadColor.Red);

                if (ValueConverter.CompValues(minRedElev, maxRedElev) != 0)
                {
                    var maxRedEnt = DrawElevation(drawing, maxRedElev, maxRedElevOffset, CadColor.Red);
                    minRedEnt.Content = $"R(L): {minRedEnt.Content}";
                    maxRedEnt.Content = $"R(H): {maxRedEnt.Content}";
                }
            }
        }

        /// <summary>
        /// Отрисовка текстового примитива
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        private DwgText DrawElevation(Drawing drawing, double value, double offset, CadColor color)
        {
            var textStyle = drawing.ActiveStyle;

            // Рассчитываем положение примитива с учётом масштаба макета
            var scaleOffset = ScaleOffset(offset);
            var position = new Vector3D(scaleOffset, 0.0, 0.0);

            // Генерируем динамический ключ для макета и создаём примитив
            var key = GenerateSimpleKey(this, scaleOffset, 0);
            BeginMockup(key, position.Pos);
            try
            {
                var ent = drawing.ActiveSpace.AddText(ValueConverter.FloatToStr(value, Precision), position,
                    textStyle.Height, textStyle.Ratio, Math.PI * 0.5, textStyle.Oblique);
                ent.Color = color;
                ent.Justify = this.DefaultTextJustify;
                return ent;
            }
            finally
            {
                EndMockup();
            }
        }
    }
}

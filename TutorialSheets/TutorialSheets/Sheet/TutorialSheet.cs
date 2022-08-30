using System.Collections.Generic;
using System.ComponentModel;
using Topomatic.Alg;
using Topomatic.Alg.Stationing;
using Topomatic.Alg.Tables;
using Topomatic.ComponentModel;
using Topomatic.Stg;
using Topomatic.Tables;
using Topomatic.Tables.Export;
using Topomatic.Tables.Sheets;

namespace TutorialSheets.Sheet
{
    class TutorialRow
    {
        [DesignAliasAttribute("Station"), DescriptionAttribute("Расстояние от начала пути")]
        public string Station { get; set; }

        [DesignAliasAttribute("Piket"), DescriptionAttribute("Пикет")]
        public string Piket { get; set; }

        [DesignAliasAttribute("EgElevation"), DescriptionAttribute("Отметка чёрного профиля")]
        public string EgElevation { get; set; }

        [DesignAliasAttribute("RedElevation"), DescriptionAttribute("Отметка красного профиля")]
        public string RedElevation { get; set; }

        [DesignAliasAttribute("ElevationDelta"), DescriptionAttribute("Разница отметок")]
        public string ElevationDelta { get; set; }
    }

    class TutorialSheet : TemplateStationingSheet
    {
        private struct HeadData
        {
            [DesignAlias("SheetName"), DescriptionAttribute("Имя ведомости")]
            public string SheetName { get; set; }
        }

        private object m_TutorialMoniker = new object();

        public TutorialSheet(string alignmentName, Alignment alignment) :
            base("TutorialSheet", "Рабочие отметки", $"{alignmentName} - ведомость рабочих отметок")
        {
            m_Alignment = alignment;
            UserSetting = false;
        }

        public override IAlgStationing Stationing
        {
            get
            {
                return m_Alignment.Stationing;
            }
        }

        public bool UserSetting { get; set; }

        protected override IDictionary<string, TemplateSheetSymbols> GetSheetSymbols()
        {
            var result = new Dictionary<string, TemplateSheetSymbols>();
            result.Add(TemplateSheetSymbols.DEFAULT_ID, new TemplateSheetSymbols(GetSymbols()));
            return result;
        }

        /// <summary>
        /// Определение строк таблицы
        /// </summary>
        /// <returns></returns>
        private IEnumerable<RowData> GetSymbols()
        {
            // Получаем границы в которых создаётся таблица
            var start_station = this.FromStation;
            var end_station = this.ToStation;
            if (this.All)
            {
                start_station = 0.0;
                end_station = m_Alignment.Plan.CompoundLine.Length;
            }

            // Получаем черный и красный профили модели
            var axisTransition = m_Alignment.Transitions[0];
            var egProfile = axisTransition.EgProfile;
            var redProfile = axisTransition.RedProfile;

            // Создаём коллекцию строк и добавляем в неё строку заголовка ведомости
            var result = new List<RowData>();
            result.Add(new SymbolContext("Head", new HeadData { SheetName = DefaultName }));
            var cnt = result.Count;
            // У каждой вершины черного профиля проверяем попадает ли она в границы ведомости
            // Определяем отметку красного профиля на соответствующем расстоянии от оси пути
            // Вычисляем рабочую отметку и формируем строку ведомости
            for (int i = 0; i < egProfile.Count; i++)
            {
                var egNode = egProfile[i];
                var sta = egNode.Station;
                if (AlignmentValueConverter.StationInLimits(sta, start_station, end_station, true, true))
                {
                    double redY;
                    var res = redProfile.GetY(sta, out redY);

                    if (res)
                    {
                        var row = new TutorialRow()
                        {
                            Station = TablesExtensions.FloatToStr(sta),
                            Piket = m_Alignment.Stationing.StationToString(sta),
                            EgElevation = TablesExtensions.FloatToStr(egNode.Elevation),
                            RedElevation = TablesExtensions.FloatToStr(redY),
                            ElevationDelta = TablesExtensions.FloatToStr(redY - egNode.Elevation)
                        };
                        result.Add(new SymbolContext("Row", row));
                    }
                }
            }

            if (cnt == result.Count)
                result.Add(new SymbolContext("Row", new TutorialRow()));
            return result;
        }

        public override IEnumerable<object> GetMonikers()
        {
            foreach (var m in base.GetMonikers())
                yield return m;
            yield return m_TutorialMoniker;
        }

        /// <summary>
        /// Получение фрейма с пользовательскими настройками
        /// </summary>
        /// <param name="moniker"></param>
        /// <returns></returns>
        public override UserSheetWizardFrame GetFrame(object moniker)
        {
            if (moniker == m_TutorialMoniker)
            {
                return new TutorialSheetFrame();
            }
            return base.GetFrame(moniker);
        }

        public override void LoadFromStg(StgNode node)
        {
            base.LoadFromStg(node);
            UserSetting = node.GetBoolean("UserSetting", false);
        }

        public override void SaveToStg(StgNode node)
        {
            base.SaveToStg(node);
            node.AddBoolean("UserSetting", UserSetting);
        }

        private Alignment m_Alignment;
    }
}
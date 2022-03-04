using System;
using System.Collections.Generic;
using System.Linq;
using SciChart.Charting.Model.ChartSeries;
using SciChart.Charting.Model.DataSeries;
using SciChart.Examples.ExternalDependencies.Data;
using Trader.Entities;

namespace Trader.ViewModels
{
    public class RsiPaneViewModel : BaseChartPaneViewModel
    {
        public RsiPaneViewModel(ChartControlViewModel parentViewModel, TCandleFactory candles) : base(parentViewModel, candles)
        {
            ChartSeriesViewModels.Add(new LineRenderableSeriesViewModel { DataSeries = candles.CurrentCandles.RsiData });
            YAxisTextFormatting = "0.0";
            Height = 100;
        }

        public override void Refresh()
        {
            ChartSeriesViewModels[0].DataSeries = Candles.CurrentCandles.RsiData;
        }
    }
}

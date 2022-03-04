using System;
using System.Linq;
using SciChart.Charting.Model.ChartSeries;
using SciChart.Charting.Model.DataSeries;
using SciChart.Examples.ExternalDependencies.Data;
using Trader.Entities;

namespace Trader.ViewModels
{
    public class VolumePaneViewModel : BaseChartPaneViewModel
    {

        public VolumePaneViewModel(ChartControlViewModel parentViewModel, TCandleFactory candles)
            : base(parentViewModel, candles)
        {
            ChartSeriesViewModels.Add(new ColumnRenderableSeriesViewModel { DataSeries = candles.CurrentCandles.VolumeData });
            YAxisTextFormatting = "###E+0";
        }

        public override void Refresh()
        {
            ChartSeriesViewModels[0].DataSeries = Candles.CurrentCandles.VolumeData;
        }
    }
}

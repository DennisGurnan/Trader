using System;
using System.Collections.Generic;
using System.Linq;
using SciChart.Charting.Model.ChartSeries;
using SciChart.Charting.Model.DataSeries;
using SciChart.Examples.ExternalDependencies.Data;
using Trader.Entities;

namespace Trader.ViewModels
{
    public class MacdPaneViewModel : BaseChartPaneViewModel
    {
        public MacdPaneViewModel(ChartControlViewModel parentViewModel, TCandleFactory candles)
    : base(parentViewModel, candles)
        {
            ChartSeriesViewModels.Add(new ColumnRenderableSeriesViewModel { DataSeries = candles.CurrentCandles.HistogramData });
            ChartSeriesViewModels.Add(new BandRenderableSeriesViewModel
            {
                DataSeries = candles.CurrentCandles.MacdData
            });
            YAxisTextFormatting = "0.00";
            Height = 100;
        }

        public MacdPoint GetLastMacdPoint()
        {
            if (Candles.CurrentCandles.MacdData.HasValues) return new MacdPoint()
            {
                Macd = Candles.CurrentCandles.MacdData.YValues.Last(),
                Signal = Candles.CurrentCandles.MacdData.Y1Values.Last(),
                Divergence = Candles.CurrentCandles.HistogramData.YValues.Last()
            };
            return new MacdPoint();
        }

        public override void Refresh()
        {
            ChartSeriesViewModels[0].DataSeries = Candles.CurrentCandles.HistogramData;
            ChartSeriesViewModels[1].DataSeries = Candles.CurrentCandles.MacdData;
        }
    }
}

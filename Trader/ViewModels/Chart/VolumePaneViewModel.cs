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
        private XyDataSeries<DateTime, double> volumePrices;

        public VolumePaneViewModel(ChartControlViewModel parentViewModel, TCandles candles)
            : base(parentViewModel, candles)
        {
            volumePrices = new XyDataSeries<DateTime, double>() { SeriesName = "Volume" };
            ChartSeriesViewModels.Add(new ColumnRenderableSeriesViewModel { DataSeries = volumePrices });
            YAxisTextFormatting = "###E+0";
        }

        public override void Reload()
        {
            volumePrices.Clear();
            if (Candles.VolumeData.Count > 0)
            {
                volumePrices.Append(Candles.TimeData, Candles.VolumeData);
            }
        }

        public override void Update()
        {
            if (Candles.VolumeData.Count > 0)
            {
                TCandle candle = Candles.GetLastCandle();
                if ((_LastCandle != null) && (_LastCandle.DateTime == candle.DateTime))
                {
                    volumePrices.Update(candle.DateTime, Candles.VolumeData.Last());
                }
                else
                {
                    volumePrices.Append(candle.DateTime, Candles.VolumeData.Last());
                }
                _LastCandle = candle;
            }
        }
    }
}

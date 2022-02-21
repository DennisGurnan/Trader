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
        private XyDataSeries<DateTime, double> rsiSeries;

        public RsiPaneViewModel(ChartControlViewModel parentViewModel, TCandles candles) : base(parentViewModel, candles)
        {
            rsiSeries = new XyDataSeries<DateTime, double>() { SeriesName = "RSI" };
            ChartSeriesViewModels.Add(new LineRenderableSeriesViewModel { DataSeries = rsiSeries });
            YAxisTextFormatting = "0.0";
            Height = 100;
        }

        public override void Reload()
        {
                rsiSeries.Clear();
                rsiSeries.Append(Candles.TimeData, Candles.RsiData);
        }

        public override void Update()
        {
            if (Candles.RsiData.Count > 0)
            {
                TCandle candle = Candles.GetLastCandle();
                if ((_LastCandle != null) && (_LastCandle.DateTime == candle.DateTime))
                {
                    rsiSeries.Update(candle.DateTime, Candles.RsiData.Last());
                }
                else
                {
                    rsiSeries.Append(candle.DateTime, Candles.RsiData.Last());
                }
                _LastCandle = candle;
            }
        }
    }
}

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
        private XyDataSeries<DateTime, double> histogramDataSeries;
        private XyyDataSeries<DateTime, double> macdDataSeries;
        public MacdPaneViewModel(ChartControlViewModel parentViewModel, TCandles candles)
    : base(parentViewModel, candles)
        {
            histogramDataSeries = new XyDataSeries<DateTime, double>() { SeriesName = "Histogram" };
            ChartSeriesViewModels.Add(new ColumnRenderableSeriesViewModel { DataSeries = histogramDataSeries });
            macdDataSeries = new XyyDataSeries<DateTime, double>() { SeriesName = "MACD" };
            ChartSeriesViewModels.Add(new BandRenderableSeriesViewModel
            {
                DataSeries = macdDataSeries
            });
            YAxisTextFormatting = "0.00";
            Height = 100;
        }

        public MacdPoint GetLastMacdPoint()
        {
            if (Candles.MacdData.Count > 0) return Candles.MacdData.Last();
            return new MacdPoint();
        }

        public override void Update()
        {
            try
            {
                TCandle candle = Candles.GetLastCandle();
                if (Candles.MacdData.Count > 0)
                {
                    if ((_LastCandle != null) && (_LastCandle.DateTime == candle.DateTime))
                    {
                        histogramDataSeries.Update(candle.DateTime, Candles.MacdData.Last().Divergence);
                        macdDataSeries.Update(candle.DateTime, Candles.MacdData.Last().Macd, Candles.MacdData.Last().Signal);
                    }
                    else
                    {
                        histogramDataSeries.Append(candle.DateTime, Candles.MacdData.Last().Divergence);
                        macdDataSeries.Append(candle.DateTime, Candles.MacdData.Last().Macd, Candles.MacdData.Last().Signal);
                    }
                    _LastCandle = candle;
                }
            }catch(Exception ex)
            {

            }
        }

        public override void Reload()
        {
            histogramDataSeries.Clear();
            histogramDataSeries.Append(Candles.TimeData, Candles.MacdData.Select(x => x.Divergence));
            macdDataSeries.Clear();
            macdDataSeries.Append(Candles.TimeData, Candles.MacdData.Select(x => x.Macd), Candles.MacdData.Select(x => x.Signal));
        }
    }
}

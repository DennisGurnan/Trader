using System;
using System.Linq;
using System.Windows.Media;
using SciChart.Charting.Model.ChartSeries;
using SciChart.Charting.Model.DataSeries;
using SciChart.Examples.ExternalDependencies.Data;
using SciChart.Charting.Visuals.Annotations;

using Trader.Entities;

namespace Trader.ViewModels
{
    public class PricePaneViewModel : BaseChartPaneViewModel
    {
        private readonly OhlcDataSeries<DateTime, double> stockPrices;
        private readonly XyDataSeries<DateTime, double> maLow;
        private readonly XyDataSeries<DateTime, double> maHigh;

        private readonly HorizontalLineAnnotation HiPrice;
        private readonly HorizontalLineAnnotation LoPrice;

        public PricePaneViewModel(ChartControlViewModel parentViewModel, TCandles candles)
            : base(parentViewModel, candles)
        {
            stockPrices = new OhlcDataSeries<DateTime, double>() { SeriesName = "OHLC" };
            ChartSeriesViewModels.Add(new CandlestickRenderableSeriesViewModel
            {
                DataSeries = stockPrices,
                StyleKey = "BaseRenderableSeriesStyle",
            });
            maLow = new XyDataSeries<DateTime, double>() { SeriesName = "Low Line" };
            ChartSeriesViewModels.Add(new LineRenderableSeriesViewModel
            {
                DataSeries = maLow,
                StyleKey = "LowLineStyle"
            });
            maHigh = new XyDataSeries<DateTime, double>() { SeriesName = "High Line" };
            ChartSeriesViewModels.Add(new LineRenderableSeriesViewModel
            {
                DataSeries = maHigh,
                StyleKey = "HighLineStyle",
            });
            YAxisTextFormatting = "0.########";
            XAxisTextFormatting = "H:mm MM/dd/yy";

            HiPrice = new HorizontalLineAnnotation()
            {
                Name = "HiPrice",
                HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                IsEditable = true,
                LabelPlacement = LabelPlacement.Axis,
                ShowLabel = true,
                Stroke = new SolidColorBrush(Color.FromRgb(255,0,0)),
                StrokeThickness = 1,
                X1 = 0,
                Y1 = 0
            };
        }

        public override void Reload()
        {
            stockPrices.Clear();
            stockPrices.Append(Candles.TimeData, Candles.OpenData, Candles.HighData, Candles.LowData, Candles.CloseData);
            maLow.Clear();
            maLow.Append(Candles.TimeData, Candles.LoLineData);
            maHigh.Clear();
            maHigh.Append(Candles.TimeData, Candles.HiLineData);
        }

        public override void Update()
        {
            TCandle lastCandle = Candles.GetLastCandle();
            if (lastCandle != null)
            {
                if ((_LastCandle != null) && (_LastCandle.DateTime == lastCandle.DateTime))
                {
                    stockPrices.Update(lastCandle.DateTime, lastCandle.Open, lastCandle.High, lastCandle.Low, lastCandle.Close);
                    maLow.Update(lastCandle.DateTime, Candles.LoLineData.Last());
                    maHigh.Update(lastCandle.DateTime, Candles.HiLineData.Last());
                }
                else
                {
                    stockPrices.Append(lastCandle.DateTime, lastCandle.Open, lastCandle.High, lastCandle.Low, lastCandle.Close);
                    maLow.Append(lastCandle.DateTime, Candles.LoLineData.Last());
                    maHigh.Append(lastCandle.DateTime, Candles.HiLineData.Last());
                }
                _LastCandle = lastCandle;
            }
        }
    }
}

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

        public PricePaneViewModel(ChartControlViewModel parentViewModel, TCandleFactory candles)
            : base(parentViewModel, candles)
        {
            ChartSeriesViewModels.Add(new CandlestickRenderableSeriesViewModel
            {
                DataSeries = candles.CurrentCandles.CandleData,
                StyleKey = "BaseRenderableSeriesStyle",
            });
            ChartSeriesViewModels.Add(new LineRenderableSeriesViewModel
            {
                DataSeries = candles.CurrentCandles.LowLineData,
                StyleKey = "LowLineStyle"
            });
            ChartSeriesViewModels.Add(new LineRenderableSeriesViewModel
            {
                DataSeries = candles.CurrentCandles.HighLineData,
                StyleKey = "HighLineStyle",
            });
            YAxisTextFormatting = "0.########";
            XAxisTextFormatting = "H:mm MM/dd/yy";
        }

        public override void Refresh()
        {
            ChartSeriesViewModels[0].DataSeries = Candles.CurrentCandles.CandleData;
            ChartSeriesViewModels[1].DataSeries = Candles.CurrentCandles.LowLineData;
            ChartSeriesViewModels[2].DataSeries = Candles.CurrentCandles.HighLineData;
        }
    }
}

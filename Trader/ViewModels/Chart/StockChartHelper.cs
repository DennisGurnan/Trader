using System.Linq;
using System.Windows;
using SciChart.Charting.ChartModifiers;
using SciChart.Charting.Visuals.TradeChart;

namespace Trader.ViewModels.Chart
{
    public static class StockChartHelper
    {
        public static readonly DependencyProperty ShowTooltipLabelProperty = DependencyProperty.RegisterAttached("ShowTooltipLabel", typeof(bool), typeof(StockChartHelper), new PropertyMetadata(default(bool), ShowTooltipLabelPropertyChanged));

        public static void SetShowTooltipLabel(DependencyObject element, bool value)
        {
            element.SetValue(ShowTooltipLabelProperty, value);
        }

        public static bool GetShowTooltipLabel(DependencyObject element)
        {
            return (bool)element.GetValue(ShowTooltipLabelProperty);
        }

        private static void ShowTooltipLabelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var stockChart = d as SciStockChart;
            if (stockChart != null)
            {
                var group = (ModifierGroup)stockChart.ChartModifier;
                var cursor = group.ChildModifiers.SingleOrDefault(x => x is CursorModifier) as CursorModifier;
                if (cursor != null)
                {
                    cursor.ShowTooltip = (bool)args.NewValue;
                }
            }
        }
    }
}

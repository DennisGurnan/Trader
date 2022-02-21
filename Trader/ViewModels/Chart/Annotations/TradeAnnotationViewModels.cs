using System;
using SciChart.Charting.Model.ChartSeries;
using Trader.Entities;

namespace Trader.ViewModels.Chart.Annotations
{
    public interface IBuySellAnnotationViewModel : IAnnotationViewModel
    {
        TTrade TradeData { get; set; }
    }

    // Viewmodel for the annotation type NewsBulletAnnotation
    public class NewsBulletAnnotationViewModel : BaseAnnotationViewModel
    {
        private TNewsEvent _newsEvent;

        public TNewsEvent NewsData
        {
            get { return _newsEvent; }
            set
            {
                _newsEvent = value;
                OnPropertyChanged("NewsData");
            }
        }

        public override Type ViewType { get { return typeof(NewsBulletAnnotation); } }
    }

    // Viewmodel for the annotation type BuyMarkerAnnotation
    public class BuyMarkerAnnotationViewModel : BaseAnnotationViewModel, IBuySellAnnotationViewModel
    {
        private TTrade _tradeData;

        public TTrade TradeData
        {
            get { return _tradeData; }
            set
            {
                _tradeData = value;
                OnPropertyChanged("TradeData");
            }
        }
        public override Type ViewType { get { return typeof(BuyMarkerAnnotation); } }
    }

    // Viewmodel for the annotation type SellMarkerAnnotation
    public class SellMarkerAnnotationViewModel : BaseAnnotationViewModel, IBuySellAnnotationViewModel
    {
        private TTrade _tradeData;

        public TTrade TradeData
        {
            get { return _tradeData; }
            set
            {
                _tradeData = value;
                OnPropertyChanged("TradeData");
            }
        }
        public override Type ViewType { get { return typeof(SellMarkerAnnotation); } }
    }

    // Viewmodel for the annotation type SellMarkerAnnotation
    public class StepAnnotationViewModel : BaseAnnotationViewModel, IBuySellAnnotationViewModel
    {
        private TTrade _tradeData;

        public TTrade TradeData
        {
            get { return _tradeData; }
            set
            {
                _tradeData = value;
                OnPropertyChanged("TradeData");
            }
        }
        public override Type ViewType { get { return typeof(StepAnnotation); } }
    }
}

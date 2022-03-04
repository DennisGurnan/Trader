using System;
using Tinkoff.InvestApi.V1;
using SciChart.Examples.ExternalDependencies.Data;
using Binance.Net.Interfaces;
namespace Trader.Entities
{
    public enum TCandleInterval
    {
        _1min, _5min, _15min, _30min, _60min, _day
    }

    public class TCandle
    {
        public double Open;
        public double Close;
        public double High;
        public double Low;
        public long Volume;
        public DateTime DateTime;
        public TCandleInterval Interval;

        public TCandle()
        {

        }

        public TCandle(TCandle c)
        {
            FromCandle(c);
        }

        public TCandle(Candle c, TCandleInterval i)
        {
            FromCandle(c, i);
        }

        public TCandle(HistoricCandle c, TCandleInterval i)
        {
            FromCandle(c, i);
        }

        public TCandle(PriceBar c, TCandleInterval i)
        {
            FromCandle(c, i);
        }

        public TCandle(IBinanceKline c, TCandleInterval i)
        {
            FromCandle(c, i);
        }

        public void FromCandle(IBinanceKline c, TCandleInterval i)
        {
            Open = (double)c.Open;
            Close = (double)c.Close;
            High = (double)c.High;
            Low = (double)c.Low;
            Volume = (long)c.BaseVolume;
            DateTime = c.OpenTime;
            Interval = i;
        }
        // Fill from Tinkoff.Candle
        public void FromCandle(Candle candle, TCandleInterval i)
        {
            Open = Utils.Convertor.QuotationToDouble(candle.Open);
            Close = Utils.Convertor.QuotationToDouble(candle.Close);
            High = Utils.Convertor.QuotationToDouble(candle.High);
            Low = Utils.Convertor.QuotationToDouble(candle.Low);
            Volume = candle.Volume;
            DateTime = candle.Time.ToDateTime();
            Interval = i;
        }

        // Fill from Tinkoff.HistoricCandle
        public void FromCandle(HistoricCandle candle, TCandleInterval i)
        {
            Open = Utils.Convertor.QuotationToDouble(candle.Open);
            Close = Utils.Convertor.QuotationToDouble(candle.Close);
            High = Utils.Convertor.QuotationToDouble(candle.High);
            Low = Utils.Convertor.QuotationToDouble(candle.Low);
            Volume = candle.Volume;
            DateTime = candle.Time.ToDateTime();
            Interval = i;
        }

        public void FromCandle(PriceBar candle, TCandleInterval i)
        {
            Open = candle.Open;
            Close = candle.Close;
            High = candle.High;
            Low = candle.Low;
            Volume = candle.Volume;
            DateTime = candle.DateTime;
            Interval = i;
        }

        public void FromCandle(TCandle candle)
        {
            Open = candle.Open;
            Close = candle.Close;
            High = candle.High;
            Low = candle.Low;
            Volume = candle.Volume;
            DateTime = candle.DateTime;
            Interval = candle.Interval;
        }
    }
}

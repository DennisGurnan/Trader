using System;
using Tinkoff.InvestApi.V1;
using Trader.MVVM;
using Binance.Net.Objects.Spot.MarketData;
using Binance.Net.Objects.Spot.SpotData;

namespace Trader.Entities
{
    public enum InstrumentType
    {
        Bond, Curency, Future, Share, Etf, All
    }

    public class TInstrument : ObservableObject
    {
        #region Private vars
        private string _Name;
        private string _Exchange;
        private string _Figi;
        private string _Ticker;
        private string _ClassCode;
        private string _Isin;
        private string _InstrumentType;
        private string _RebalancingFreq;
        private string _FocusType;
        private string _Currency;
        private string _IsoCurrencyName;
        private string _FuturesType;
        private string _AssetType;
        private string _BasicAsset;
        private string _CountryOfRisk;
        private string _CountryOfRiskName;
        private string _Sector;
        private string _IssueKind;
        private int _Lot;
        private int _CouponQuantityPerYear;
        private decimal _Klong;
        private decimal _Kshort;
        private decimal _Dlong;
        private decimal _Dshort;
        private decimal _DlongMin;
        private decimal _DshortMin;
        private decimal _Nominal;
        private decimal _FixedCommision;
        private decimal _NumShares;
        private decimal _PlacementPrice;
        private decimal _AciValue;
        private decimal _MinPriceIncrement;
        private decimal _BasicAssetSize;
        private decimal _LastPrice;
        private long _IssueSize;
        private long _IssueSizePlan;
        private bool _ShortEnabledFlag;
        private bool _OtcFlag;
        private bool _BuyAvailableFlag;
        private bool _SellAvailableFlag;
        private bool _DivYieldFlag;
        private bool _FloatingCouponFlag;
        private bool _PerpetualFlag;
        private bool _AmortizationFlag;
        private bool _ApiTradeAvailableFlag;
        private ShareType _ShareType;
        private SecurityTradingStatus _TradingStatus;
        private DateTime _IpoDate;
        private DateTime _MaturityDate;
        private DateTime _ReleasedDate;
        private DateTime _StateRegDate;
        private DateTime _PlacementDate;
        private DateTime _FirstTradeDate;
        private DateTime _LastTradeDate;
        private DateTime _ExpirationDate;
        private DateTime _LastTimeUpdate;
        private DateTime _LastPriceTime;
        #endregion
        #region Properties
        public string Name              { get => _Name; set {_Name = value; RaisePropertyChangedEvent("Name"); } }
        public string Exchange          { get => _Exchange; set { _Exchange = value; RaisePropertyChangedEvent("Exchange"); } }
        public string Figi              { get => _Figi; set { _Figi = value; RaisePropertyChangedEvent("Figi"); } }
        public string Ticker            { get => _Ticker; set { _Ticker = value; RaisePropertyChangedEvent("Ticker"); } }
        public string ClassCode         { get => _ClassCode; set { _ClassCode = value; RaisePropertyChangedEvent("ClassCode"); } }
        public string Isin              { get => _Isin; set { _Isin = value; RaisePropertyChangedEvent("Isin"); } }
        public string InstrumentType    { get => _InstrumentType; set { _InstrumentType = value; RaisePropertyChangedEvent("InstrumentType"); } }
        public string RebalancingFreq   { get => _RebalancingFreq; set { _RebalancingFreq = value; RaisePropertyChangedEvent("RebalancingFreq"); } }
        public string FocusType         { get => _FocusType; set { _FocusType = value; RaisePropertyChangedEvent("FocusType"); } }
        public string Currency          { get => _Currency; set { _Currency = value; RaisePropertyChangedEvent("Currency"); } }
        public string IsoCurrencyName   { get => _IsoCurrencyName; set { _IsoCurrencyName = value; RaisePropertyChangedEvent("IsoCurrencyName"); } }
        public string FuturesType       { get => _FuturesType; set { _FuturesType = value; RaisePropertyChangedEvent("FuturesType"); } }
        public string AssetType         { get => _AssetType; set { _AssetType = value; RaisePropertyChangedEvent("AssetType"); } }
        public string BasicAsset        { get => _BasicAsset; set { _BasicAsset = value; RaisePropertyChangedEvent("BasicAsset"); } }
        public string CountryOfRisk     { get => _CountryOfRisk; set { _CountryOfRisk = value; RaisePropertyChangedEvent("CountryOfRisk"); } }
        public string CountryOfRiskName { get => _CountryOfRiskName; set { _CountryOfRiskName = value; RaisePropertyChangedEvent("CountryOfRiskName"); } }
        public string Sector            { get => _Sector; set { _Sector = value; RaisePropertyChangedEvent("Sector"); } }
        public string IssueKind         { get => _IssueKind; set { _IssueKind = value; RaisePropertyChangedEvent("IssueKind"); } }
        public int Lot                  { get => _Lot; set { _Lot = value; RaisePropertyChangedEvent("Lot"); } }
        public int CouponQuantityPerYear { get => _CouponQuantityPerYear; set { _CouponQuantityPerYear = value; RaisePropertyChangedEvent("CouponQuantityPerYear"); } }
        public decimal Klong            { get => _Klong; set { _Klong = value; RaisePropertyChangedEvent("Klong"); } }
        public decimal Kshort           { get => _Kshort; set { _Kshort = value; RaisePropertyChangedEvent("Kshort"); } }
        public decimal Dlong            { get => _Dlong; set { _Dlong = value; RaisePropertyChangedEvent("Dlong"); } }
        public decimal Dshort           { get => _Dshort; set { _Dshort = value; RaisePropertyChangedEvent("Dshort"); } }
        public decimal DlongMin         { get => _DlongMin; set { _DlongMin = value; RaisePropertyChangedEvent("DlongMin"); } }
        public decimal DshortMin        { get => _DshortMin; set { _DshortMin = value; RaisePropertyChangedEvent("DshortMin"); } }
        public decimal Nominal          { get => _Nominal; set { _Nominal = value; RaisePropertyChangedEvent("Nominal"); } }
        public decimal FixedCommision   { get => _FixedCommision; set { _FixedCommision = value; RaisePropertyChangedEvent("FixedCommision"); } }
        public decimal NumShares        { get => _NumShares; set { _NumShares = value; RaisePropertyChangedEvent("NumShares"); } }
        public decimal PlacementPrice   { get => _PlacementPrice; set { _PlacementPrice = value; RaisePropertyChangedEvent("PlacementPrice"); } }
        public decimal AciValue         { get => _AciValue; set { _AciValue = value; RaisePropertyChangedEvent("AciValue"); } }
        public decimal MinPriceIncrement { get => _MinPriceIncrement; set { _MinPriceIncrement = value; RaisePropertyChangedEvent("MinPriceIncrement"); } }
        public decimal BasicAssetSize   { get => _BasicAssetSize; set { _BasicAssetSize = value; RaisePropertyChangedEvent("BasicAssetSize"); } }
        public decimal LastPrice        { get => _LastPrice; set { _LastPrice = value; RaisePropertyChangedEvent("LastPrice"); } }
        public long IssueSize           { get => _IssueSize; set { _IssueSize = value; RaisePropertyChangedEvent("IssueSize"); } }
        public long IssueSizePlan       { get => _IssueSizePlan; set { _IssueSizePlan = value; RaisePropertyChangedEvent("IssueSizePlan"); } }
        public bool ShortEnabledFlag    { get => _ShortEnabledFlag; set { _ShortEnabledFlag = value; RaisePropertyChangedEvent("ShortEnabledFlag"); } }
        public bool OtcFlag             { get => _OtcFlag; set { _OtcFlag = value; RaisePropertyChangedEvent("OtcFlag"); } }
        public bool BuyAvailableFlag    { get => _BuyAvailableFlag; set { _BuyAvailableFlag = value; RaisePropertyChangedEvent("BuyAvailableFlag"); } }
        public bool SellAvailableFlag   { get => _SellAvailableFlag; set { _SellAvailableFlag = value; RaisePropertyChangedEvent("SellAvailableFlag"); } }
        public bool DivYieldFlag        { get => _DivYieldFlag; set { _DivYieldFlag = value; RaisePropertyChangedEvent("DivYieldFlag"); } }
        public bool FloatingCouponFlag  { get => _FloatingCouponFlag; set { _FloatingCouponFlag = value; RaisePropertyChangedEvent("FloatingCouponFlag"); } }
        public bool PerpetualFlag       { get => _PerpetualFlag; set { _PerpetualFlag = value; RaisePropertyChangedEvent("PerpetualFlag"); } }
        public bool AmortizationFlag    { get => _AmortizationFlag; set { _AmortizationFlag = value; RaisePropertyChangedEvent("AmortizationFlag"); } }
        public bool ApiTradeAvailableFlag { get => _ApiTradeAvailableFlag; set { _ApiTradeAvailableFlag = value; RaisePropertyChangedEvent("ApiTradeAvailableFlag"); } }
        public ShareType ShareType      { get => _ShareType; set { _ShareType = value; RaisePropertyChangedEvent("ShareType"); } }
        public SecurityTradingStatus TradingStatus { get => _TradingStatus; set { _TradingStatus = value; RaisePropertyChangedEvent("TradingStatus"); } }
        public DateTime IpoDate         { get => _IpoDate; set { _IpoDate = value; RaisePropertyChangedEvent("IpoDate"); } }
        public DateTime MaturityDate    { get => _MaturityDate; set { _MaturityDate = value; RaisePropertyChangedEvent("MaturityDate"); } }
        public DateTime ReleasedDate    { get => _ReleasedDate; set { _ReleasedDate = value; RaisePropertyChangedEvent("ReleasedDate"); } }
        public DateTime StateRegDate    { get => _StateRegDate; set { _StateRegDate = value; RaisePropertyChangedEvent("StateRegDate"); } }
        public DateTime PlacementDate   { get => _PlacementDate; set { _PlacementDate = value; RaisePropertyChangedEvent("PlacementDate"); } }
        public DateTime FirstTradeDate  { get => _FirstTradeDate; set { _FirstTradeDate = value; RaisePropertyChangedEvent("FirstTradeDate"); } }
        public DateTime LastTradeDate   { get => _LastTradeDate; set { _LastTradeDate = value; RaisePropertyChangedEvent("LastTradeDate"); } }
        public DateTime ExpirationDate  { get => _ExpirationDate; set { _ExpirationDate = value; RaisePropertyChangedEvent("ExpirationDate"); } }
        public DateTime LastTimeUpdate  { get => _LastTimeUpdate; set { _LastTimeUpdate = value; RaisePropertyChangedEvent("LastTimeUpdate"); } }
        public DateTime LastPriceTime   { get => _LastPriceTime; set { _LastPriceTime = value; RaisePropertyChangedEvent("LastPriceTime"); } }
        // Cripto
        public int _BaseAssetPrecision;
        public int _QuoteAssetPrecision;
        public bool _IcebergAllowed;
        public bool _OcoAllowed;
        public bool _IsSpotTradingAllowed;
        public bool _IsMarginTradingAllowed;
        public int BaseAssetPrecision { get => _BaseAssetPrecision; set { _BaseAssetPrecision = value; RaisePropertyChangedEvent("BaseAssetPrecision"); } }
        public int QuoteAssetPrecision { get => _QuoteAssetPrecision; set { _QuoteAssetPrecision = value; RaisePropertyChangedEvent("QuoteAssetPrecision"); } }
        public bool IcebergAllowed { get => _IcebergAllowed; set { _IcebergAllowed = value; RaisePropertyChangedEvent("IcebergAllowed"); } }
        public bool OcoAllowed { get => _OcoAllowed; set { _OcoAllowed = value; RaisePropertyChangedEvent("OcoAllowed"); } }
        public bool IsSpotTradingAllowed { get => _IsSpotTradingAllowed; set { _IsSpotTradingAllowed = value; RaisePropertyChangedEvent("IsSpotTradingAllowed"); } }
        public bool IsMarginTradingAllowed { get => _IsMarginTradingAllowed; set { _IsMarginTradingAllowed = value; RaisePropertyChangedEvent("IsMarginTradingAllowed"); } }
        public decimal LastLotPrice
        {
            get => LastPrice * Lot;
        }
        #endregion
        #region Constructor
        public TInstrument()
        {
        }

        public TInstrument(Instrument instr = null)
        {
            if (instr != null) TnkUpdate(instr);
        }

        public TInstrument(string figi)
        {
            Figi = figi;
        }
        #endregion
        #region Update
        public void TnkUpdate(Instrument instr)
        {
            Figi = instr.Figi;
            Ticker = instr.Ticker;
            ClassCode = instr.ClassCode;
            Isin = instr.Isin;
            Lot = instr.Lot;
            Currency = instr.Currency;
            Klong = Utils.Convertor.QuotationToDec(instr.Klong);
            Kshort = Utils.Convertor.QuotationToDec(instr.Kshort);
            Dlong = Utils.Convertor.QuotationToDec(instr.Dlong);
            Dshort = Utils.Convertor.QuotationToDec(instr.Dshort);
            DlongMin = Utils.Convertor.QuotationToDec(instr.DlongMin);
            DshortMin = Utils.Convertor.QuotationToDec(instr.DshortMin);
            ShortEnabledFlag = instr.ShortEnabledFlag;
            Name = instr.Name;
            Exchange = instr.Exchange;
            CountryOfRisk = instr.CountryOfRisk;
            CountryOfRiskName = instr.CountryOfRiskName;
            InstrumentType = instr.InstrumentType;
            TradingStatus = instr.TradingStatus;
            OtcFlag = instr.OtcFlag;
            BuyAvailableFlag = instr.BuyAvailableFlag;
            SellAvailableFlag = instr.SellAvailableFlag;
            MinPriceIncrement = Utils.Convertor.QuotationToDec(instr.MinPriceIncrement);
            ApiTradeAvailableFlag = instr.ApiTradeAvailableFlag;
            LastTimeUpdate = DateTime.Now;
        }

        public void TnkUpdate(Share instr)
        {
            Figi = instr.Figi;
            Ticker = instr.Ticker;
            ClassCode = instr.ClassCode;
            Isin = instr.Isin;
            Lot = instr.Lot;
            Currency = instr.Currency;
            Klong = Utils.Convertor.QuotationToDec(instr.Klong);
            Kshort = Utils.Convertor.QuotationToDec(instr.Kshort);
            Dlong = Utils.Convertor.QuotationToDec(instr.Dlong);
            Dshort = Utils.Convertor.QuotationToDec(instr.Dshort);
            DlongMin = Utils.Convertor.QuotationToDec(instr.DlongMin);
            DshortMin = Utils.Convertor.QuotationToDec(instr.DshortMin);
            ShortEnabledFlag = instr.ShortEnabledFlag;
            Name = instr.Name;
            Exchange = instr.Exchange;
            IpoDate = (instr.IpoDate == null) ? DateTime.MinValue : instr.IpoDate.ToDateTime();
            IssueSize = instr.IssueSize;
            CountryOfRisk = instr.CountryOfRisk;
            CountryOfRiskName = instr.CountryOfRiskName;
            InstrumentType = "share";
            Sector = instr.Sector;
            IssueSizePlan = instr.IssueSizePlan;
            Nominal = Utils.Convertor.MoneyToDec(instr.Nominal);
            TradingStatus = instr.TradingStatus;
            OtcFlag = instr.OtcFlag;
            BuyAvailableFlag = instr.BuyAvailableFlag;
            SellAvailableFlag = instr.SellAvailableFlag;
            DivYieldFlag = instr.DivYieldFlag;
            ShareType = instr.ShareType;
            MinPriceIncrement = Utils.Convertor.QuotationToDec(instr.MinPriceIncrement);
            ApiTradeAvailableFlag = instr.ApiTradeAvailableFlag;
            LastTimeUpdate = DateTime.Now;
        }

        public void TnkUpdate(Future instr)
        {
            Figi = instr.Figi;
            Ticker = instr.Ticker;
            ClassCode = instr.ClassCode;
            Lot = instr.Lot;
            Currency = instr.Currency;
            Klong = Utils.Convertor.QuotationToDec(instr.Klong);
            Kshort = Utils.Convertor.QuotationToDec(instr.Kshort);
            Dlong = Utils.Convertor.QuotationToDec(instr.Dlong);
            Dshort = Utils.Convertor.QuotationToDec(instr.Dshort);
            DlongMin = Utils.Convertor.QuotationToDec(instr.DlongMin);
            DshortMin = Utils.Convertor.QuotationToDec(instr.DshortMin);
            ShortEnabledFlag = instr.ShortEnabledFlag;
            Name = instr.Name;
            Exchange = instr.Exchange;
            FirstTradeDate = (instr.FirstTradeDate == null) ? DateTime.MinValue : instr.FirstTradeDate.ToDateTime();
            LastTradeDate = (instr.LastTradeDate == null) ? DateTime.MinValue : instr.LastTradeDate.ToDateTime();
            AssetType = instr.AssetType;
            BasicAsset = instr.BasicAsset;
            BasicAssetSize = Utils.Convertor.QuotationToDec(instr.BasicAssetSize);
            CountryOfRisk = instr.CountryOfRisk;
            CountryOfRiskName = instr.CountryOfRiskName;
            InstrumentType = "future";
            Sector = instr.Sector;
            ExpirationDate = (instr.ExpirationDate == null) ? DateTime.MinValue : instr.ExpirationDate.ToDateTime();
            TradingStatus = instr.TradingStatus;
            OtcFlag = instr.OtcFlag;
            BuyAvailableFlag = instr.BuyAvailableFlag;
            SellAvailableFlag = instr.SellAvailableFlag;
            MinPriceIncrement = Utils.Convertor.QuotationToDec(instr.MinPriceIncrement);
            ApiTradeAvailableFlag = instr.ApiTradeAvailableFlag;
            LastTimeUpdate = DateTime.Now;
        }

        public void TnkUpdate(Etf instr)
        {
            Figi = instr.Figi;
            Ticker = instr.Ticker;
            ClassCode = instr.ClassCode;
            Isin = instr.Isin;
            Lot = instr.Lot;
            Currency = "rub";
            Klong = Utils.Convertor.QuotationToDec(instr.Klong);
            Kshort = Utils.Convertor.QuotationToDec(instr.Kshort);
            Dlong = Utils.Convertor.QuotationToDec(instr.Dlong);
            Dshort = Utils.Convertor.QuotationToDec(instr.Dshort);
            DlongMin = Utils.Convertor.QuotationToDec(instr.DlongMin);
            DshortMin = Utils.Convertor.QuotationToDec(instr.DshortMin);
            ShortEnabledFlag = instr.ShortEnabledFlag;
            Name = instr.Name;
            Exchange = instr.Exchange;
            Nominal = 0;
            FixedCommision = Utils.Convertor.QuotationToDec(instr.FixedCommission);
            FocusType = instr.FocusType;
            ReleasedDate = (instr.ReleasedDate == null) ? DateTime.MinValue : instr.ReleasedDate.ToDateTime();
            NumShares = Utils.Convertor.QuotationToDec(instr.NumShares);
            CountryOfRisk = instr.CountryOfRisk;
            CountryOfRiskName = instr.CountryOfRiskName;
            Sector = instr.Sector;
            InstrumentType = "etf";
            RebalancingFreq = instr.RebalancingFreq;
            TradingStatus = instr.TradingStatus;
            OtcFlag = instr.OtcFlag;
            BuyAvailableFlag = instr.BuyAvailableFlag;
            SellAvailableFlag = instr.SellAvailableFlag;
            MinPriceIncrement = Utils.Convertor.QuotationToDec(instr.MinPriceIncrement);
            ApiTradeAvailableFlag = instr.ApiTradeAvailableFlag;
            LastTimeUpdate = DateTime.Now;
        }

        public void TnkUpdate(Currency instr)
        {
            Figi = instr.Figi;
            Ticker = instr.Ticker;
            ClassCode = instr.ClassCode;
            Isin = instr.Isin;
            Lot = instr.Lot;
            Currency = "rub";
            Klong = Utils.Convertor.QuotationToDec(instr.Klong);
            Kshort = Utils.Convertor.QuotationToDec(instr.Kshort);
            Dlong = Utils.Convertor.QuotationToDec(instr.Dlong);
            Dshort = Utils.Convertor.QuotationToDec(instr.Dshort);
            DlongMin = Utils.Convertor.QuotationToDec(instr.DlongMin);
            DshortMin = Utils.Convertor.QuotationToDec(instr.DshortMin);
            ShortEnabledFlag = instr.ShortEnabledFlag;
            Name = instr.Name;
            Exchange = instr.Exchange;
            Nominal = Utils.Convertor.MoneyToDec(instr.Nominal);
            CountryOfRisk = instr.CountryOfRisk;
            CountryOfRiskName = instr.CountryOfRiskName;
            InstrumentType = "currency";
            TradingStatus = instr.TradingStatus;
            OtcFlag = instr.OtcFlag;
            BuyAvailableFlag = instr.BuyAvailableFlag;
            SellAvailableFlag = instr.SellAvailableFlag;
            IsoCurrencyName = instr.IsoCurrencyName;
            MinPriceIncrement = Utils.Convertor.QuotationToDec(instr.MinPriceIncrement);
            ApiTradeAvailableFlag = instr.ApiTradeAvailableFlag;
            LastTimeUpdate = DateTime.Now;
        }

        public void TnkUpdate(Bond instr)
        {
            Figi = instr.Figi;
            Ticker = instr.Ticker;
            ClassCode = instr.ClassCode;
            Lot = instr.Lot;
            Currency = instr.Currency;
            Klong = Utils.Convertor.QuotationToDec(instr.Klong);
            Kshort = Utils.Convertor.QuotationToDec(instr.Kshort);
            Dlong = Utils.Convertor.QuotationToDec(instr.Dlong);
            Dshort = Utils.Convertor.QuotationToDec(instr.Dshort);
            DlongMin = Utils.Convertor.QuotationToDec(instr.DlongMin);
            DshortMin = Utils.Convertor.QuotationToDec(instr.DshortMin);
            ShortEnabledFlag = instr.ShortEnabledFlag;
            Name = instr.Name;
            Exchange = instr.Exchange;
            CouponQuantityPerYear = instr.CouponQuantityPerYear;
            MaturityDate = (instr.MaturityDate == null) ? DateTime.MinValue : instr.MaturityDate.ToDateTime();
            Nominal = Utils.Convertor.MoneyToDec(instr.Nominal);
            StateRegDate = (instr.StateRegDate == null)?DateTime.MinValue : instr.StateRegDate.ToDateTime();
            PlacementDate = (instr.PlacementDate == null) ? DateTime.MinValue : instr.PlacementDate.ToDateTime();
            PlacementPrice = Utils.Convertor.MoneyToDec(instr.PlacementPrice);
            AciValue = Utils.Convertor.MoneyToDec(instr.AciValue);
            CountryOfRisk = instr.CountryOfRisk;
            CountryOfRiskName = instr.CountryOfRiskName;
            IssueKind = instr.IssueKind;
            IssueSize = instr.IssueSize;
            IssueSizePlan = instr.IssueSizePlan;
            InstrumentType = "bond";
            TradingStatus = instr.TradingStatus;
            OtcFlag = instr.OtcFlag;
            BuyAvailableFlag = instr.BuyAvailableFlag;
            SellAvailableFlag = instr.SellAvailableFlag;
            FloatingCouponFlag = instr.FloatingCouponFlag;
            PerpetualFlag = instr.PerpetualFlag;
            AmortizationFlag = instr.AmortizationFlag;
            MinPriceIncrement = Utils.Convertor.QuotationToDec(instr.MinPriceIncrement);
            ApiTradeAvailableFlag = instr.ApiTradeAvailableFlag;
            LastTimeUpdate = DateTime.Now;
        }

        public void BinUpdate(BinanceSymbol instr)
        {
            Name = instr.BaseAsset + "<-->" + instr.QuoteAsset;
            Ticker = instr.BaseAsset;
            InstrumentType = "Exchange";
            Currency = instr.QuoteAsset;
            ClassCode = "CryproExchange";
            BaseAssetPrecision = instr.BaseAssetPrecision;
            QuoteAssetPrecision = instr.QuoteAssetPrecision;
            IcebergAllowed = instr.IceBergAllowed;
            OcoAllowed = instr.OCOAllowed;
            IsSpotTradingAllowed = instr.IsSpotTradingAllowed;
            IsMarginTradingAllowed = instr.IsMarginTradingAllowed;
        }
        public void BinUpdate(string asset)
        {
            Name = asset;
            Currency = asset;
            Ticker = asset;
            InstrumentType = "Currency";
            ClassCode = "Crypto";
        }
        #endregion
    }
}

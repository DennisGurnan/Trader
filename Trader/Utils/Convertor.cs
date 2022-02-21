using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tinkoff.InvestApi.V1;

namespace Trader.Utils
{
    public static class Convertor
    {
        static public decimal MoneyToDec(MoneyValue money)
        {
            if (money == null) return 0;
            money.Nano = Math.Abs(money.Nano);
            string sum = money.Units.ToString() + "," + money.Nano.ToString();
            return decimal.Parse(sum);
        }

        static public double MoneyToDouble(MoneyValue money)
        {
            if (money == null) return 0;
            money.Nano = Math.Abs(money.Nano);
            string sum = money.Units.ToString() + "," + money.Nano.ToString();
            return double.Parse(sum);
        }

        static public string MoneyToString(MoneyValue money)
        {
            money.Nano = Math.Abs(money.Nano);
            return MoneyToDec(money).ToString("N2") + " " + money.Currency;
        }

        static public MoneyValue DecToMoney(decimal d, string c)
        {
            MoneyValue v = new MoneyValue();
            v.Currency = c;
            v.Units = (long)Math.Truncate(d);
            v.Nano = (int)((d - (decimal)v.Units) * 1000000000);
            return v;
        }

        static public decimal QuotationToDec(Quotation q)
        {
            if (q == null) return 0;
            q.Nano = Math.Abs(q.Nano);
            string sum = q.Units.ToString() + "," + q.Nano.ToString();
            return decimal.Parse(sum);
        }

        static public double QuotationToDouble(Quotation q)
        {
            if (q == null) return 0;
            q.Nano = Math.Abs(q.Nano);
            string sum = q.Units.ToString() + "," + q.Nano.ToString();
            return double.Parse(sum);
        }

        static public Quotation DecToQuotation(decimal d)
        {
            Quotation q = new Quotation();
            q.Units = (long)Math.Truncate(d);
            q.Nano = (int)((d - (decimal)q.Units) * 1000000000);
            return q;
        }
    }
}

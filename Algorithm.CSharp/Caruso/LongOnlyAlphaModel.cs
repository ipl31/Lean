using QuantConnect.Algorithm.Framework.Alphas;
using QuantConnect.Data;
using QuantConnect.Data.UniverseSelection;
using QuantConnect.Securities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantConnect.Algorithm.CSharp
{
    class LongOnlyAlphaModel : AlphaModel
    {
        List<Symbol> investedSymbols = new List<Symbol>();
        // This is trading days, avg per month is 21. I should probably do this on a month by
        // month calcuation because there is at least one month with less than 21 days. In this case I
        // think we would needlessly sell the portfolio for a day.
        TimeSpan period = TimeSpan.FromDays(21);
        object investedSecuritiesUpdateLock = new object();
        int insightsLastUpdatedMonth = -1;

        public override IEnumerable<Insight> Update(QCAlgorithm algorithm, Slice data)
        {
            List<Insight> insights = new List<Insight>();
            // Only update insights once a month. This is hacky, need to come up with a cleaner solution. 
            int currentMonth = DateTime.Now.Month;
            if (currentMonth != this.insightsLastUpdatedMonth)
            {
                // Generate buy and sell insights on Update calls.
                insights = insights.Concat(this.GenerateBuyInsights(algorithm)).ToList();
                this.insightsLastUpdatedMonth = currentMonth;
            }
            return insights;
        }

        public override void OnSecuritiesChanged(QCAlgorithm algorithm, SecurityChanges changes)
        {
            this.ProcessSecuritiesChanges(changes);
        }

        private List<Insight> GenerateBuyInsights(QCAlgorithm algorithm)
        {
            List<Insight> insights = new List<Insight>();
            foreach (Symbol symbol in this.investedSymbols)
            {
                insights.Add(Insight.Price(symbol, this.period, InsightDirection.Up));
            }

            return insights;

        }

        private void ProcessSecuritiesChanges(SecurityChanges securitiesChanges)
        {
            lock (this.investedSecuritiesUpdateLock)
                foreach (Security security in securitiesChanges.RemovedSecurities)
                {
                    this.investedSymbols.Remove(security.Symbol);
                }

            foreach (Security security in securitiesChanges.AddedSecurities)
            {
                this.investedSymbols.Add(security.Symbol);
            }
        }

    }
}

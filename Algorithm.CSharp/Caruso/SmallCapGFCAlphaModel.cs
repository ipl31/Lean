using QuantConnect.Algorithm.Framework.Alphas;
using QuantConnect.Data;
using QuantConnect.Securities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantConnect.Algorithm.CSharp.Caruso
{
    public partial class SmallCapGFCAlphaModel : AlphaModel
    {
        bool initialized = false;
        TimeSpan signalValidityPeriod;

        public SmallCapGFCAlphaModel(TimeSpan signalValidityPeriod)
        {
            this.signalValidityPeriod = signalValidityPeriod;
        }

        public override IEnumerable<Insight> Update(QCAlgorithm algorithm, Slice data)
        {
            List<Insight> insights = new List<Insight>();
            if (!initialized)
            {
                foreach (KeyValuePair<Symbol, BaseData> keyValuePair in data)
                {
                    insights.Add(Insight.Price(keyValuePair.Key, this.signalValidityPeriod, InsightDirection.Up));
                }

            }
            this.initialized = true;
            return insights;
        }

    }
}

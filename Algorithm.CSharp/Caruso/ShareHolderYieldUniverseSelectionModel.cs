using QuantConnect.Algorithm.Framework.Selection;
using QuantConnect.Data.Fundamental;
using QuantConnect.Data.UniverseSelection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantConnect.Algorithm.CSharp
{
    class ShareHolderYieldLiquidUniverseSelectionModel : FundamentalUniverseSelectionModel
    {

        public ShareHolderYieldLiquidUniverseSelectionModel(bool filterFineData)
            : base(filterFineData) { }

        public override IEnumerable<Symbol> SelectCoarse(QCAlgorithm qcAlgorithm, IEnumerable<CoarseFundamental> coarseFundamentals)
        {
            List<CoarseFundamental> filtered = new List<CoarseFundamental>();
            List<Symbol> symbols = new List<Symbol>();

            foreach (CoarseFundamental coarseFundamental in coarseFundamentals)
            {
                if (coarseFundamental.HasFundamentalData)
                {
                    filtered.Add(coarseFundamental);
                }

            }
            filtered = filtered.OrderByDescending(o => o.DollarVolume).ToList();
            return filtered.Select(o => o.Symbol).ToList();
        }

        override public IEnumerable<Symbol> SelectFine(QCAlgorithm qCAlgorithm, IEnumerable<FineFundamental> fine)
        {
            List<FineFundamental> filtered = new List<FineFundamental>();
            filtered = fine.OrderByDescending(o => o.ValuationRatios.TotalYield).ToList();
            if (filtered.Count < 10)
            {
                return filtered.Select(o => o.Symbol).ToList();
            }
            return filtered.GetRange(0, 9).Select(o => o.Symbol).ToList();

        }


    }

}

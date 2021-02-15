using QuantConnect.Algorithm.Framework.Selection;
using QuantConnect.Data.Fundamental;
using QuantConnect.Data.UniverseSelection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantConnect.Algorithm.CSharp.Caruso
{
    public partial class SmallCapGFCUniverseSelectionModel : FundamentalUniverseSelectionModel 
    {
        bool initialized = false;
        IEnumerable<FineFundamental> universe;

        public SmallCapGFCUniverseSelectionModel(bool filterFineData)
            : base(filterFineData) { }

        public override IEnumerable<Symbol> SelectCoarse(QCAlgorithm qcAlgorithm, IEnumerable<CoarseFundamental> coarseFundamentals)
        {
            List<CoarseFundamental> filtered = new List<CoarseFundamental>();
            List<Symbol> symbols = new List<Symbol>();

            foreach (CoarseFundamental coarseFundamental in coarseFundamentals)
            {   if (coarseFundamental.Price <= 0)
                {
                    continue;
                }
                if (coarseFundamental.Volume <= 0)
                {
                    continue;
                }
                if (!coarseFundamental.HasFundamentalData)
                {
                    continue;
                }
                filtered.Add(coarseFundamental);
                

            }
            filtered = filtered.OrderByDescending(o => o.DollarVolume).ToList();
            symbols = filtered.Select(o => o.Symbol).ToList();
            qcAlgorithm.Debug("Coarse: " + symbols.Count.ToString(CultureInfo.CreateSpecificCulture("en-GB")));
            // if (symbols.Count > 100)
            // {
            //    return symbols.Take(100);
            // }
            return symbols;

        }

        override public IEnumerable<Symbol> SelectFine(QCAlgorithm qCAlgorithm, IEnumerable<FineFundamental> fine)
        {
            if (initialized)
            {
                return this.universe.Select(o => o.Symbol);
            }
            List<FineFundamental> filtered = new List<FineFundamental>();
            foreach (FineFundamental fineFundamental in fine)
            {
                if (fineFundamental.AssetClassification.StyleBox == StyleBox.SmallCore)
                {
                    qCAlgorithm.Debug("Add SmallCore");
                    filtered.Add(fineFundamental);
                }
                if (fineFundamental.AssetClassification.StyleBox == StyleBox.SmallValue)
                {
                    qCAlgorithm.Debug("Add SmallValue");
                    filtered.Add(fineFundamental);
                }
                if (fineFundamental.AssetClassification.StyleBox == StyleBox.SmallGrowth)
                {
                    qCAlgorithm.Debug("Add SmallGrowth");
                    filtered.Add(fineFundamental);
                }

            }
            qCAlgorithm.Debug("Fine: " + filtered.Count.ToString(CultureInfo.CreateSpecificCulture("en-GB")));
            this.universe = filtered.Take(100);
            this.initialized = true;
            return this.universe.Select(o => o.Symbol);


        }
    }
}

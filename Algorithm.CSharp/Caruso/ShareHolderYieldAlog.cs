using QuantConnect.Algorithm.Framework.Execution;
using QuantConnect.Algorithm.Framework.Portfolio;
using QuantConnect.Algorithm.Framework.Risk;
using QuantConnect.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantConnect.Algorithm.CSharp
{
    public partial class ShareHolderYieldAlgo : QCAlgorithm
    {

        public override void Initialize()
        {
            SetStartDate(2017, 1, 1);
            SetCash(10000);

            UniverseSettings.Resolution = Resolution.Daily;
            SetUniverseSelection(new ShareHolderYieldLiquidUniverseSelectionModel(true));
            SetAlpha(new LongOnlyAlphaModel());
            SetRiskManagement(new NullRiskManagementModel());
            SetPortfolioConstruction(new EqualWeightingPortfolioConstructionModel());
            SetExecution(new ImmediateExecutionModel());
        }

    }
}

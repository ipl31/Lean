using QuantConnect.Algorithm.Framework.Execution;
using QuantConnect.Algorithm.Framework.Portfolio;
using QuantConnect.Algorithm.Framework.Risk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantConnect.Algorithm.CSharp.Caruso
{
    public partial class SmallCapGFC : QCAlgorithm
    {
        public override void Initialize()
        {
            SetStartDate(2009, 3, 1);
            SetEndDate(2011, 3, 1);
            SetCash(1000000);

            UniverseSettings.Resolution = Resolution.Daily;
            SetUniverseSelection(new SmallCapGFCUniverseSelectionModel(true));
            TimeSpan signalValidityPeriod = TimeSpan.FromDays(2000);
            SetAlpha(new SmallCapGFCAlphaModel(signalValidityPeriod));
            SetRiskManagement(new NullRiskManagementModel());
            SetPortfolioConstruction(new EqualWeightingPortfolioConstructionModel(signalValidityPeriod));
            SetExecution(new ImmediateExecutionModel());
        }

    }
}

﻿/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Deedle;
using Python.Runtime;
using QuantConnect.Orders;
using QuantConnect.Packets;

namespace QuantConnect.Report.ReportElements
{
    internal sealed class ExposureReportElement : ChartReportElement
    {
        private LiveResult _live;
        private BacktestResult _backtest;

        /// <summary>
        /// Create a new plot of the exposure
        /// </summary>
        /// <param name="name">Name of the widget</param>
        /// <param name="key">Location of injection</param>
        /// <param name="backtest">Backtest result object</param>
        /// <param name="live">Live result object</param>
        public ExposureReportElement(string name, string key, BacktestResult backtest, LiveResult live)
        {
            _live = live;
            _backtest = backtest;
            Name = name;
            Key = key;
        }

        /// <summary>
        /// Generate the exposure plot using the python libraries.
        /// </summary>
        public override string Render()
        {
            var backtestPoints = Calculations.EquityPoints(_backtest);
            var livePoints = Calculations.EquityPoints(_live);
            var liveOrders = _live == null ? new List<Order>() : _live.Orders.Values.ToList();

            var longBacktestFrame = new Series<DateTime, double>(backtestPoints.Keys, backtestPoints.Values).Exposure(_backtest.Orders.Values.ToList(), OrderDirection.Buy);
            var shortBacktestFrame = new Series<DateTime, double>(backtestPoints.Keys, backtestPoints.Values).Exposure(_backtest.Orders.Values.ToList(), OrderDirection.Sell);
            var longLiveFrame = new Series<DateTime, double>(livePoints.Keys, livePoints.Values).Exposure(liveOrders, OrderDirection.Buy);
            var shortLiveFrame = new Series<DateTime, double>(livePoints.Keys, livePoints.Values).Exposure(liveOrders, OrderDirection.Sell);

            longBacktestFrame.Print();
            shortBacktestFrame.Print();

            var backtestFrame = longBacktestFrame.Join(shortBacktestFrame * -1)
                .FillMissing(Direction.Forward)
                .FillMissing(0.0);

            var liveFrame = longLiveFrame.Join(shortLiveFrame * -1)
                .FillMissing(Direction.Forward)
                .FillMissing(0.0);

            longBacktestFrame = Frame.CreateEmpty<DateTime, Tuple<SecurityType, OrderDirection>>();
            shortBacktestFrame = Frame.CreateEmpty<DateTime, Tuple<SecurityType, OrderDirection>>();
            longLiveFrame = Frame.CreateEmpty<DateTime, Tuple<SecurityType, OrderDirection>>();
            shortLiveFrame = Frame.CreateEmpty<DateTime, Tuple<SecurityType, OrderDirection>>();

            foreach (var key in backtestFrame.ColumnKeys)
            {
                longBacktestFrame[key] = backtestFrame[key].SelectValues(x => x < 0 ? 0 : x);
                shortBacktestFrame[key] = backtestFrame[key].SelectValues(x => x > 0 ? 0 : x);
            }

            foreach (var key in liveFrame.ColumnKeys)
            {
                longLiveFrame[key] = liveFrame[key].SelectValues(x => x < 0 ? 0 : x);
                shortLiveFrame[key] = liveFrame[key].SelectValues(x => x > 0 ? 0 : x);
            }

            var base64 = "";
            using (Py.GIL())
            {
                var time = backtestFrame.RowKeys.ToList().ToPython();
                var longSecurities = longBacktestFrame.ColumnKeys.Select(x => x.Item1.ToStringInvariant()).ToList().ToPython();
                var shortSecurities = shortBacktestFrame.ColumnKeys.Select(x => x.Item1.ToStringInvariant()).ToList().ToPython();
                var longData = longBacktestFrame.ColumnKeys.Select(x => longBacktestFrame[x].Values.ToList().ToPython()).ToPython();
                var shortData = shortBacktestFrame.ColumnKeys.Select(x => shortBacktestFrame[x].Values.ToList().ToPython()).ToPython();
                var liveTime = liveFrame.RowKeys.ToList().ToPython();
                var liveLongSecurities = longLiveFrame.ColumnKeys.Select(x => x.Item1.ToStringInvariant()).ToList().ToPython();
                var liveShortSecurities = shortLiveFrame.ColumnKeys.Select(x => x.Item1.ToStringInvariant()).ToList().ToPython();
                var liveLongData = longLiveFrame.ColumnKeys.Select(x => longLiveFrame[x].Values.ToList().ToPython()).ToPython();
                var liveShortData = shortLiveFrame.ColumnKeys.Select(x => shortLiveFrame[x].Values.ToList().ToPython()).ToPython();

                base64 = Charting.GetExposure(
                    time,
                    longSecurities,
                    shortSecurities,
                    longData,
                    shortData,
                    liveTime,
                    liveLongSecurities,
                    liveShortSecurities,
                    liveLongData,
                    liveShortData
                );
            }

            return base64;
        }
    }
}
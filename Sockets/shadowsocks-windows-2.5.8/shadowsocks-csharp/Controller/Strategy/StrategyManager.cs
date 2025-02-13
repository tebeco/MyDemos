﻿using Shadowsocks.Controller;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shadowsocks.Controller.Strategy
{
    class StrategyManager
    {
        private List<IStrategy> _strategies;
        public StrategyManager(ShadowsocksController controller)
        {
            _strategies = new List<IStrategy>();

            _strategies.Add(new BalancingStrategy(controller));
            _strategies.Add(new HighAvailabilityStrategy(controller));
            _strategies.Add(new SimplyChooseByStatisticsStrategy(controller));

            // TODO: load DLL plugins
        }

        public IList<IStrategy> GetStrategies()
        {
            return _strategies;
        }
    }
}

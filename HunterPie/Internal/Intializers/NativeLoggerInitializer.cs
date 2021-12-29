﻿using HunterPie.Core.Domain.Features;
using HunterPie.Core.Logger;
using HunterPie.Domain.Constants;
using HunterPie.Domain.Interfaces;
using HunterPie.Internal.Logger;

namespace HunterPie.Internal.Intializers
{
    internal class NativeLoggerInitializer : IInitializer
    {
        public void Init()
        {
            if (FeatureFlagManager.IsEnabled(FeatureFlags.FEATURE_NATIVE_LOGGER))
            {
                ILogger logger = new NativeLogger();
                Log.Add(logger);

                Log.Info("Hello world! HunterPie stdout has been initialized!");
            }
        }
    }
}
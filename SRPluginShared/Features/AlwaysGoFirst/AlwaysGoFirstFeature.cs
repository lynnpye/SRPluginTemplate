using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace SRPlugin.Features.AlwaysGoFirst
{
    public class AlwaysGoFirstFeature : FeatureImpl
    {
        private static ConfigItem<bool> CIAlwaysGoFirst;

        public AlwaysGoFirstFeature()
            : base(
                  nameof(AlwaysGoFirst),
                  new List<ConfigItemBase>()
                  {
                      (CIAlwaysGoFirst = new ConfigItem<bool>(PLUGIN_FEATURES_SECTION, nameof(AlwaysGoFirst), true, "your team will always go first in combat")),
                  },
                  new List<PatchRecord>()
                  {
                      PatchRecord.Postfix(
                          typeof(TurnDirector).GetMethod(nameof(TurnDirector.TeamRating)),
                          typeof(TurnDirectorPatch).GetMethod(nameof(TurnDirectorPatch.TeamRatingPostfix))
                          ),
                  })
        {

        }

        public static bool AlwaysGoFirst { get => CIAlwaysGoFirst.GetValue(); set => CIAlwaysGoFirst.SetValue( value ); }

        [HarmonyPatch(typeof(TurnDirector))]
        internal class TurnDirectorPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(TurnDirector.TeamRating))]
            public static void TeamRatingPostfix(ref int __result, Team t)
            {
                List<Player> units = AccessTools.Field(typeof(Team), "units").GetValue(t) as List<Player>;

                if (units == null) return;

                foreach (var u in units)
                {
                    if (u.actorUID == SceneSingletonBehavior<TurnDirector>.Instance.PlayerZero.actorUID)
                    {
                        __result = int.MaxValue;
                        return;
                    }
                }
            }
        }
    }
}

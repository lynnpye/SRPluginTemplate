using System.Collections.Generic;
using HarmonyLib;
using isogame;
using Localize;
using UnityEngine;

namespace SRPlugin.Features.CyberwareAffinityEssenceBonusOverride
{
    public class CyberwareAffinityEssenceBonusOverrideFeature : FeatureImpl
    {
#if !SRHK
        public CyberwareAffinityEssenceBonusOverrideFeature()
            : base(null, new List<ConfigItemBase>() { }, new List<PatchRecord>() { }) { }
#else
        private static ConfigItem<bool> CICyberwareAffinityEssenceBonusesOverride;
        private static ConfigItem<string> CICyberwareAffinityEssenceBonuses;

        public CyberwareAffinityEssenceBonusOverrideFeature()
            : base(
                nameof(CyberwareAffinityEssenceBonusesOverride),
                new List<ConfigItemBase>()
                {
                    (
                        CICyberwareAffinityEssenceBonusesOverride = new ConfigItem<bool>(
                            PLUGIN_FEATURES_SECTION,
                            nameof(CyberwareAffinityEssenceBonusesOverride),
                            true,
                            "enable the ability to override essence bonuses, the default value is the game default"
                        )
                    ),
                    (
                        CICyberwareAffinityEssenceBonuses = new ConfigItem<string>(
                            nameof(CyberwareAffinityEssenceBonuses),
                            CADefaultString,
                            CAEssenceBonusHelp
                        )
                    ),
                },
                new List<PatchRecord>(
                    PatchRecord.RecordPatches(
                        AccessTools.Method(
                            typeof(StatsUtilPatch),
                            nameof(StatsUtilPatch.GetSkillMaxPostfix)
                        ),
                        AccessTools.Method(
                            typeof(StatsUtilPatch),
                            nameof(StatsUtilPatch.GetSkillCapPostfix)
                        ),
                        AccessTools.Method(
                            typeof(ActorGetDerivedEssencePatch),
                            nameof(ActorGetDerivedEssencePatch.GetDerivedEssencePostfix)
                        ),
                        AccessTools.Method(
                            typeof(KarmaEntry2Patch),
                            nameof(KarmaEntry2Patch.InitializePostfix)
                        )
                    )
                )
                {
                    }
            ) { }

        public override void PostApplyPatches()
        {
            Bonuses = GetCyberwareAffinityBonuses();
        }

        private static string CADefaultString = "0 0 1 0 0 1 0";
        private static string CASkill = "Cyberware Affinity";
        private static string CAEssenceBonusHelp =
            $@"this should be a string containing a set of numbers, with the order of numbers
matching increasing points in {CASkill}, so the first number is the bonus essence points
from having 1 point in {CASkill}, the second is the bonus for 2 points, and so on
the number listed is the bonus additional essence for gaining that level. all bonuses are cumulative.
the default setting should match the base SRHK game (i.e. +1 additional essence at 3 and 6 ranks in Cyberware Affinity";

        public static bool CyberwareAffinityEssenceBonusesOverride
        {
            get => CICyberwareAffinityEssenceBonusesOverride.GetValue();
            set => CICyberwareAffinityEssenceBonusesOverride.SetValue(value);
        }
        public static string CyberwareAffinityEssenceBonuses
        {
            get =>
                CyberwareAffinityEssenceBonusesOverride
                    ? CICyberwareAffinityEssenceBonuses.GetValue()
                    : CADefaultString;
            set => CICyberwareAffinityEssenceBonuses.SetValue(value);
        }

        private static int[] GetCyberwareAffinityBonuses()
        {
            int CAmax = StatsUtil.GetSkillMax(Skill.Skill_CyberwareAffinity);

            // we're going to make a list that is one larger than the actual
            // number of CA ranks, to handle rank 0, no investment

            int CAranks = CAmax + 1; // added for rank 0
            int[] bonuses = new int[CAranks];

            string bstring = CyberwareAffinityEssenceBonuses;
            if (bstring == null || bstring.Trim().Length == 0)
            {
                bstring = CADefaultString;
            }

            // we are going to prepend a 0 for rank 0
            bstring = "0 " + bstring;

            string[] updatedList = new string[CAmax];
            string[] parts = bstring.Split(' ');
            for (int i = 0; i < CAranks; i++)
            {
                if (parts.Length <= i)
                {
                    bonuses[i] = 0;
                    continue;
                }
                string part = parts[i];
                int bonus = 0;
                if (int.TryParse(part, out bonus) && bonus >= 0)
                {
                    bonuses[i] = bonus;
                }
                else
                {
                    bonuses[i] = 0;
                }

                if (i > 0)
                {
                    // the updated list won't include rank 0
                    updatedList[i - 1] = bonuses[i].ToString();
                }
            }

            var newVal = string.Join(" ", updatedList);
            if (!string.Equals(newVal, CyberwareAffinityEssenceBonuses))
            {
                CyberwareAffinityEssenceBonuses = newVal;
            }

            return bonuses;
        }

        private static int[] _bonuses;
        public static int[] Bonuses
        {
            get
            {
                if (_bonuses == null)
                {
                    _bonuses = GetCyberwareAffinityBonuses();
                }
                return _bonuses;
            }
            private set { _bonuses = value; }
        }

        public static float GetCyberwareAffinityBonusEssence(int cyberwareAffinity)
        {
            float bonus = 0f;

            for (int i = 0; i < Bonuses.Length && i < cyberwareAffinity; i++)
            {
                bonus += Bonuses[i];
            }

            return bonus;
        }

        // It's unfortunate that CA has a weirdly hard coded max value
        [HarmonyPatch(typeof(StatsUtil))]
        internal class StatsUtilPatch
        {
            // honestly, I think this is more of a standalone fix of sorts, as I think it more appropriately represents the expected return result
            [HarmonyPostfix]
            [HarmonyPatch(nameof(StatsUtil.GetSkillMax))]
            public static void GetSkillMaxPostfix(ref int __result, Skill entry)
            {
                if (!CyberwareAffinityEssenceBonusesOverride)
                    return;

                if (entry == Skill.Skill_CyberwareAffinity)
                {
                    __result = Mathf.Min(7, __result);
                }
            }

            // honestly, I think this is more of a standalone fix of sorts, as I think it more appropriately represents the expected return result
            [HarmonyPostfix]
            [HarmonyPatch(nameof(StatsUtil.GetSkillCap))]
            public static void GetSkillCapPostfix(ref int __result, Skill entry)
            {
                if (!CyberwareAffinityEssenceBonusesOverride)
                    return;

                if (entry == Skill.Skill_CyberwareAffinity)
                {
                    __result = Mathf.Min(7, __result);
                }
            }
        }

        [HarmonyPatch(typeof(Actor))]
        internal class ActorGetDerivedEssencePatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(Actor.GetDerivedEssence))]
            public static void GetDerivedEssencePostfix(ref float __result, Actor __instance)
            {
                if (!CyberwareAffinityEssenceBonusesOverride)
                    return;

                int cyberwareAffinity = StatsUtil.GetSkill(
                    __instance,
                    Skill.Skill_CyberwareAffinity
                );
                float derivedEssenceAdjustment = 0f;

                // remove anything added via base game cyberware affinity bonuses
                if (cyberwareAffinity >= 6)
                {
                    derivedEssenceAdjustment = -2f;
                }
                else if (cyberwareAffinity >= 3)
                {
                    derivedEssenceAdjustment = -1f;
                }

                derivedEssenceAdjustment += GetCyberwareAffinityBonusEssence(cyberwareAffinity);

                __result = __result + derivedEssenceAdjustment;
            }
        }

        [HarmonyPatch(typeof(KarmaEntry2))]
        internal class KarmaEntry2Patch
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(KarmaEntry2.Initialize))]
            public static void InitializePostfix(
                KarmaEntry2 __instance,
                KarmaScreen2.KarmaEntryData ___data,
                BetterList<KarmaBlock> ___blockList
            )
            {
                if (!CyberwareAffinityEssenceBonusesOverride)
                    return;

                // no, we are not modifying anything, we are peeking tyvm
#pragma warning disable Harmony003 // Harmony non-ref patch parameters modified
                Skill dataSkill = ___data.skill;
#pragma warning restore Harmony003 // Harmony non-ref patch parameters modified

                if (__instance == null || Skill.Skill_CyberwareAffinity != dataSkill)
                    return;

                int[] bonii = Bonuses;

                int skillLevel = 0;

                foreach (KarmaBlock block in ___blockList)
                {
                    skillLevel++;

                    // remove the previous references to essence addition
                    if (skillLevel == 3)
                    {
                        block.SetToInfo(
                            Strings.T("Unlocks \"Eviscerate\" for Hand Razors and Spurs")
                        );
                    }
                    if (skillLevel == 6)
                    {
                        block.SetToInfo(
                            Strings.T(
                                "Cyberware can allow you to exceed racial maximums for Attributes."
                            )
                        );
                    }

                    if (bonii.Length > skillLevel)
                    {
                        int bonus = bonii[skillLevel];

                        block.SetToInfo(
                            Strings.T($"+{bonus} Additional Essence. {block.infoText}")
                        );
                    }
                }
            }
        }
#endif
    }
}

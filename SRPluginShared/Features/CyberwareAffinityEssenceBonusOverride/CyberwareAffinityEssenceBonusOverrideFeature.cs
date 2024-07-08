using HarmonyLib;
using isogame;
using System.Collections.Generic;
using UnityEngine;

namespace SRPlugin.Features.CyberwareAffinityEssenceBonusOverride
{
    public class CyberwareAffinityEssenceBonusOverrideFeature : FeatureImpl
    {
#if SRHK
        private static ConfigItem<string> CICyberwareAffinityEssenceBonuses;
#endif

        public CyberwareAffinityEssenceBonusOverrideFeature()
            : base(new List<ConfigItemBase>()
            {
#if SRHK
                (CICyberwareAffinityEssenceBonuses = new ConfigItem<string>(FEATURES_SECTION, nameof(CyberwareAffinityEssenceBonuses), CADefaultString, CAEssenceBonusHelp)),
#endif
            }, new List<PatchRecord>()
            {
#if SRHK
                PatchRecord.Postfix(
                    typeof(StatsUtil).GetMethod(nameof(StatsUtil.GetSkillCap)),
                    typeof(StatsUtilPatch).GetMethod(nameof(StatsUtilPatch.GetSkillCapPostfix))
                    ),
                PatchRecord.Postfix(
                    typeof(StatsUtil).GetMethod(nameof(StatsUtil.GetSkillMax)),
                    typeof(StatsUtilPatch).GetMethod(nameof(StatsUtilPatch.GetSkillMaxPostfix))
                    ),
#endif
            })
        {

        }

        public override void PostApplyPatches()
        {
#if SRHK
            Bonuses = GetCyberwareAffinityBonuses();
#endif
        }

#if SRHK
        private static string CADefaultString = "0 0 1 1 1 2";
        private static string CASkill = "Cyberware Affinity";
        private static string CAEssenceBonusHelp =
$@"this should be a string containing a set of numbers, with the order of numbers
matching increasing points in {CASkill}, so the first number is the bonus essence points
from having 1 point in {CASkill}, the second is the bonus for 2 points, and so on
whatever is the last number is the max bonus gained
a number should never be lower than the number to its left; if it is, it will be treated like it is equal
the default setting should match the base SRHK game";
#endif
#if SRHK
        public static string CyberwareAffinityEssenceBonuses { get => CICyberwareAffinityEssenceBonuses.GetValue(); set => CICyberwareAffinityEssenceBonuses.SetValue(value); }
#endif

#if SRHK

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
            int lastBonus = 0;
            for (int i = 0; i < CAranks; i++)
            {
                if (parts.Length <= i)
                {
                    bonuses[i] = lastBonus;
                    continue;
                }
                string part = parts[i];
                int bonus = 0;
                if (int.TryParse(part, out bonus) && bonus >= lastBonus)
                {
                    lastBonus = bonus;
                }
                bonuses[i] = lastBonus;
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

            private set
            {
                _bonuses = value;
            }
        }

        public static float GetCyberwareAffinityBonusEssence(int cyberwareAffinity)
        {
            if (Bonuses.Length >= cyberwareAffinity)
            {
                return Bonuses[cyberwareAffinity];
            }

            return 0f;
        }
#endif

#if SRHK
        // It's unfortunate that CA has a weirdly hard coded max value
        [HarmonyPatch(typeof(StatsUtil))]
        internal class StatsUtilPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch(nameof(StatsUtil.GetSkillMax))]
            public static void GetSkillMaxPostfix(ref int __result, Skill entry)
            {
                if (entry == Skill.Skill_CyberwareAffinity)
                {
                    __result = Mathf.Min(7, __result);
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(nameof(StatsUtil.GetSkillCap))]
            public static void GetSkillCapPostfix(ref int __result, Skill entry)
            {
                if (entry == Skill.Skill_CyberwareAffinity)
                {
                    __result = Mathf.Min(7, __result);
                }
            }
        }
#endif
    }
}

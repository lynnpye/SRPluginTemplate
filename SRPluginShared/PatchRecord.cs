using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace SRPlugin
{
    public enum PatchType
    {
        None,
        UnpatchOnly,
        Prefix,
        Postfix,
        Reverse
    }

    public class PatchRecord
    {
        private MethodBase original;
        private MethodInfo patch;
        private PatchType patchType;
        private int priority;

        public PatchRecord(MethodBase original, MethodInfo patch, PatchType patchType, int priority)
        {
            this.original = original;
            this.patch = patch;
            this.patchType = patchType;
            this.priority = priority;
        }

        public static MethodBase GetOriginalFromHarmonyPatch(
            HarmonyPatch classPatch,
            HarmonyPatch methodPatch
        )
        {
            if (classPatch == null || methodPatch == null)
                return null;

            if (methodPatch.info.methodType == MethodType.Getter)
            {
                return AccessTools.PropertyGetter(
                    classPatch.info.declaringType,
                    methodPatch.info.methodName
                );
            }
            else if (methodPatch.info.methodType == MethodType.Setter)
            {
                return AccessTools.PropertySetter(
                    classPatch.info.declaringType,
                    methodPatch.info.methodName
                );
            }
            else
            {
                return AccessTools.Method(
                    classPatch.info.declaringType,
                    methodPatch.info.methodName,
                    methodPatch.info.argumentTypes
                );
            }
        }

        public static List<PatchRecord> MakePatchRecords(
            MethodInfo patch,
            PatchType patchType,
            int priority,
            HarmonyPatch classHarmonyPatch,
            HarmonyPatch[] harmonyPatches
        )
        {
            List<PatchRecord> records = new List<PatchRecord>();

            foreach (var harmonyPatch in harmonyPatches)
            {
                records.Add(
                    new PatchRecord(
                        original: GetOriginalFromHarmonyPatch(classHarmonyPatch, harmonyPatch),
                        patch: patch,
                        patchType: patchType,
                        priority: priority
                    )
                );
            }

            return records;
        }

        public static List<PatchRecord> RecordPatches(params MethodInfo[] patches)
        {
            try
            {
                if (patches == null || patches.Length == 0)
                    return [];

                List<PatchRecord> records = new List<PatchRecord>();

                foreach (var patch in patches)
                {
                    var harmonyPatches =
                        patch.GetCustomAttributes(typeof(HarmonyPatch), false) as HarmonyPatch[];
                    if (harmonyPatches == null || harmonyPatches.Length == 0)
                    {
                        SRPlugin.Logger.LogInfo(
                            $"Method {patch.Name} is not a Harmony patch method."
                        );
                        return [];
                    }

                    var classHarmonyPatches =
                        patch.DeclaringType.GetCustomAttributes(typeof(HarmonyPatch), false)
                        as HarmonyPatch[];
                    if (classHarmonyPatches == null || classHarmonyPatches.Length == 0)
                    {
                        SRPlugin.Logger.LogInfo(
                            $"Class {patch.DeclaringType.Name} is not a Harmony patch class."
                        );
                        return [];
                    }

                    int priority = Priority.Normal;
                    var harmonyPriority =
                        patch.GetCustomAttributes(typeof(HarmonyPriority), false)
                        as HarmonyPriority[];
                    if (harmonyPriority != null && harmonyPriority.Length > 0)
                    {
                        priority = harmonyPriority[0].info.priority;
                    }

                    PatchType patchType = PatchType.None;

                    var harmonyPrefix =
                        patch.GetCustomAttributes(typeof(HarmonyPrefix), false) as HarmonyPrefix[];
                    if (harmonyPrefix != null && harmonyPrefix.Length > 0)
                    {
                        patchType = PatchType.Prefix;

                        records.AddRange(
                            MakePatchRecords(
                                patch,
                                patchType,
                                priority,
                                classHarmonyPatches[0],
                                harmonyPatches
                            )
                        );
                    }

                    if (patchType == PatchType.None)
                    {
                        var harmonyPostfix =
                            patch.GetCustomAttributes(typeof(HarmonyPostfix), false)
                            as HarmonyPostfix[];
                        if (harmonyPostfix != null && harmonyPostfix.Length > 0)
                        {
                            patchType = PatchType.Postfix;

                            records.AddRange(
                                MakePatchRecords(
                                    patch,
                                    patchType,
                                    priority,
                                    classHarmonyPatches[0],
                                    harmonyPatches
                                )
                            );
                        }
                    }

                    if (patchType == PatchType.None)
                    {
                        var harmonyReverse =
                            patch.GetCustomAttributes(typeof(HarmonyReversePatch), false)
                            as HarmonyReversePatch[];
                        if (harmonyReverse != null && harmonyReverse.Length > 0)
                        {
                            patchType = PatchType.Reverse;

                            records.AddRange(
                                MakePatchRecords(
                                    patch,
                                    patchType,
                                    priority,
                                    classHarmonyPatches[0],
                                    harmonyPatches
                                )
                            );
                        }
                    }
                }

                return records;
            }
            catch (Exception e)
            {
                SRPlugin.Squawk($"Exception while recording patches.");
                SRPlugin.Squawk(e.ToString());
                return [];
            }
        }

        public void Patch()
        {
            HarmonyMethod harmonyMethod = null;
            try
            {
                harmonyMethod = new HarmonyMethod(method: patch, priority: priority);

                switch (patchType)
                {
                    case PatchType.Prefix:
                        SRPlugin.Harmony.Patch(original: original, prefix: harmonyMethod);
                        break;
                    case PatchType.Postfix:
                        SRPlugin.Harmony.Patch(original: original, postfix: harmonyMethod);
                        break;
                    case PatchType.Reverse:
                        {
                            var reversePatcher = SRPlugin.Harmony.CreateReversePatcher(
                                original,
                                harmonyMethod
                            );
                            reversePatcher.Patch();
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                SRPlugin.Squawk(
                    $"Exception while patching. original:{original}:  patch:{harmonyMethod}:  patchType:{patchType}: priority:{priority}:"
                );
                SRPlugin.Squawk(e.ToString());
            }
        }

        public void Unpatch()
        {
            SRPlugin.Harmony.Unpatch(original, patch);
        }
    }
}

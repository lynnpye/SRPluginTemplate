using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;

namespace SRPlugin.Features.LoadLocalSaves
{
    public class LoadLocalSavesFeature : FeatureImpl
    {
        private static ConfigItem<bool> CILoadLocalSaves;
        private static ConfigItem<string> CIAlternateLocalSaveFolder;

        public LoadLocalSavesFeature()
            : base(
                nameof(LoadLocalSaves),
                [
                    (
                        CILoadLocalSaves = new ConfigItem<bool>(
                            PLUGIN_FEATURES_SECTION,
                            nameof(LoadLocalSaves),
                            true,
                            $@"Enable loading local saves even when still connected to Steam; game default location is '{FileLoader.UserDataPath() + "/Saves/"}'
This will only impact Steam versions since the cloud save system is Steam-only.
This will now allow 'creating' a local save, per se, but since you can always
obtain a locally cached copy of your Steam saves, you still have access.
The SaveManager functionality essentially disables the local versions
of save game handling when Steam is available. This makes the local areas
readable.

You can now drop a save game into a folder, install this plug-in, and load it up.
You can share save games easily, or use it for testing. In fact, thanks goes to
Nexusmods user spektukal for reporting the bug and providing a save game that
made realize I needed a way to load them in Steam.
"
                        )
                    ),
                    (
                        CIAlternateLocalSaveFolder = new ConfigItem<string>(
                            nameof(AlternateLocalSaveFolder),
                            "",
                            $"alternate folder to load local saves from; game default location is '{FileLoader.UserDataPath() + "/Saves/"}'"
                        )
                    )
                ],
                new List<PatchRecord>(
                    PatchRecord.RecordPatches(
                        AccessTools.Method(
                            typeof(SaveManagerPatch),
                            nameof(SaveManagerPatch.SaveDirectoryPropertyPatch)
                        ),
                        AccessTools.Method(
                            typeof(SaveManagerPatch),
                            nameof(SaveManagerPatch.FileReadPatch)
                        ),
                        AccessTools.Method(
                            typeof(SaveManagerPatch),
                            nameof(SaveManagerPatch.FileExistsPatch)
                        ),
                        AccessTools.Method(
                            typeof(SaveManagerPatch),
                            nameof(SaveManagerPatch.AllSaveFilesPatch)
                        )
                    )
                )
                {
                    }
            ) { }

        public static bool LoadLocalSaves
        {
            get => CILoadLocalSaves.GetValue();
            set => CILoadLocalSaves.SetValue(value);
        }
        public static string AlternateLocalSaveFolder
        {
            get => CIAlternateLocalSaveFolder.GetValue();
            set => CIAlternateLocalSaveFolder.SetValue(value);
        }

        public override void PreApplyPatches()
        {
            // validate the directory if specified
            if (
                !string.IsNullOrEmpty(AlternateLocalSaveFolder)
                && !Directory.Exists(AlternateLocalSaveFolder)
            )
            {
                SRPlugin.Squawk(
                    $"AlternateLocalSaveFolder does not exist:{AlternateLocalSaveFolder}:"
                );
                AlternateLocalSaveFolder = CIAlternateLocalSaveFolder.defaultValue;
            }
        }

        /*
         * Not really a reverse patch, but sort of behaves like it. Doing it
         * without reverse patch in order to make sure we get the patched
         * version above.
         */
        private static string SaveDirectoryReversePatch()
        {
            return AccessTools
                .PropertyGetter(typeof(SaveManager), "SaveDirectory")
                .Invoke(null, null)
                .ToString();
        }

        [HarmonyPatch(typeof(SaveManager))]
        internal class SaveManagerPatch
        {
            // for AlternateLocalSaveFolder
            [HarmonyPostfix]
            [HarmonyPatch("SaveDirectory", MethodType.Getter)]
            public static void SaveDirectoryPropertyPatch(ref string __result)
            {
                if (!LoadLocalSaves || string.IsNullOrEmpty(AlternateLocalSaveFolder))
                {
                    return;
                }

                __result = AlternateLocalSaveFolder;
            }

            // for LoadLocalSaves
            [HarmonyPrefix]
            [HarmonyPatch("FileRead")]
            public static bool FileReadPatch(ref byte[] __result, string filename)
            {
                if (!Steam.Steamworks.IsAvailable || !LoadLocalSaves)
                {
                    return true;
                }

                var saveFolder = SaveDirectoryReversePatch();
                if (string.IsNullOrEmpty(saveFolder))
                {
                    // if saveFolder is empty we can't help you anyway
                    return true;
                }

                // if you got here, you're connected to steam and the file couldn't be found
                // let's go
                var fullSaveFileName = Path.GetFullPath(Path.Combine(saveFolder, filename));
                var doesFileExist = File.Exists(fullSaveFileName);
                if (doesFileExist)
                {
                    __result = File.ReadAllBytes(fullSaveFileName);
                    return false;
                }
                else
                {
                    SRPlugin.Squawk(
                        $"requested file but File.Exists fails, bailing out: {fullSaveFileName}"
                    );
                    return true;
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch("FileExists")]
            public static void FileExistsPatch(ref bool __result, string filename)
            {
                if (__result || !Steam.Steamworks.IsAvailable || !LoadLocalSaves)
                {
                    // if the file was already found it exists, regardless of source
                    // we can worry about the source later
                    return;
                }

                var saveFolder = SaveDirectoryReversePatch();
                if (string.IsNullOrEmpty(saveFolder))
                {
                    // if saveFolder is empty we can't help you anyway
                    return;
                }

                // if you got here, you're connected to steam and the file couldn't be found
                // let's go
                var fullSaveFileName = Path.Combine(saveFolder, filename);
                var doesFileExist = File.Exists(fullSaveFileName);
                if (doesFileExist)
                {
                    __result = true;
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(nameof(SaveManager.AllSaveFiles))]
            public static void AllSaveFilesPatch(ref string[] __result)
            {
                // if not on steam, you already pulled from local
                if (!LoadLocalSaves || !Steam.Steamworks.IsAvailable)
                {
                    return;
                }

                var saveFolder = SaveDirectoryReversePatch();

                if (string.IsNullOrEmpty(saveFolder))
                {
                    return;
                }

                DirectoryInfo saveDI = new DirectoryInfo(saveFolder);
                if (!saveDI.Exists)
                {
                    return;
                }

                FileInfo[] files = saveDI.GetFiles("*.sav");
                string[] names = new string[files.Length];
                for (int i = 0; i < files.Length; i++)
                {
                    names[i] = files[i].Name;
                }

                if (names.Length < 1)
                {
                    return;
                }

                if (__result == null || __result.Length < 1)
                {
                    __result = names;
                    return;
                }

                var merged = names.Concat(__result).ToArray();
                __result = merged;
            }
        }
    }
}

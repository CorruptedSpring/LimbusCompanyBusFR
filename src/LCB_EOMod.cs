﻿using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using Il2CppSystem.Runtime.Remoting.Messaging;
using LimbusCompanyFR.EO;
using StorySystem;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace LimbusCompanyFR
{

    [BepInPlugin(GUID, NAME, VERSION)]
    public class LCB_EOMod : BasePlugin
    {
        public static ConfigFile EO_Settings;
        public static string ModPath;
        public static string GamePath;
        public const string GUID = "Com.EdenOffice.LocalizeLimbusCompanyFR";
        public const string NAME = "LimbusCompanyFR";
        public const string VERSION = "0.6.0";
        public const string VERSION_STATE = "";
        public const string AUTHOR = "Bright (Modified by Knightey)";
        public const string EOLink = "https://github.com/Eden-Office/LimbusCompanyBusFR";
        public static Action<string, Action> LogFatalError { get; set; }
        public static Action<string> LogInfo { get; set; }
        public static Action<string> LogError { get; set; }
        public static Action<string> LogWarning { get; set; }
        public static void OpenEOURL() { Application.OpenURL(EOLink); }
        public static void OpenGamePath() { Application.OpenURL(GamePath); }
        public override void Load()
        {
            EO_Settings = Config;
            LogInfo = (string log) => { Log.LogInfo(log); Debug.Log(log); };
            LogError = (string log) => { Log.LogError(log); Debug.LogError(log); };
            LogWarning = (string log) => { Log.LogWarning(log); Debug.LogWarning(log); };
            LogFatalError = (string log, Action action) => { EO_Manager.FatalErrorlog += log + "\n"; LogError(log); EO_Manager.FatalErrorAction = action; EO_Manager.CheckModActions(); };
            GamePath = new DirectoryInfo(Application.dataPath).Parent.FullName;
            var matchingFiles = Directory.EnumerateFiles(GamePath + "\\BepInEx\\plugins", "LimbusCompanyFR_BIE.dll", SearchOption.AllDirectories);
            foreach (var filePath in matchingFiles)
            {
                ModPath = Path.GetDirectoryName(filePath);
            }
            EO_UpdateChecker.StartAutoUpdate();
            try
            {
                HarmonyLib.Harmony harmony = new(NAME);
                if (EO_French_Setting.IsUseFrench.Value)
                {
                    EO_Manager.InitLocalizes(new DirectoryInfo(ModPath + "/Localize/FR"));
                    harmony.PatchAll(typeof(LCB_French_Font));
                    harmony.PatchAll(typeof(EO_ReadmeManager));
                    harmony.PatchAll(typeof(EO_LoadingManager));
                    harmony.PatchAll(typeof(EO_TemporaryTextures));
                    harmony.PatchAll(typeof(EO_TextUI));
                    harmony.PatchAll(typeof(EO_SpriteUI));
                    harmony.PatchAll(typeof(EO_StoryUI));
                    harmony.PatchAll(typeof(EO_CreditsUI));
                    harmony.PatchAll(typeof(EO_EventUI));
                    harmony.PatchAll(typeof(EO_SeasonUI));
                }
                harmony.PatchAll(typeof(EO_Manager));
                harmony.PatchAll(typeof(EO_French_Setting));
                if (!LCB_French_Font.AddFrenchFont(ModPath + "/tmpfrenchfonts"))
                    LogFatalError("Vous avez oublié d'installer le mod de mise à jour de police d'écriture. Veuillez relire le README sur Github.", OpenEOURL);
                LogInfo("-------------------------\n");
                LogInfo("Startup" + DateTime.Now);
                //LogInfo("EventEnd" + new DateTime(2024, 9, 12, 2, 59, 0).ToLocalTime());
            }
            catch (Exception e)
            {
                LogFatalError("Le mod a rencontré une erreur inconnue ! S'il vous plaît, contactez nous à l'aide des urls du log sur Github.", () => { CopyLog(); OpenGamePath(); OpenEOURL(); });
                LogError(e.ToString());
            }
        }
        public static void CopyLog()
        {
            File.Copy(GamePath + "/BepInEx/LogOutput.log", GamePath + "/Latest.log", true);
            File.Copy(Application.consoleLogPath, GamePath + "/Player.log", true);
        }
    }
}

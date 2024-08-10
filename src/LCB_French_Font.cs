﻿using Addressable;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using SimpleJSON;
using StorySystem;
using System;
using System.IO;
using TMPro;
using UnityEngine;
using UtilityUI;
using static Il2CppSystem.DateTimeParse;

namespace LimbusCompanyFR
{
    public static class LCB_French_Font
    {
        public static List<TMP_FontAsset> tmpfrenchfonts = new();
        public static List<string> tmpfrenchfontsnames = new();
        public static List<Material> tmpfrenchmats = new();
        public static List<string> tmpfrenchmatsnames = new();
        #region Шрифты
        public static bool AddFrenchFont(string path)
        {
            if (File.Exists(path))
            {
                bool result1 = false;
                bool result2 = false;
                bool __result = false;
                var AllAssets = AssetBundle.LoadFromFile(path).LoadAllAssets();
                foreach (var Asset in AllAssets)
                {
                    var TryCastFontAsset = Asset.TryCast<TMP_FontAsset>();
                    if (TryCastFontAsset)
                    {
                        UnityEngine.Object.DontDestroyOnLoad(TryCastFontAsset);
                        TryCastFontAsset.hideFlags |= HideFlags.HideAndDontSave;
                        tmpfrenchfonts.Add(TryCastFontAsset);
                        tmpfrenchfontsnames.Add(TryCastFontAsset.name);
                        result1 = true;
                    }
                    var TryCastMaterial = Asset.TryCast<Material>();
                    if (TryCastMaterial)
                    {

                        UnityEngine.Object.DontDestroyOnLoad(TryCastMaterial);
                        TryCastMaterial.hideFlags |= HideFlags.HideAndDontSave;
                        tmpfrenchmats.Add(TryCastMaterial);
                        tmpfrenchmatsnames.Add(TryCastMaterial.name);
                        result2 = true;
                    }

                    if (result1 = result2)
                        __result = true;
                }

                return __result;
            }
            return false;
        }
        public static bool GetFrenchFonts(string fontname, out TMP_FontAsset fontAsset)
        {
            fontAsset = null;
            if (tmpfrenchfonts.Count == 0)
                return false;
            if (fontname == "BebasKai SDF" || fontname == "Liberation Sans SDF")
            {
                fontAsset = GetFrenchFonts(0);
                return true;
            }
            if (fontname == "Caveat Semibold SDF")
            {
                fontAsset = GetFrenchFonts(1);
                return true;
            }
            if (fontname == "ExcelsiorSans SDF")
            {
                fontAsset = GetFrenchFonts(2);
                return true;
            }
            if (fontname.StartsWith("Corporate-Logo-Bold") || fontname == "Mikodacs SDF" || fontname == "KOTRA_BOLD SDF")
            {
                fontAsset = GetFrenchFonts(3);
                return true;
            }
            if (fontname == "Pretendard-Regular SDF" || fontname.StartsWith("HigashiOme - Gothic - C") || fontname.StartsWith("SCDream"))
            {
                fontAsset = GetFrenchFonts(4);
                return true;
            }
            return false;
        }
        public static TMP_FontAsset GetFrenchFonts(int idx)
        {
            int Count = tmpfrenchfonts.Count - 1;
            if (Count < idx)
                idx = Count;
            return tmpfrenchfonts[idx];
        }
        public static Material GetFrenchMats(int idx)
        {
            int Count = tmpfrenchmats.Count - 1;
            if (Count < idx)
                idx = Count;
            return tmpfrenchmats[idx];
        }

        public static bool IsFrenchFont(TMP_FontAsset fontAsset)
        {
            return tmpfrenchfontsnames.Contains(fontAsset.name);
        }

        public static bool IsCyrillicMat(Material matAsset)
        {
            return tmpfrenchmatsnames.Contains(matAsset.name);
        }
        public static Texture2D duplicateTexture(Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
        }

        [HarmonyPatch(typeof(TMP_Text), nameof(TMP_Text.font), MethodType.Setter)]
        [HarmonyPrefix]
        private static bool set_font(TMP_Text __instance, ref TMP_FontAsset value)
        {
            if (IsFrenchFont(__instance.m_fontAsset))
                return false;
            string fontname = __instance.m_fontAsset.name;
            if (GetFrenchFonts(fontname, out TMP_FontAsset font))
            {

                Debug.Log("Material Name : " + __instance.fontMaterial.name);
                Debug.Log("Test : " + __instance.text);
                if (__instance.fontMaterial.name.Contains("Mikodacs SDF UnderLine") || __instance.fontMaterial.name.Contains("KOTRA_BOLD SDF Underline"))
                {
                    if (__instance.fontMaterial.IsKeywordEnabled("UNDERLAY_ON"))
                    {

                        if (!premat.ContainsKey(__instance))
                        {
                            premat[__instance] = __instance.fontMaterial;
                        }
                    }
                }
                value = font;
            }
            return true;
        }
        public static Dictionary<TMP_Text, Material> premat = new Dictionary<TMP_Text, Material>();
        public static Dictionary<TMP_Text, Material> prematGlow = new Dictionary<TMP_Text, Material>();
        [HarmonyPatch(typeof(TMP_Text), nameof(TMP_Text.fontMaterial), MethodType.Setter)]
        [HarmonyPrefix]
        private static void set_fontMaterial(TMP_Text __instance, ref Material value)
        {
            if (IsFrenchFont(__instance.m_fontAsset))
            {
                value = __instance.m_fontAsset.material;
                if (premat.ContainsKey(__instance))
                {
                    if (CloneMat == null)
                    {
                        CloneMat = UnityEngine.Object.Instantiate(__instance.m_fontAsset.material);
                    }
                    value = CloneMat;
                    Material pre = premat[__instance];
                    Color f1 = Color.black;

                    CloneMat.shader = Shader.Find("TextMeshPro/Distance Field");
                    CloneMat.SetColor("_UnderlayColor", f1);
                    CloneMat.SetFloat("_UnderlayOffsetX", 5);
                    CloneMat.SetFloat("_UnderlayOffsetY", -5);
                    CloneMat.SetFloat("_UnderlayDilate", 3);
                    CloneMat.SetFloat("_UnderlaySoftness", 0);
                    CloneMat.EnableKeyword("UNDERLAY_ON");

                    __instance.m_fontAsset.material.shader = Shader.Find("TextMeshPro/Distance Field");
                    __instance.m_fontAsset.material.SetColor("_UnderlayColor", f1);
                    __instance.m_fontAsset.material.SetFloat("_UnderlayOffsetX", 0);
                    __instance.m_fontAsset.material.SetFloat("_UnderlayOffsetY", (float)-0.5);
                    __instance.m_fontAsset.material.EnableKeyword("UNDERLAY_ON");
                }
            }
        }
        public static Material CloneMat;
        [HarmonyPatch(typeof(TextMeshProLanguageSetter), nameof(TextMeshProLanguageSetter.UpdateTMP))]
        [HarmonyPrefix]
        private static bool UpdateTMP(TextMeshProLanguageSetter __instance, LOCALIZE_LANGUAGE lang)
        {
            FontInformation fontInformation = __instance._fontInformation.Count > 0 ? __instance._fontInformation[0] : null;
            if (fontInformation == null)
                return false;
            if (fontInformation.fontAsset == null)
                return false;
            if (__instance._text == null)
                return false;
            var raw_fontAsset = fontInformation.fontAsset;
            bool use_fr = GetFrenchFonts(raw_fontAsset.name, out var fr_fontAsset);

            var fontAsset = use_fr ? fr_fontAsset : fontInformation.fontAsset;
            var fontMaterial = use_fr ? fr_fontAsset.material : fontInformation.fontMaterial ?? fontInformation.fontAsset.material;

            __instance._text.font = fontAsset;
            __instance._text.fontMaterial = fontMaterial;
            if (__instance._matSetter)
            {
                __instance._matSetter.defaultMat = fontMaterial;
                __instance._matSetter.ResetMaterial();
            }
            return false;
        }
        [HarmonyPatch(typeof(TextMeshProLanguageSetter), nameof(TextMeshProLanguageSetter.Awake))]
        [HarmonyPrefix]
        private static void Awake(TextMeshProLanguageSetter __instance)
        {
            if (!__instance._text)
                if (__instance.TryGetComponent<TextMeshProUGUI>(out var textMeshProUGUI))
                    __instance._text = textMeshProUGUI;
            if (!__instance._matSetter)
                if (__instance.TryGetComponent<TextMeshProMaterialSetter>(out var textMeshProMaterialSetter))
                    __instance._matSetter = textMeshProMaterialSetter;
        }
        #endregion
        #region Я устав переводить китайский
        private static void LoadRemote2(LOCALIZE_LANGUAGE lang)
        {
            var tm = TextDataManager.Instance;
            TextDataManager.RomoteLocalizeFileList romoteLocalizeFileList = JsonUtility.FromJson<TextDataManager.RomoteLocalizeFileList>(SingletonBehavior<AddressableManager>.Instance.LoadAssetSync<TextAsset>("Assets/Resources_moved/Localize", "RemoteLocalizeFileList", null, null).Item1.ToString());
            tm._uiList.Init(romoteLocalizeFileList.UIFilePaths);
            tm._characterList.Init(romoteLocalizeFileList.CharacterFilePaths);
            tm._personalityList.Init(romoteLocalizeFileList.PersonalityFilePaths);
            tm._enemyList.Init(romoteLocalizeFileList.EnemyFilePaths);
            tm._egoList.Init(romoteLocalizeFileList.EgoFilePaths);
            tm._skillList.Init(romoteLocalizeFileList.SkillFilePaths);
            tm._passiveList.Init(romoteLocalizeFileList.PassiveFilePaths);
            tm._bufList.Init(romoteLocalizeFileList.BufFilePaths);
            tm._itemList.Init(romoteLocalizeFileList.ItemFilePaths);
            tm._keywordList.Init(romoteLocalizeFileList.keywordFilePaths);
            tm._skillTagList.Init(romoteLocalizeFileList.skillTagFilePaths);
            tm._abnormalityEventList.Init(romoteLocalizeFileList.abnormalityEventsFilePath);
            tm._attributeList.Init(romoteLocalizeFileList.attributeTextFilePath);
            tm._abnormalityCotentData.Init(romoteLocalizeFileList.abnormalityGuideContentFilePath);
            tm._keywordDictionary.Init(romoteLocalizeFileList.keywordDictionaryFilePath);
            tm._actionEvents.Init(romoteLocalizeFileList.actionEventsFilePath);
            tm._egoGiftData.Init(romoteLocalizeFileList.egoGiftFilePath);
            tm._stageChapter.Init(romoteLocalizeFileList.stageChapterPath);
            tm._stagePart.Init(romoteLocalizeFileList.stagePartPath);
            tm._stageNodeText.Init(romoteLocalizeFileList.stageNodeInfoPath);
            tm._dungeonNodeText.Init(romoteLocalizeFileList.dungeonNodeInfoPath);
            tm._storyDungeonNodeText.Init(romoteLocalizeFileList.storyDungeonNodeInfoPath);
            tm._quest.Init(romoteLocalizeFileList.Quest);
            tm._dungeonArea.Init(romoteLocalizeFileList.dungeonAreaPath);
            tm._battlePass.Init(romoteLocalizeFileList.BattlePassPath);
            tm._storyTheater.Init(romoteLocalizeFileList.StoryTheater);
            tm._announcer.Init(romoteLocalizeFileList.Announcer);
            tm._normalBattleResultHint.Init(romoteLocalizeFileList.NormalBattleHint);
            tm._abBattleResultHint.Init(romoteLocalizeFileList.AbBattleHint);
            tm._tutorialDesc.Init(romoteLocalizeFileList.TutorialDesc);
            tm._iapProductText.Init(romoteLocalizeFileList.IAPProduct);
            tm._illustGetConditionText.Init(romoteLocalizeFileList.GetConditionText);
            tm._choiceEventResultDesc.Init(romoteLocalizeFileList.ChoiceEventResult);
            tm._battlePassMission.Init(romoteLocalizeFileList.BattlePassMission);
            tm._gachaTitle.Init(romoteLocalizeFileList.GachaTitle);
            tm._introduceCharacter.Init(romoteLocalizeFileList.IntroduceCharacter);
            tm._userBanner.Init(romoteLocalizeFileList.UserBanner);
            tm._threadDungeon.Init(romoteLocalizeFileList.ThreadDungeon);
            tm._railwayDungeonText.Init(romoteLocalizeFileList.RailwayDungeon);
            tm._railwayDungeonNodeText.Init(romoteLocalizeFileList.RailwayDungeonNodeInfo);
            tm._railwayDungeonStationName.Init(romoteLocalizeFileList.RailwayDungeonStationName);
            tm._dungeonName.Init(romoteLocalizeFileList.DungeonName);
            tm._danteNoteDesc.Init(romoteLocalizeFileList.DanteNote);
            tm._danteNoteCategoryKeyword.Init(romoteLocalizeFileList.DanteNoteCategoryKeyword);
            tm._userTicket_L.Init(romoteLocalizeFileList.UserTicketL);
            tm._userTicket_R.Init(romoteLocalizeFileList.UserTicketR);
            tm._userTicket_EGOBg.Init(romoteLocalizeFileList.UserTicketEGOBg);
            tm._panicInfo.Init(romoteLocalizeFileList.PanicInfo);
            tm._mentalConditionList.Init(romoteLocalizeFileList.mentalCondition);
            tm._dungeonStartBuffs.Init(romoteLocalizeFileList.DungeonStartBuffs);
            tm._railwayDungeonBuffText.Init(romoteLocalizeFileList.RailwayDungeonBuff);
            tm._buffAbilityList.Init(romoteLocalizeFileList.buffAbilities);
            tm._egoGiftCategory.Init(romoteLocalizeFileList.EgoGiftCategory);
            tm._mirrorDungeonEgoGiftLockedDescList.Init(romoteLocalizeFileList.MirrorDungeonEgoGiftLockedDesc);
            tm._mirrorDungeonEnemyBuffDescList.Init(romoteLocalizeFileList.MirrorDungeonEnemyBuffDesc);
            tm._iapStickerText.Init(romoteLocalizeFileList.IAPSticker);
            tm._battleSpeechBubbleText.Init(romoteLocalizeFileList.BattleSpeechBubble);
            tm._danteAbilityDataList.Init(romoteLocalizeFileList.DanteAbility);

            tm._abnormalityEventCharDlg.AbEventCharDlgRootInit(romoteLocalizeFileList.abnormalityCharDlgFilePath);

            tm._personalityVoiceText._voiceDictionary.JsonDataListInit(romoteLocalizeFileList.PersonalityVoice);
            tm._announcerVoiceText._voiceDictionary.JsonDataListInit(romoteLocalizeFileList.AnnouncerVoice);
            tm._bgmLyricsText._lyricsDictionary.JsonDataListInit(romoteLocalizeFileList.BgmLyrics);
            tm._egoVoiceText._voiceDictionary.JsonDataListInit(romoteLocalizeFileList.EGOVoice);
        }
        [HarmonyPatch(typeof(EGOVoiceJsonDataList), nameof(EGOVoiceJsonDataList.Init))]
        [HarmonyPrefix]
        private static bool EGOVoiceJsonDataListInit(EGOVoiceJsonDataList __instance, List<string> jsonFilePathList)
        {
            __instance._voiceDictionary = new Dictionary<string, LocalizeTextDataRoot<TextData_EGOVoice>>();
            int callcount = 0;
            foreach (string jsonFilePath in jsonFilePathList)
            {
                Action<LocalizeTextDataRoot<TextData_EGOVoice>> LoadLocalizeDel = delegate (LocalizeTextDataRoot<TextData_EGOVoice> data)
                {
                    if (data != null)
                    {
                        string[] array = jsonFilePath.Split('_');
                        string text = array[^1];
                        text = text.Replace(".json", "");
                        __instance._voiceDictionary[text] = data;
                    }
                    callcount++;
                    if (callcount == jsonFilePathList.Count)
                        LoadRemote2(LOCALIZE_LANGUAGE.EN);
                };
                AddressableManager.Instance.LoadLocalizeJsonAssetAsync<TextData_EGOVoice>(jsonFilePath, LoadLocalizeDel);
            }
            return false;
        }
        [HarmonyPatch(typeof(StoryDataParser), nameof(StoryDataParser.GetScenario))]
        [HarmonyPrefix]
        private static bool GetScenario(StoryDataParser __instance, string scenarioID, ref LOCALIZE_LANGUAGE lang, ref Scenario __result)
        {
            TextAsset textAsset = AddressableManager.Instance.LoadAssetSync<TextAsset>("Assets/Resources_moved/Story/Effect", scenarioID, null, null).Item1;
            if (!textAsset)
            {
                LCB_EOMod.LogError("Story Unknown Error! Call Story: Dirty Hacker");
                scenarioID = "SDUMMY";
                textAsset = AddressableManager.Instance.LoadAssetSync<TextAsset>("Assets/Resources_moved/Story/Effect", scenarioID, null, null).Item1;
            }
            if (!EO_Manager.Localizes.TryGetValue(scenarioID, out string text))
            {
                LCB_EOMod.LogError("Story error! We can't find the FR story file, so we'll use EN story");
                text = AddressableManager.Instance.LoadAssetSync<TextAsset>("Assets/Resources_moved/Localize/en/StoryData", "EN_" + scenarioID, null, null).Item1.ToString();
            }
            string text2 = textAsset.ToString();
            Scenario scenario = new()
            {
                ID = scenarioID
            };
            JSONArray jsonarray = JSONNode.Parse(text)[0].AsArray;
            JSONArray jsonarray2 = JSONNode.Parse(text2)[0].AsArray;
            int s = 0;
            for (int i = 0; i < jsonarray.Count; i++)
            {
                var jSONNode = jsonarray[i];
                if (jSONNode.Count < 1)
                {
                    s++;
                    continue;
                }
                int num;
                if (jSONNode[0].IsNumber && jSONNode[0].AsInt < 0)
                    continue;
                num = i - s;
                JSONNode effectToken = jsonarray2[num];
                if ("{\"controlCG\": {\"IsNotPlayDialog\":true}}".Equals(effectToken["effectv2"]))
                {
                    s--;
                    scenario.Scenarios.Add(new Dialog(num, new(), effectToken));
                    effectToken = jsonarray2[num + 1];
                }
                scenario.Scenarios.Add(new Dialog(num, jSONNode, effectToken));
            }
            __result = scenario;
            return false;
        }
        [HarmonyPatch(typeof(StoryAssetLoader), nameof(StoryAssetLoader.GetTellerName))]
        [HarmonyPrefix]
        private static bool GetTellerName(StoryAssetLoader __instance, string name, LOCALIZE_LANGUAGE lang, ref string __result)
        {
            if (__instance._modelAssetMap.TryGetValueEX(name, out var scenarioAssetData))
                __result = scenarioAssetData.krname ?? string.Empty;
            return false;
        }
        [HarmonyPatch(typeof(StoryAssetLoader), nameof(StoryAssetLoader.GetTellerTitle))]
        [HarmonyPrefix]
        private static bool GetTellerTitle(StoryAssetLoader __instance, string name, LOCALIZE_LANGUAGE lang, ref string __result)
        {
            if (__instance._modelAssetMap.TryGetValueEX(name, out var scenarioAssetData))
                __result = scenarioAssetData.nickName ?? string.Empty;
            return false;
        }
        private static bool LoadLocal(LOCALIZE_LANGUAGE lang)
        {
            var tm = TextDataManager.Instance;
            TextDataManager.LocalizeFileList localizeFileList = JsonUtility.FromJson<TextDataManager.LocalizeFileList>(Resources.Load<TextAsset>("Localize/LocalizeFileList").ToString());
            tm._loginUIList.Init(localizeFileList.LoginUIFilePaths);
            tm._fileDownloadDesc.Init(localizeFileList.FileDownloadDesc);
            //tm._battleHint._dic.Clear();
            tm._battleHint.Init(localizeFileList.BattleHint);
            return false;
        }
        [HarmonyPatch(typeof(TextDataManager), nameof(TextDataManager.LoadRemote))]
        [HarmonyPrefix]
        private static void LoadRemote(ref LOCALIZE_LANGUAGE lang)
           => lang = LOCALIZE_LANGUAGE.EN;
        [HarmonyPatch(typeof(StoryData), nameof(StoryData.Init))]
        [HarmonyPostfix]
        private static void StoryDataInit(StoryData __instance)
        {
            foreach (ScenarioAssetData scenarioAssetData in JsonUtility.FromJson<ScenarioAssetDataList>(EO_Manager.Localizes["NickName"]).assetData)
                __instance._modelAssetMap._modelAssetMap[scenarioAssetData.name] = scenarioAssetData;
        }
        [HarmonyPatch(typeof(LoginSceneManager), nameof(LoginSceneManager.SetLoginInfo))]
        [HarmonyPostfix]
        private static void SetLoginInfo(LoginSceneManager __instance)
        {
            LoadLocal(LOCALIZE_LANGUAGE.EN);
            __instance.tmp_loginAccount.text = "LimbusCompany Français v" + LCB_EOMod.VERSION;
            __instance.tmp_loginAccount.characterSpacing = -2;
            __instance.tmp_loginAccount.lineSpacing = -20;
        }
        private static void Init<T>(this JsonDataList<T> jsonDataList, List<string> jsonFilePathList) where T : LocalizeTextData, new()
        {
            foreach (string text in jsonFilePathList)
            {
                if (!EO_Manager.Localizes.TryGetValue(text, out var text2)) { continue; }
                var localizeTextData = JsonUtility.FromJson<LocalizeTextDataRoot<T>>(text2);
                foreach (T t in localizeTextData.DataList)
                {
                    jsonDataList._dic[t.ID.ToString()] = t;
                }
            }
        }

        private static void AbEventCharDlgRootInit(this AbEventCharDlgRoot root, List<string> jsonFilePathList)
        {
            root._personalityDict = new();
            foreach (string text in jsonFilePathList)
            {
                if (!EO_Manager.Localizes.TryGetValue(text, out var text2)) { continue; }
                var localizeTextData = JsonUtility.FromJson<LocalizeTextDataRoot<TextData_AbnormalityEventCharDlg>>(text2);
                foreach (var t in localizeTextData.DataList)
                {
                    if (!root._personalityDict.TryGetValueEX(t.PersonalityID, out var abEventKeyDictionaryContainer))
                    {
                        abEventKeyDictionaryContainer = new AbEventKeyDictionaryContainer();
                        root._personalityDict[t.PersonalityID] = abEventKeyDictionaryContainer;
                    }
                    string[] array = t.Usage.Trim().Split(new char[] { '(', ')' });
                    for (int i = 1; i < array.Length; i += 2)
                    {
                        string[] array2 = array[i].Split(',');
                        int num = int.Parse(array2[0].Trim());
                        AB_DLG_EVENT_TYPE ab_DLG_EVENT_TYPE = (AB_DLG_EVENT_TYPE)Enum.Parse(typeof(AB_DLG_EVENT_TYPE), array2[1].Trim());
                        AbEventKey abEventKey = new(num, ab_DLG_EVENT_TYPE);
                        abEventKeyDictionaryContainer.AddDlgWithEvent(abEventKey, t);
                    }
                }

            }
        }
        private static void JsonDataListInit<T>(this Dictionary<string, LocalizeTextDataRoot<T>> jsonDataList, List<string> jsonFilePathList)
        {
            foreach (string text in jsonFilePathList)
            {
                if (!EO_Manager.Localizes.TryGetValue(text, out var text2)) { continue; }
                var localizeTextData = JsonUtility.FromJson<LocalizeTextDataRoot<T>>(text2);
                jsonDataList[text.Split('_')[^1]] = localizeTextData;
            }
        }

        #endregion
        public static bool TryGetValueEX<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, out TValue value)
        {
            var entries = dic._entries;
            var Entr = dic.FindEntry(key);
            value = Entr == -1 ? default : entries == null ? default : entries[Entr].value;
            return value != null;
        }
    }
}
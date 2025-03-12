using System;
using System.Collections.Generic;
using System.Diagnostics;
using Definitions.Build;
using TrollKing.Core;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace TrollKingStudios.Utilities
{
    public static class SetAuthDomain
    {
        private static LogScope Log = new LogScope(nameof(SetAuthDomain));

        private static string TK_QUEST_DEV = "TK_QUEST_DEV";
        private static string TK_QUEST_PROD = "TK_QUEST_PROD";
        private static string TK_ANDROID_DEV = "TK_ANDROID_DEV";
        private static string TK_ANDROID_PROD = "TK_ANDROID_PROD";
        private static string TK_IOS_DEV = "TK_IOS_DEV";
        private static string TK_IOS_PROD = "TK_IOS_PROD";
        private static string TK_WIN_DEV = "TK_WIN_DEV";
        private static string TK_WIN_PROD = "TK_WIN_PROD";
        
        private static List<string> AuthSymbols = new ()
        {
            TK_ANDROID_DEV,
            TK_ANDROID_PROD,
            TK_IOS_DEV,
            TK_IOS_PROD,
            TK_WIN_DEV,
            TK_WIN_PROD,
            TK_QUEST_DEV,
            TK_QUEST_PROD
        };
        
        // BETA
        
        [MenuItem("TrollKing/Auth/Beta/Quest")]
        public static void SetAuthDomainBetaQuest()
        {
            ReplaceAuthSymbol(NamedBuildTarget.Android, TK_QUEST_DEV);
        }
        
        [MenuItem("TrollKing/Auth/Beta/Android")]
        public static void SetAuthDomainBetaAndroid()
        {
            ReplaceAuthSymbol(NamedBuildTarget.Android, TK_ANDROID_DEV);
        }
        
        [MenuItem("TrollKing/Auth/Beta/IOS")]
        public static void SetAuthDomainBetaIOS()
        {
            ReplaceAuthSymbol(NamedBuildTarget.iOS, TK_IOS_DEV);
        }
        
        [MenuItem("TrollKing/Auth/Beta/Windows")]
        public static void SetAuthDomainBetaWin()
        {
            ReplaceAuthSymbol(NamedBuildTarget.Standalone, TK_WIN_DEV);
        }
        
        // PROD
        
        [MenuItem("TrollKing/Auth/Prod/Quest")]
        public static void SetAuthDomainProdQuest()
        {
            ReplaceAuthSymbol(NamedBuildTarget.Android, TK_QUEST_PROD);
        }
        
        [MenuItem("TrollKing/Auth/Prod/Android")]
        public static void SetAuthDomainProdAndroid()
        {
            ReplaceAuthSymbol(NamedBuildTarget.Android, TK_ANDROID_PROD);
        }
        
        [MenuItem("TrollKing/Auth/Prod/IOS")]
        public static void SetAuthDomainProdIOS()
        {
            ReplaceAuthSymbol(NamedBuildTarget.iOS, TK_IOS_PROD);
        }
        
        [MenuItem("TrollKing/Auth/Prod/Windows")]
        public static void SetAuthDomainProdWin()
        {
            ReplaceAuthSymbol(NamedBuildTarget.Standalone, TK_WIN_PROD);
        }

        private static string ReplaceAuthSymbol(NamedBuildTarget target, string toAdd)
        {
            var scriptingSymbols = PlayerSettings.GetScriptingDefineSymbols(target);
            var symbols = scriptingSymbols.Split(';');

            List<string> newSymbols = new List<string>();
            foreach (string symbol in symbols)
            {
                if (AuthSymbols.Contains(symbol))
                {
                    continue;
                }
                newSymbols.Add(symbol);
            }
            
            newSymbols.Add(toAdd);

            var newSymbolString = string.Join(";", newSymbols);
            
            Log.Info(() => $"Scripting Symbols: {scriptingSymbols} new: {newSymbolString}");
            PlayerSettings.SetScriptingDefineSymbols(target, newSymbolString);
            
            return newSymbolString;
        }
    }
}

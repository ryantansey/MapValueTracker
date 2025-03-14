﻿using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using TMPro;
using UnityEngine;
using MapValueTracker.Config;
using System.Collections.Generic;
using System.Linq;

namespace MapValueTracker
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    public class MapValueTracker : BaseUnityPlugin
    {
        public const string PLUGIN_GUID = "MapValueTracker";
        public const string PLUGIN_NAME = "MapValueTracker";
        public const string PLUGIN_VERSION = "1.1.2";

        public static new ManualLogSource Logger;
        private readonly Harmony harmony = new Harmony("Tansinator.REPO.MapValueTracker");

        public static MapValueTracker instance;
        public static GameObject textInstance;
        public static TextMeshProUGUI valueText;

        public static float totalValue = 0f;

        public void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");

            if (instance == null)
            {
                instance = this;
            }

            Configuration.Init(Config);

            harmony.PatchAll();
        }

        public static void ResetValues()
        {
            if (!SemiFunc.RunIsLevel())
                totalValue = 0;

            Logger.LogDebug("In ResetValues()");

            Logger.LogInfo("Total Map Value: " + totalValue);
        }

        public static void CheckForItems(ValuableObject ignoreThis = null)
        {
            if (!Traverse.Create(RoundDirector.instance).Field("allExtractionPointsCompleted").GetValue<bool>())
            {
                totalValue = 0f;
                List<ValuableObject> valuebleObjects = Object.FindObjectsOfType<ValuableObject>().ToList();

                if (ignoreThis != null)
                {
                    valuebleObjects.Remove(ignoreThis);
                }
                for (int i = 0; i < valuebleObjects.Count; i++)
                {
                    totalValue += valuebleObjects[i].dollarValueCurrent;
                }
            }
        }
    }

    public class MyOnDestroy : MonoBehaviour
    {
        void OnDestroy()
        {
            MapValueTracker.Logger.LogInfo("Destroying!");
            var vo = GetComponent<ValuableObject>();
            MapValueTracker.Logger.LogDebug("Destroyed Valuable Object! " + vo.name + " Val: " + vo.dollarValueCurrent);
            MapValueTracker.totalValue -= vo.dollarValueCurrent;
            MapValueTracker.Logger.LogDebug("Total Val: " + MapValueTracker.totalValue);
        }
    }


}
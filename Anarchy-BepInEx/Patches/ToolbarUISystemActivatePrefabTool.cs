// <copyright file="ToolbarUISystemActivatePrefabTool.cs" company="Yenyang's Mods. MIT License">
// Copyright (c) Yenyang's Mods. MIT License. All rights reserved.
// </copyright>

namespace Anarchy.Patches
{
    using Anarchy.Utils;
    using Game.Prefabs;
    using Game.Tools;
    using Game.UI.InGame;
    using HarmonyLib;
    using Unity.Entities;

    /// <summary>
    /// Patches Unique Asset Tracking System IsPlacedUniqueAsset to return false.
    /// </summary>
    [HarmonyPatch(typeof(ToolbarUISystem), "ActivatePrefabTool")]
    public class ToolbarUISystemActivatePrefabToolPatch
    {
        /// <summary>
        /// Patches Toolbar UISystem Activate Prefab Tool to allow for multiple uniques being placed.
        /// </summary>
        /// <param name="assetEntity">The prefab entity selected.</param>
        public static void Postfix(Entity assetEntity)
        {
            ToolSystem toolSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<ToolSystem>();
            PrefabSystem prefabSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<PrefabSystem>();
            if (assetEntity != Entity.Null && AnarchyMod.Settings.AllowPlacingMultipleUniqueBuildings)
            {
                PrefabBase prefab = prefabSystem.GetPrefab<PrefabBase>(assetEntity);
                toolSystem.ActivatePrefabTool(prefab);
            }
        }
    }
}

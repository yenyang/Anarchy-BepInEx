// <copyright file="ToolbarUISystemOnUpdatePatch.cs" company="Yenyang's Mods. MIT License">
// Copyright (c) Yenyang's Mods. MIT License. All rights reserved.
// </copyright>

namespace Anarchy.Patches
{
    using Anarchy.Utils;
    using Game.UI.InGame;
    using HarmonyLib;
    using Unity.Entities;

    /// <summary>
    /// Patches Unique Asset Tracking System IsPlacedUniqueAsset to return false.
    /// </summary>
    [HarmonyPatch(typeof(ToolbarUISystem), "OnUpdate")]
    public class ToolbarUISystemOnUpdatePatch
    {
        /// <summary>
        /// Patches Unique Asset Tracking System IsPlacedUniqueAsset to return false.
        /// </summary>
        public static void Prefix()
        {
            ToolbarUISystem toolbarUISystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<ToolbarUISystem>();
            toolbarUISystem.SetMemberValue("m_UniqueAssetStatusChanged", false);
        }
    }
}

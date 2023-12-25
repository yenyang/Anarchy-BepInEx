// <copyright file="GameModeExtensionsIsEditorPatch.cs" company="Yenyang's Mods. MIT License">
// Copyright (c) Yenyang's Mods. MIT License. All rights reserved.
// </copyright>

namespace Anarchy.Patches
{
    using Anarchy.Tooltip;
    using Game;
    using Game.Tools;
    using HarmonyLib;
    using Unity.Entities;

    /// <summary>
    /// Patches IsEditor while using Bulldoze tool inGame so that you can get editor functinality.
    /// </summary>
    [HarmonyPatch(typeof(GameModeExtensions), "IsEditor")]
    public class GameModeExtensionsIsEditorPatch
    {
        /// <summary>
        /// Patches IsEditor while using Bulldoze tool inGame so that you can get editor functinality.
        /// </summary>
        /// <param name="result"> A bool representing whether the game is in Editor mode.</param>
        public static void Postfix(ref bool result)
        {
            AnarchySystem anarchySystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<AnarchySystem>();
            ToolSystem toolSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<ToolSystem>();
            BulldozeToolSystem bulldozeToolSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<BulldozeToolSystem>();
            if (anarchySystem.AnarchyEnabled && toolSystem.activeTool == bulldozeToolSystem && toolSystem.actionMode.IsGame())
            {
                result = true;
            }
        }
    }
}

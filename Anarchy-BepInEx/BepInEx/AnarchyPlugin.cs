// <copyright file="AnarchyPlugin.cs" company="Yenyang's Mods. MIT License">
// Copyright (c) Yenyang's Mods. MIT License. All rights reserved.
// </copyright>

#if BEPINEX

namespace Anarchy
{
    using BepInEx;
    using Game;
    using Game.Common;
    using HarmonyLib;

    /// <summary>
    /// Mod entry point for BepInEx configuaration.
    /// </summary>
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, "Anarchy", "1.3.1")]
    [HarmonyPatch]
    public class AnarchyPlugin : BaseUnityPlugin
    {
        /// <summary>
        /// A static instance of the IMod for mod entry point.
        /// </summary>
        internal static AnarchyMod _mod;

        /// <summary>
        /// Patches and Injects mod into game via Harmony.
        /// </summary>
        public void Awake()
        {
            _mod = new ();
            _mod.OnLoad();
            _mod.Logger.Info($"{nameof(AnarchyPlugin)}.{nameof(Awake)}");
            Harmony.CreateAndPatchAll(typeof(AnarchyPlugin).Assembly, MyPluginInfo.PLUGIN_GUID);
        }

        [HarmonyPatch(typeof(SystemOrder), nameof(SystemOrder.Initialize), new[] { typeof(UpdateSystem) })]
        [HarmonyPostfix]
        private static void InjectSystems(UpdateSystem updateSystem)
        {
            _mod.Logger.Info($"{nameof(AnarchyPlugin)}.{nameof(InjectSystems)}");
            _mod.OnCreateWorld(updateSystem);
        }
    }
}
#endif
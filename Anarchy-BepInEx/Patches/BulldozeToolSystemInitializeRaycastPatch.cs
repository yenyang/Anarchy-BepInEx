// <copyright file="BulldozeToolSystemInitializeRaycastPatch.cs" company="Yenyang's Mods. MIT License">
// Copyright (c) Yenyang's Mods. MIT License. All rights reserved.
// </copyright>

namespace Anarchy.Patches
{
    using Anarchy.Systems;
    using Anarchy.Tooltip;
    using Game.Areas;
    using Game.Common;
    using Game.Net;
    using Game.Rendering;
    using Game.Tools;
    using HarmonyLib;
    using Unity.Entities;

    /// <summary>
    /// Patches Bulldoze Tool System Inititialize Raycast to add Markers as something to raycast.
    /// </summary>
    [HarmonyPatch(typeof(BulldozeToolSystem), "InitializeRaycast")]
    public class BulldozeToolSystemInitializeRaycastPatch
    {
        /// <summary>
        /// Patches Bulldoze Tool System Inititialize Raycast to add Markers as something to raycast.
        /// </summary>
        public static void Postfix()
        {
            AnarchySystem anarchySystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<AnarchySystem>();
            ToolSystem toolSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<ToolSystem>();
            ToolRaycastSystem toolRaycastSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<ToolRaycastSystem>();
            AnarchyUISystem anarchyUISystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<AnarchyUISystem>();
            RenderingSystem renderingSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<RenderingSystem>();
            if (renderingSystem.markersVisible && anarchyUISystem.SelectedRaycastTarget == AnarchyUISystem.RaycastTarget.Markers)
            {
                toolRaycastSystem.typeMask = TypeMask.Net;
                toolRaycastSystem.netLayerMask = Layer.MarkerPathway | Layer.MarkerTaxiway;
                toolRaycastSystem.raycastFlags = RaycastFlags.Markers;
            }
            else if (anarchyUISystem.SelectedRaycastTarget == AnarchyUISystem.RaycastTarget.Surfaces)
            {
                toolRaycastSystem.typeMask = TypeMask.Areas;
                toolRaycastSystem.areaTypeMask = AreaTypeMask.Surfaces;
                if (anarchySystem.AnarchyEnabled)
                {
                    toolRaycastSystem.raycastFlags |= RaycastFlags.SubElements;
                }
            }
        }
    }
}

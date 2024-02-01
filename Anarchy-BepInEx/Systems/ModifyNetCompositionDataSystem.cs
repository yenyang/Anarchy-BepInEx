// <copyright file="ModifyNetCompositionDataSystem.cs" company="Yenyang's Mods. MIT License">
// Copyright (c) Yenyang's Mods. MIT License. All rights reserved.
// </copyright>

// #define VERBOSE
namespace Anarchy.Systems
{
    using Anarchy;
    using Anarchy.Components;
    using Anarchy.Tooltip;
    using Colossal.Entities;
    using Colossal.Logging;
    using Game;
    using Game.Prefabs;
    using Game.Tools;
    using Unity.Collections;
    using Unity.Entities;

    /// <summary>
    /// A system zeros out net composition data height ranges if anarchy is enabled and using net tool.
    /// </summary>
    public partial class ModifyNetCompositionDataSystem : GameSystemBase
    {
        private ToolSystem m_ToolSystem;
        private EntityQuery m_NetCompositionDataQuery;
        private AnarchySystem m_AnarchySystem;
        private ILog m_Log;
        private NetToolSystem m_NetToolSystem;
        private ResetNetCompositionDataSystem m_ResetNetCompositionDataSystem;
        private PrefabSystem m_PrefabSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModifyNetCompositionDataSystem"/> class.
        /// </summary>
        public ModifyNetCompositionDataSystem()
        {
        }

        /// <inheritdoc/>
        protected override void OnCreate()
        {
            m_Log = AnarchyMod.Instance.Logger;
            m_AnarchySystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<AnarchySystem>();
            m_ToolSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<ToolSystem>();
            m_NetToolSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<NetToolSystem>();
            m_PrefabSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<PrefabSystem>();
            m_ResetNetCompositionDataSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<ResetNetCompositionDataSystem>();
            m_Log.Info($"{nameof(ModifyNetCompositionDataSystem)} Created.");
            m_NetCompositionDataQuery = GetEntityQuery(new EntityQueryDesc[]
            {
                new EntityQueryDesc
                {
                    All = new ComponentType[]
                    {
                        ComponentType.ReadWrite<NetCompositionData>(),
                    },
                },
            });
            RequireForUpdate(m_NetCompositionDataQuery);
            base.OnCreate();
        }

        /// <inheritdoc/>
        protected override void OnUpdate()
        {
            if (m_ToolSystem.activeTool != m_NetToolSystem || !m_AnarchySystem.AnarchyEnabled)
            {
                return;
            }

            NativeArray<Entity> entities = m_NetCompositionDataQuery.ToEntityArray(Allocator.Temp);
            foreach (Entity currentEntity in entities)
            {
                if (EntityManager.TryGetComponent(currentEntity, out NetCompositionData netCompositionData))
                {
                    if (!EntityManager.HasComponent<HeightRangeRecord>(currentEntity))
                    {
                        HeightRangeRecord heightRangeRecord = new ()
                        {
                            min = netCompositionData.m_HeightRange.min,
                            max = netCompositionData.m_HeightRange.max,
                        };
                        EntityManager.AddComponent<HeightRangeRecord>(currentEntity);
                        EntityManager.SetComponentData(currentEntity, heightRangeRecord);

                        // m_Log.Debug($"{nameof(ModifyNetCompositionDataSystem)}.{nameof(OnUpdate)} Recorded m_HeightRange {netCompositionData.m_HeightRange.min}+{netCompositionData.m_HeightRange.max} for entity: {currentEntity.Index}.{currentEntity.Version}.");
                    }

                    netCompositionData.m_HeightRange.min = -1f * AnarchyMod.Settings.MinimumClearanceBelowElevatedNetworks;
                    netCompositionData.m_HeightRange.max = 0f;

                    // m_Log.Debug($"{nameof(ModifyNetCompositionDataSystem)}.{nameof(OnUpdate)} Setting m_HeightRange to 0 for entity: {currentEntity.Index}.{currentEntity.Version}.");
                    EntityManager.SetComponentData(currentEntity, netCompositionData);
                }
                else
                {
                    m_Log.Warn($"{nameof(ModifyNetCompositionDataSystem)}.{nameof(OnUpdate)} could not retrieve net composition data for Entity {currentEntity.Index}.{currentEntity.Version}.");
                }
            }

            entities.Dispose();
        }
    }
}

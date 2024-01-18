// <copyright file="DisableToolErrorsSystem.cs" company="Yenyang's Mods. MIT License">
// Copyright (c) Yenyang's Mods. MIT License. All rights reserved.
// </copyright>

// #define VERBOSE
namespace Anarchy.Systems
{
    using Anarchy;
    using Anarchy.Tooltip;
    using Colossal.Entities;
    using Colossal.Logging;
    using Game;
    using Game.Prefabs;
    using Game.Tools;
    using Unity.Collections;
    using Unity.Entities;

    /// <summary>
    /// A system the queries for toolErrorPrefabs and then disables relevent tool errors in game if active tool is applicable.
    /// </summary>
    public partial class NetCompositionDataSystem : GameSystemBase
    {
        private ToolSystem m_ToolSystem;
        private EntityQuery m_NetCompositionDataQuery;
        private AnarchySystem m_AnarchySystem;
        private ILog m_Log;
        private NetToolSystem m_NetToolSystem;
        private PrefabSystem m_PrefabSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetCompositionDataSystem"/> class.
        /// </summary>
        public NetCompositionDataSystem()
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
            m_Log.Info($"{nameof(DisableToolErrorsSystem)} Created.");
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
            if (m_ToolSystem.activeTool != m_NetToolSystem)
            {
                return;
            }

            NativeArray <Entity> entities = m_NetCompositionDataQuery.ToEntityArray(Allocator.Temp);
            foreach (Entity currentEntity in entities)
            {
                if (EntityManager.TryGetComponent(currentEntity, out NetCompositionData netCompositionData))
                {
                    netCompositionData.m_HeightRange.min = 0f;
                    netCompositionData.m_HeightRange.max = 0f;
                    m_Log.Debug($"{nameof(NetCompositionDataSystem)}.{nameof(OnUpdate)} Setting m_HeightRange to 0 for entity: {currentEntity.Index}.{currentEntity.Version}.");
                    EntityManager.SetComponentData(currentEntity, netCompositionData);
                }
            }

            entities.Dispose();
        }
    }
}

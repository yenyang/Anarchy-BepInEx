// <copyright file="AnarchyPlopSystem.cs" company="Yenyang's Mods. MIT License">
// Copyright (c) Yenyang's Mods. MIT License. All rights reserved.
// </copyright>

namespace Anarchy.Systems
{
    using System.Collections.Generic;
    using Anarchy;
    using Anarchy.Components;
    using Anarchy.Tooltip;
    using Colossal.Logging;
    using Game;
    using Game.Common;
    using Game.Prefabs;
    using Game.Tools;
    using Unity.Entities;

    /// <summary>
    /// A system that prevents objects from being overriden when placed on each other.
    /// </summary>
    public partial class OwnedAndOverrideSystem : GameSystemBase
    {
        private readonly List<string> m_AppropriateTools = new List<string>()
        {
            { "Object Tool" },
            { "Line Tool" },
            { "Bulldoze Tool" },
        };

        private AnarchySystem m_AnarchySystem;
        private ILog m_Log;
        private ToolSystem m_ToolSystem;
        private NetToolSystem m_NetToolSystem;
        private ObjectToolSystem m_ObjectToolSystem;
        private PrefabSystem m_PrefabSystem;
        private EntityQuery m_OwnedAndOverridenQuery;

        /// <summary>
        /// Initializes a new instance of the <see cref="OwnedAndOverrideSystem"/> class.
        /// </summary>
        public OwnedAndOverrideSystem()
        {
        }

        /// <inheritdoc/>
        protected override void OnCreate()
        {
            m_Log = AnarchyMod.Instance.Logger;
            m_Log.Info($"{nameof(OwnedAndOverrideSystem)} Created.");
            m_AnarchySystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<AnarchySystem>();
            m_ToolSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<ToolSystem>();
            m_NetToolSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<NetToolSystem>();
            m_ObjectToolSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<ObjectToolSystem>();
            m_PrefabSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<PrefabSystem>();
            m_OwnedAndOverridenQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<Owner>(),
                    ComponentType.ReadOnly<Updated>(),
                    ComponentType.ReadOnly<Overridden>(),
                },
                None = new ComponentType[]
                {
                    ComponentType.ReadOnly<Temp>(),
                    ComponentType.ReadOnly<BuildingData>(),
                },
            });
            RequireForUpdate(m_OwnedAndOverridenQuery);
            base.OnCreate();
        }

        /// <inheritdoc/>
        protected override void OnUpdate()
        {
            if (m_ToolSystem.activeTool.toolID == null)
            {
                return;
            }

            if (m_ToolSystem.activePrefab != null)
            {
                Entity prefabEntity = m_PrefabSystem.GetEntity(m_ToolSystem.activePrefab);
                if (EntityManager.HasComponent<BuildingData>(prefabEntity))
                {
                    return;
                }
            }

            if (m_AnarchySystem.AnarchyEnabled && m_AppropriateTools.Contains(m_ToolSystem.activeTool.toolID) && !m_NetToolSystem.TrySetPrefab(m_ToolSystem.activePrefab))
            {
                EntityManager.RemoveComponent(m_OwnedAndOverridenQuery, ComponentType.ReadWrite<Overridden>());
                EntityManager.AddComponent(m_OwnedAndOverridenQuery, ComponentType.ReadWrite<PreventOverride>());
            }
        }
    }
}

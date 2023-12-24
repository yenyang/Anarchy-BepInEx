// <copyright file="AnarchyPlopSystem.cs" company="Yenyang's Mods. MIT License">
// Copyright (c) Yenyang's Mods. MIT License. All rights reserved.
// </copyright>

namespace Anarchy.Systems
{
    using System.Collections.Generic;
    using Anarchy;
    using Anarchy.Components;
    using Anarchy.Tooltip;
    using Colossal.Entities;
    using Colossal.Logging;
    using Game;
    using Game.Common;
    using Game.Prefabs;
    using Game.Tools;
    using Unity.Entities;

    /// <summary>
    /// A system that prevents objects from being overriden when placed on each other.
    /// </summary>
    public partial class AnarchyPlopSystem : GameSystemBase
    {
        private readonly List<string> m_AppropriateTools = new List<string>()
        {
            { "Object Tool" },
            { "Line Tool" },
        };

        private AnarchySystem m_AnarchySystem;
        private ILog m_Log;
        private ToolSystem m_ToolSystem;
        private AnarchyUISystem m_AnarchyUISystem;
        private NetToolSystem m_NetToolSystem;
        private ObjectToolSystem m_ObjectToolSystem;
        private PrefabSystem m_PrefabSystem;
        private EntityQuery m_CreatedQuery;
        private EntityQuery m_OwnedAndOverridenQuery;
        private EntityQuery m_AnarchyObjectQuery;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnarchyPlopSystem"/> class.
        /// </summary>
        public AnarchyPlopSystem()
        {
        }

        /// <inheritdoc/>
        protected override void OnCreate()
        {
            m_Log = AnarchyMod.Instance.Logger;
            m_Log.Info($"{nameof(AnarchyPlopSystem)} Created.");
            m_AnarchySystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<AnarchySystem>();
            m_ToolSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<ToolSystem>();
            m_NetToolSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<NetToolSystem>();
            m_ObjectToolSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<ObjectToolSystem>();
            m_AnarchyUISystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<AnarchyUISystem>();
            m_PrefabSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<PrefabSystem>();
            m_CreatedQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<Created>(),
                    ComponentType.ReadOnly<Updated>(),
                },
                None = new ComponentType[]
                {
                    ComponentType.ReadOnly<Temp>(),
                    ComponentType.ReadOnly<Owner>(),
                    ComponentType.ReadOnly<BuildingData>(),
                },
            });
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
                    ComponentType.ReadOnly<Deleted>(),
                },
            });
            m_AnarchyObjectQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
               {
                    ComponentType.ReadOnly<AnarchyObject>(),
                    ComponentType.ReadOnly<Updated>(),
                    ComponentType.ReadOnly<Overridden>(),
               },
                None = new ComponentType[]
               {
                    ComponentType.ReadOnly<Temp>(),
                    ComponentType.ReadOnly<BuildingData>(),
                    ComponentType.ReadOnly<Deleted>(),
               },
            });
            RequireForUpdate(m_CreatedQuery);
            base.OnCreate();
        }

        /// <inheritdoc/>
        protected override void OnUpdate()
        {
            if (m_ToolSystem.activeTool.toolID == null)
            {
                return;
            }

            bool placingBuilding = false;
            if (m_ToolSystem.activePrefab != null)
            {
                Entity prefabEntity = m_PrefabSystem.GetEntity(m_ToolSystem.activePrefab);
                if (EntityManager.HasComponent<BuildingData>(prefabEntity))
                {
                    placingBuilding = true;
                }
            }

            if (m_AnarchySystem.AnarchyEnabled && m_AppropriateTools.Contains(m_ToolSystem.activeTool.toolID) && !m_NetToolSystem.TrySetPrefab(m_ToolSystem.activePrefab) && !placingBuilding)
            {
                EntityManager.RemoveComponent(m_CreatedQuery, ComponentType.ReadOnly<Overridden>());

                if (AnarchyMod.Settings.PermanetlyPreventOverride)
                {
                    EntityManager.AddComponent(m_CreatedQuery, ComponentType.ReadOnly<Overridden>());
                }

                EntityManager.AddComponent(m_CreatedQuery, ComponentType.ReadOnly<AnarchyObject>());

                EntityManager.RemoveComponent(m_OwnedAndOverridenQuery, ComponentType.ReadOnly<Overridden>());
                EntityManager.AddComponent(m_OwnedAndOverridenQuery, ComponentType.ReadOnly<PreventOverride>());
            }

            if (m_AnarchySystem.AnarchyEnabled && m_AnarchySystem.IsToolAppropriate(m_ToolSystem.activeTool.toolID))
            {
                EntityManager.RemoveComponent(m_AnarchyObjectQuery, ComponentType.ReadOnly<Overridden>());
                EntityManager.AddComponent(m_AnarchyObjectQuery, ComponentType.ReadOnly<Updated>());
            }
        }
    }
}

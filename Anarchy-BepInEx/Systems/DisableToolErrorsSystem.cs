﻿// <copyright file="DisableToolErrorsSystem.cs" company="Yenyang's Mods. MIT License">
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
    public partial class DisableToolErrorsSystem : GameSystemBase
    {
        private ToolSystem m_ToolSystem;
        private EntityQuery m_ToolErrorPrefabQuery;
        private AnarchySystem m_AnarchySystem;
        private EnableToolErrorsSystem m_EnableToolErrorsSystem;
        private ILog m_Log;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisableToolErrorsSystem"/> class.
        /// </summary>
        public DisableToolErrorsSystem()
        {
        }

        /// <inheritdoc/>
        protected override void OnCreate()
        {
            m_Log = AnarchyMod.Instance.Logger;
            m_AnarchySystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<AnarchySystem>();
            m_EnableToolErrorsSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<EnableToolErrorsSystem>();
            m_ToolSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<ToolSystem>();
            m_Log.Info($"{nameof(DisableToolErrorsSystem)} Created.");
            m_ToolErrorPrefabQuery = GetEntityQuery(new EntityQueryDesc[]
            {
                new EntityQueryDesc
                {
                    All = new ComponentType[]
                    {
                        ComponentType.ReadWrite<ToolErrorData>(),
                        ComponentType.ReadOnly<NotificationIconData>(),
                    },
                },
            });
            RequireForUpdate(m_ToolErrorPrefabQuery);
            base.OnCreate();
        }

        /// <inheritdoc/>
        protected override void OnUpdate()
        {
            if (m_ToolSystem.activeTool.toolID != null && m_AnarchySystem.AnarchyEnabled)
            {
                if (m_AnarchySystem.IsToolAppropriate(m_ToolSystem.activeTool.toolID))
                {
                    NativeArray<Entity> toolErrorPrefabs = m_ToolErrorPrefabQuery.ToEntityArray(Allocator.TempJob);
                    foreach (Entity currentEntity in toolErrorPrefabs)
                    {
                        if (EntityManager.TryGetComponent<ToolErrorData>(currentEntity, out ToolErrorData toolErrorData))
                       {
                            if (m_AnarchySystem.IsErrorTypeAllowed(toolErrorData.m_Error))
                            {
#if VERBOSE
                                m_Log.Verbose("DisableToolErrorsSystem.OnUpdate currentEntity.index = " + currentEntity.Index + " currentEntity.version = " + currentEntity.Version + " ErrorType = " + toolErrorData.m_Error.ToString());
                                m_Log.Verbose("DisableToolErrorsSystem.OnUpdate toolErrorData.m_Flags = " + toolErrorData.m_Flags.ToString()); 
#endif
                                toolErrorData.m_Flags |= ToolErrorFlags.DisableInGame;
                                toolErrorData.m_Flags |= ToolErrorFlags.DisableInEditor;
                                EntityManager.SetComponentData(currentEntity, toolErrorData);
                           }
                       }
                    }

                    toolErrorPrefabs.Dispose();
                    m_EnableToolErrorsSystem.Enabled = true;
                }
            }
        }
    }
}

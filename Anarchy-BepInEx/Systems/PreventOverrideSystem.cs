// <copyright file="PreventOverrideSystem.cs" company="Yenyang's Mods. MIT License">
// Copyright (c) Yenyang's Mods. MIT License. All rights reserved.
// </copyright>

namespace Anarchy.Systems
{
    using Anarchy;
    using Anarchy.Components;
    using Colossal.Logging;
    using Game;
    using Game.Common;
    using Game.Prefabs;
    using Game.Tools;
    using Unity.Collections;
    using Unity.Entities;

    /// <summary>
    /// A system that prevents objects from being overriden that has a custom component.
    /// </summary>
    public partial class PreventOverrideSystem : GameSystemBase
    {
        private ILog m_Log;
        private EntityQuery m_PreventOverrideQuery;
        private EntityQuery m_NeedToPreventOverrideQuery;
        private PrefabSystem m_PrefabSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreventOverrideSystem"/> class.
        /// </summary>
        public PreventOverrideSystem()
        {
        }

        /// <inheritdoc/>
        protected override void OnCreate()
        {
            m_Log = AnarchyMod.Instance.Logger;
            m_Log.Info($"{nameof(PreventOverrideSystem)} Created.");
            m_PrefabSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<PrefabSystem>();
            m_PreventOverrideQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<PreventOverride>(),
                },
                None = new ComponentType[]
                {
                    ComponentType.ReadOnly<Temp>(),
                },
            });

            m_NeedToPreventOverrideQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
               {
                    ComponentType.ReadOnly<PreventOverride>(),
                    ComponentType.ReadOnly<Overridden>(),
               },
                None = new ComponentType[]
               {
                    ComponentType.ReadOnly<Temp>(),
               },
            });
            RequireAnyForUpdate(new EntityQuery[] { m_NeedToPreventOverrideQuery, m_PreventOverrideQuery });
            base.OnCreate();
        }

        /// <inheritdoc/>
        protected override void OnUpdate()
        {
            if (!m_NeedToPreventOverrideQuery.IsEmptyIgnoreFilter && AnarchyMod.Settings.PermanetlyPreventOverride)
            {
                NativeArray<Entity> overridenEntities = m_NeedToPreventOverrideQuery.ToEntityArray(Allocator.Temp);
                foreach (Entity currentEntity in overridenEntities)
                {
                    if (EntityManager.HasComponent<PreventOverride>(currentEntity))
                    {
                        EntityManager.RemoveComponent<Overridden>(currentEntity);
                        EntityManager.AddComponent<BatchesUpdated>(currentEntity);
                    }
#if VERBOSE
                    m_Log.Verbose($"{nameof(PreventOverrideSystem)}.{nameof(OnUpdate)} Removed  {nameof(Overridden)}  component from Entity {currentEntity.Index}.{currentEntity.Version}");
#endif
                }

                overridenEntities.Dispose();
            }

            if (!m_PreventOverrideQuery.IsEmptyIgnoreFilter && !AnarchyMod.Settings.PermanetlyPreventOverride)
            {
                NativeArray<Entity> entitiesWithComponent = m_PreventOverrideQuery.ToEntityArray(Allocator.Temp);
                foreach (Entity currentEntity in entitiesWithComponent)
                {
                    EntityManager.RemoveComponent<PreventOverride>(currentEntity);
#if VERBOSE
                    m_Log.Verbose($"{nameof(PreventOverrideSystem)}.{nameof(OnUpdate)} Removed {nameof(PreventOverride)} component from Entity {currentEntity.Index}.{currentEntity.Version}");
#endif
                }

                entitiesWithComponent.Dispose();
            }
        }
    }
}

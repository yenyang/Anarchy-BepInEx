// <copyright file="RemovePreventOverrideSystem.cs" company="Yenyang's Mods. MIT License">
// Copyright (c) Yenyang's Mods. MIT License. All rights reserved.
// </copyright>

namespace Anarchy.Systems
{
    using Anarchy;
    using Anarchy.Components;
    using Colossal.Logging;
    using Game;
    using Game.Tools;
    using Unity.Collections;
    using Unity.Entities;

    /// <summary>
    /// A system that removed prevent override component.
    /// </summary>
    public partial class RemovePreventOverrideSystem : GameSystemBase
    {
        private ILog m_Log;
        private EntityQuery m_PreventOverrideQuery;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemovePreventOverrideSystem"/> class.
        /// </summary>
        public RemovePreventOverrideSystem()
        {
        }

        /// <inheritdoc/>
        protected override void OnCreate()
        {
            m_Log = AnarchyMod.Instance.Logger;
            m_Log.Info($"{nameof(RemovePreventOverrideSystem)} Created.");
            m_PreventOverrideQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<PreventOverride>(),
                },
            });
            RequireForUpdate(m_PreventOverrideQuery);
            Enabled = false;
            base.OnCreate();
        }

        /// <inheritdoc/>
        protected override void OnUpdate()
        {
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
            else if (AnarchyMod.Settings.PermanetlyPreventOverride)
            {
                Enabled = false;
            }
        }
    }
}

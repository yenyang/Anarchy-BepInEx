// <copyright file="PreventCullingSystem.cs" company="Yenyang's Mods. MIT License">
// Copyright (c) Yenyang's Mods. MIT License. All rights reserved.
// </copyright>

namespace Anarchy.Systems
{
    using Anarchy;
    using Anarchy.Components;
    using Colossal.Logging;
    using Colossal.Mathematics;
    using Colossal.Serialization.Entities;
    using Game;
    using Game.Common;
    using Game.Rendering;
    using Game.Serialization;
    using Game.Simulation;
    using Game.Tools;
    using Unity.Burst.Intrinsics;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Jobs;
    using Unity.Mathematics;
    using UnityEngine.Rendering;

    /// <summary>
    /// A system that prevents objects from being overriden that has a custom component.
    /// </summary>
    public partial class PreventCullingSystem : GameSystemBase, IPostDeserialize
    {
        private ILog m_Log;
        private EntityQuery m_CullingInfoQuery;
        private TypeHandle __TypeHandle;
        private ToolOutputBarrier m_ToolOutputBarrier;
        private int m_FrameCount = 10;
        private bool m_Loaded = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreventCullingSystem"/> class.
        /// </summary>
        public PreventCullingSystem()
        {
        }

        /// <inheritdoc/>
        public void PostDeserialize(Context context)
        {
            m_Loaded = true;
        }

        /// <inheritdoc/>
        protected override void OnCreate()
        {
            m_Log = AnarchyMod.Instance.Logger;
            m_Log.Info($"{nameof(PreventCullingSystem)} Created.");
            m_ToolOutputBarrier = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<ToolOutputBarrier>();
            m_CullingInfoQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
               {
                    ComponentType.ReadOnly<PreventOverride>(),
                    ComponentType.ReadOnly<CullingInfo>(),
               },
            });
            RequireForUpdate(m_CullingInfoQuery);
            base.OnCreate();
        }

        /// <inheritdoc/>
        protected override void OnUpdate()
        {
            if (m_FrameCount < 10)
            {
                m_FrameCount++;
                return;
            }

            m_FrameCount = 0;

            if (!AnarchyMod.Settings.PreventAccidentalPropCulling || !m_Loaded)
            {
                return;
            }

            __TypeHandle.__CullingInfo_RO_ComponentTypeHandle.Update(ref CheckedStateRef);
            __TypeHandle.__Unity_Entities_Entity_TypeHandle.Update(ref CheckedStateRef);

            PreventCullingJob preventCullingJob = new ()
            {
                m_CullingInfoType = __TypeHandle.__CullingInfo_RO_ComponentTypeHandle,
                m_EntityType = __TypeHandle.__Unity_Entities_Entity_TypeHandle,
                buffer = m_ToolOutputBarrier.CreateCommandBuffer().AsParallelWriter(),
            };
            JobHandle jobHandle = preventCullingJob.ScheduleParallel(m_CullingInfoQuery, Dependency);
            m_ToolOutputBarrier.AddJobHandleForProducer(jobHandle);
            Dependency = jobHandle;
        }

        /// <inheritdoc/>
        protected override void OnCreateForCompiler()
        {
            base.OnCreateForCompiler();
            __TypeHandle.__AssignHandles(ref CheckedStateRef);
        }

        private struct TypeHandle
        {
            [ReadOnly]
            public EntityTypeHandle __Unity_Entities_Entity_TypeHandle;
            [ReadOnly]
            public ComponentTypeHandle<CullingInfo> __CullingInfo_RO_ComponentTypeHandle;

            public void __AssignHandles(ref SystemState state)
            {
                __Unity_Entities_Entity_TypeHandle = state.GetEntityTypeHandle();
                __CullingInfo_RO_ComponentTypeHandle = state.GetComponentTypeHandle<CullingInfo>();
            }
        }

        private struct PreventCullingJob : IJobChunk
        {
            [ReadOnly]
            public EntityTypeHandle m_EntityType;
            [ReadOnly]
            public ComponentTypeHandle<CullingInfo> m_CullingInfoType;
            public EntityCommandBuffer.ParallelWriter buffer;

            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                NativeArray<Entity> entityNativeArray = chunk.GetNativeArray(m_EntityType);
                NativeArray<CullingInfo> cullingInfoNativeArray = chunk.GetNativeArray(ref m_CullingInfoType);
                for (int i = 0; i < chunk.Count; i++)
                {
                    Entity currentEntity = entityNativeArray[i];
                    CullingInfo currentCullingInfo = cullingInfoNativeArray[i];
                    if (currentCullingInfo.m_PassedCulling == 0)
                    {
                        buffer.AddComponent<Updated>(unfilteredChunkIndex, currentEntity);
                    }
                }
            }
        }
    }
}

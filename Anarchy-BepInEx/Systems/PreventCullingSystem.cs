// <copyright file="PreventCullingSystem.cs" company="Yenyang's Mods. MIT License">
// Copyright (c) Yenyang's Mods. MIT License. All rights reserved.
// </copyright>

namespace Anarchy.Systems
{
    using Anarchy;
    using Anarchy.Components;
    using Colossal.Logging;
    using Colossal.Mathematics;
    using Game;
    using Game.Common;
    using Game.Rendering;
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
    public partial class PreventCullingSystem : GameSystemBase
    {
        private ILog m_Log;
        private EntityQuery m_CullingInfoQuery;
        private TypeHandle __TypeHandle;
        private ToolOutputBarrier m_ToolOutputBarrier;
        private SimulationSystem m_SimulationSystem;
        private RenderingSystem m_RenderingSystem;
        private BatchDataSystem m_BatchDataSystem;
        private CameraUpdateSystem m_CameraUpdateSystem;
        private int m_FrameCount = 10;
        private float3 m_PrevCameraPosition;
        private float3 m_PrevCameraDirection;
        private float4 m_PrevLodParameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreventCullingSystem"/> class.
        /// </summary>
        public PreventCullingSystem()
        {
        }

        /// <inheritdoc/>
        protected override void OnCreate()
        {
            m_Log = AnarchyMod.Instance.Logger;
            m_Log.Info($"{nameof(PreventCullingSystem)} Created.");
            m_ToolOutputBarrier = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<ToolOutputBarrier>();
            m_SimulationSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<SimulationSystem>();
            m_CameraUpdateSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<CameraUpdateSystem>();
            m_BatchDataSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<BatchDataSystem>();
            m_RenderingSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<RenderingSystem>();
            m_CullingInfoQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
               {
                    ComponentType.ReadOnly<PreventOverride>(),
                    ComponentType.ReadOnly<CullingInfo>(),
               },
            });
            RequireForUpdate(m_CullingInfoQuery);

            m_PrevCameraDirection = math.forward();
            m_PrevLodParameters = 1f;
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

            if (!AnarchyMod.Settings.PreventAccidentalPropCulling)
            {
                return;
            }

            __TypeHandle.__CullingInfo_RO_ComponentTypeHandle.Update(ref CheckedStateRef);
            __TypeHandle.__Unity_Entities_Entity_TypeHandle.Update(ref CheckedStateRef);
            __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle.Update(ref CheckedStateRef);

            float3 cameraPosition = m_PrevCameraPosition;
            float3 cameraDirection = m_PrevCameraDirection;
            float4 currentLodParameters = m_PrevLodParameters;
            if (m_CameraUpdateSystem.TryGetLODParameters(out LODParameters lodParameters))
            {
                cameraPosition = lodParameters.cameraPosition;
                IGameCameraController activeCameraController = m_CameraUpdateSystem.activeCameraController;
                currentLodParameters = RenderingUtils.CalculateLodParameters(m_BatchDataSystem.GetLevelOfDetail(m_RenderingSystem.frameLod, activeCameraController), lodParameters);
                cameraDirection = m_CameraUpdateSystem.activeViewer.forward;
            }

            PreventCullingJob preventCullingJob = new ()
            {
                m_CullingInfoType = __TypeHandle.__CullingInfo_RO_ComponentTypeHandle,
                m_EntityType = __TypeHandle.__Unity_Entities_Entity_TypeHandle,
                m_TransformType = __TypeHandle.__Game_Objects_Transform_RO_ComponentTypeHandle,
                buffer = m_ToolOutputBarrier.CreateCommandBuffer().AsParallelWriter(),
                m_CameraDirection = cameraDirection,
                m_CameraPosition = cameraPosition,
                m_LodParameters = currentLodParameters,
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
            [ReadOnly]
            public ComponentTypeHandle<Game.Objects.Transform> __Game_Objects_Transform_RO_ComponentTypeHandle;

            public void __AssignHandles(ref SystemState state)
            {
                __Unity_Entities_Entity_TypeHandle = state.GetEntityTypeHandle();
                __CullingInfo_RO_ComponentTypeHandle = state.GetComponentTypeHandle<CullingInfo>();
                __Game_Objects_Transform_RO_ComponentTypeHandle = state.GetComponentTypeHandle<Game.Objects.Transform>();
            }
        }

        private struct PreventCullingJob : IJobChunk
        {
            [ReadOnly]
            public EntityTypeHandle m_EntityType;
            [ReadOnly]
            public ComponentTypeHandle<CullingInfo> m_CullingInfoType;
            [ReadOnly]
            public ComponentTypeHandle<Game.Objects.Transform> m_TransformType;
            public EntityCommandBuffer.ParallelWriter buffer;
            [ReadOnly]
            public float4 m_LodParameters;
            [ReadOnly]
            public float3 m_CameraPosition;
            [ReadOnly]
            public float3 m_CameraDirection;

            public void Execute(in ArchetypeChunk chunk, int unfilteredChunkIndex, bool useEnabledMask, in v128 chunkEnabledMask)
            {
                NativeArray<Entity> entityNativeArray = chunk.GetNativeArray(m_EntityType);
                NativeArray<CullingInfo> cullingInfoNativeArray = chunk.GetNativeArray(ref m_CullingInfoType);
                NativeArray<Game.Objects.Transform> transformNativeArray = chunk.GetNativeArray(ref m_TransformType);
                for (int i = 0; i < chunk.Count; i++)
                {
                    Entity currentEntity = entityNativeArray[i];
                    CullingInfo currentCullingInfo = cullingInfoNativeArray[i];
                    Game.Objects.Transform currentTransform = transformNativeArray[i];
                    if (currentCullingInfo.m_PassedCulling != 0)
                    {
                        return;
                    }

                    currentCullingInfo.m_Bounds = new Bounds3(currentTransform.m_Position - currentCullingInfo.m_Radius, currentTransform.m_Position + currentCullingInfo.m_Radius);
                    float num2 = math.max(0f, RenderingUtils.CalculateMinDistance(currentCullingInfo.m_Bounds, m_CameraPosition, m_CameraDirection, m_LodParameters) - 277.777771f);
                    if (RenderingUtils.CalculateLod(num2 * num2, m_LodParameters) >= currentCullingInfo.m_MinLod)
                    {
                        currentCullingInfo.m_Bounds = new Bounds3(currentTransform.m_Position - currentCullingInfo.m_Radius, currentTransform.m_Position + currentCullingInfo.m_Radius);
                        float num3 = RenderingUtils.CalculateMinDistance(currentCullingInfo.m_Bounds, m_CameraPosition, m_CameraDirection, m_LodParameters);
                        if (RenderingUtils.CalculateLod(num3 * num3, m_LodParameters) >= currentCullingInfo.m_MinLod)
                        {
                            buffer.AddComponent<Updated>(unfilteredChunkIndex, currentEntity);
                        }
                    }
                }
            }
        }
    }
}

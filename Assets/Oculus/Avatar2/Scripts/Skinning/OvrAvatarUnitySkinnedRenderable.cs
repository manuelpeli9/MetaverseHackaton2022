using System;
using System.Runtime.InteropServices;

using Oculus.Avatar2;

using UnityEngine;

/// @file OvrAvatarUnitySkinnedRenderable.cs

namespace Oculus.Skinning
{
    /**
     * Component that encapsulates the meshes of a skinned avatar.
     * This component implements skinning using Unity.
     * It is used when the skinning configuration is set
     * to *SkinningConfig.UNITY*.
     *
     * @see OvrAvatarSkinnedRenderable
     * @see OvrAvatarGpuSkinnedRenderable
     * @see OvrAvatarEntity.SkinningConfig
     */
    public class OvrAvatarUnitySkinnedRenderable : OvrAvatarSkinnedRenderable
    {
        [SerializeField]
        [Tooltip("Configuration to override SkinQuality, otherwise indicates which Quality was selected for this LOD")]
        private SkinQuality _skinQuality = SkinQuality.Auto;

        public SkinQuality SkinQuality
        {
            get => _skinQuality;
            set
            {
                if (_skinQuality != value)
                {
                    _skinQuality = value;
                    UpdateSkinQuality();
                }
            }
        }
        private void UpdateSkinQuality()
        {
            if (_skinnedRenderer != null)
            {
                _skinnedRenderer.quality = _skinQuality;
            }
        }

        private SkinnedMeshRenderer _skinnedRenderer;

        private float[] _morphBuffer;
        private uint _morphCount;

        private float[] MorphBuffer
        {
            get
            {
                return _morphBuffer ??
                   (_morphBuffer = _morphCount > 0
                        ? new float[(int)_morphCount]
                        : Array.Empty<float>());
            }
        }
        protected void OnDisable()
        {
            _morphBuffer = null;

            _bufferHandle.Dispose();
        }

        protected override void AddDefaultRenderer()
        {
            _skinnedRenderer = AddRenderer<SkinnedMeshRenderer>();
        }

        protected internal override void ApplyMeshPrimitive(OvrAvatarPrimitive primitive)
        {
            base.ApplyMeshPrimitive(primitive);

            _skinnedRenderer.sharedMesh = MyMesh;
            _skinnedRenderer.localBounds = primitive.hasBounds ? primitive.mesh.bounds : FixedBounds;

            if (_skinQuality == SkinQuality.Auto)
            {
                _skinQuality = QualityForLODIndex(primitive.HighestQualityLODIndex);
            }
            _skinnedRenderer.quality = _skinQuality;

            _morphCount = primitive.morphTargetCount;
        }

        public override void ApplySkeleton(Transform[] bones)
        {
            if (_skinnedRenderer.sharedMesh)
            {
                _skinnedRenderer.rootBone = transform;
                _skinnedRenderer.bones = bones;
            }
            else
            {
                OvrAvatarLog.LogError("Had no shared mesh to apply skeleton to!");
            }
        }

        public override IDisposableBuffer CheckoutMorphTargetBuffer(uint morphCount)
        {
            _bufferHandle.SetMorphBuffer(MorphBuffer);
            return _bufferHandle;
        }

        public override void MorphTargetBufferUpdated(IDisposableBuffer buffer)
        {
            Debug.Assert(_bufferHandle.BufferPtr == buffer.BufferPtr);
            for (int morphTargetIndex = 0; morphTargetIndex < _morphBuffer.Length; ++morphTargetIndex)
            {
                _skinnedRenderer.SetBlendShapeWeight(morphTargetIndex, _morphBuffer[morphTargetIndex]);
            }
        }

        public override bool UpdateJointMatrices(CAPI.ovrAvatar2EntityId entityId, OvrAvatarPrimitive primitive, CAPI.ovrAvatar2PrimitiveRenderInstanceID primitiveInstanceId)
        {
            // No-op
            // TODO: Update transforms here
            return false;
        }

        private static SkinQuality QualityForLODIndex(uint lodIndex)
        {
            return OvrAvatarManager.Instance.GetUnitySkinQualityForLODIndex(lodIndex);
        }

        protected override void Dispose(bool isMainThread)
        {
            _skinnedRenderer = null;
            _morphBuffer = null;
            _bufferHandle.Dispose();

            base.Dispose(isMainThread);
        }

        // TODO: This is disposed via the `Cleanup` method
#pragma warning disable CA2213 // Disposable fields should be disposed
        private readonly BufferHandle _bufferHandle = new BufferHandle();
#pragma warning restore CA2213 // Disposable fields should be disposed

        private sealed class BufferHandle : IDisposableBuffer
        {
            private GCHandle _morphHandle;

            public void SetMorphBuffer(float[] buffer)
            {
                Debug.Assert(!_morphHandle.IsAllocated);

                _morphHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            }

            public IntPtr BufferPtr => _morphHandle.AddrOfPinnedObject();

            public void Dispose()
            {
                if (_morphHandle.IsAllocated)
                {
                    _morphHandle.Free();
                }
            }
        }

        // TODO: FixedBounds should definitely be removed ASAP
        private static Bounds FixedBounds
            => new Bounds(new Vector3(0f, 0.5f, 0.0f), new Vector3(2.0f, 2.0f, 2.0f));

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            UpdateSkinQuality();
        }
#endif
    }
}

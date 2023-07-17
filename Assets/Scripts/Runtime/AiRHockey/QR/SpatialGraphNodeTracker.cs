using Microsoft.MixedReality.OpenXR;
using UnityEngine;
using HTW.AiRHockey.Game;

namespace HTW.AiRHockey.QR
{
    public class SpatialGraphNodeTracker : MonoBehaviour
    {
		#region attributes

		private SpatialGraphNode node;

        private System.Guid _id;
        public System.Guid Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    InitializeSpatialGraphNode();
                }
            }
        }

        private float _physicalSideLength;
        public float PhysicalSideLength
		{
            get => _physicalSideLength;
            set => _physicalSideLength = value;
		}

		#endregion

		#region lifecycle

		private void Start()
        {
            InitializeSpatialGraphNode();
        }

        private void Update()
        {
            if (node == null || node.Id != Id)
            {
                node = (Id != System.Guid.Empty) ? SpatialGraphNode.FromStaticNodeId(Id) : null;
                Debug.Log($"Initialize SpatialGraphNode Id={Id} Node={node}");
            }

            if (node != null)
            {
                if (node.TryLocate(FrameTime.OnUpdate, out Pose pose))
                {
                    if (Camera.main.transform.parent != null)
                        pose = pose.GetTransformedBy(Camera.main.transform.parent);

                    transform.localScale = InstanceFinder.GameSettings.ArenaSizeMultiplier * PhysicalSideLength * Vector3.one;
                    pose.rotation *= Quaternion.Euler(90, 0, 0);
                    float deltaCenter = PhysicalSideLength * 0.5f;
                    pose.position += pose.rotation * (deltaCenter * Vector3.right) + pose.rotation * (1 * Vector3.up) - pose.rotation * (deltaCenter * Vector3.forward);
                    transform.SetPositionAndRotation(pose.position, pose.rotation);
                    Debug.Log($"Id={Id} QRPose={pose.position} QRRot={pose.rotation}");
                }
                else
                {
                    Debug.LogWarning($"Cannot locate {Id}");
                }
            }
        }

		#endregion

		#region private methods

		private void InitializeSpatialGraphNode()
        {
            if (node == null || node.Id != Id)
            {
                node = (Id != System.Guid.Empty) ? SpatialGraphNode.FromStaticNodeId(Id) : null;
                Debug.Log($"Initialize SpatialGraphNode Id={Id}");
            }
        }

		#endregion
	}
}
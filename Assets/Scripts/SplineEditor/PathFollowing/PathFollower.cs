using System;
using Extensions.Other;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SplineEditor.PathFollowing
{
    public class PathFollower : MonoBehaviour
    {
//-------Public Variables-------//
        public bool IsEnabled = false;
        public float Speed;
        [Range(0f, 1f), OnValueChanged(nameof(UpdatePosition))] public float NormalizedPosition;
//------Serialized Fields-------//
        [SerializeField, Required, PropertyOrder(-1)] private CatmullRom Spline;
//------Private Variables-------//


#region UNITY_METHODS

        private void Update()
        {
            if (!IsEnabled)
                return;
            UpdatePosition();
        }

#endregion


#region PUBLIC_METHODS

#endregion


#region PRIVATE_METHODS

        private void UpdatePosition()
        {
            var (targetPos, tangent) = Spline.GetPositionAndTangentFromNormalizedValue(NormalizedPosition);
            transform.position = targetPos;
            transform.rotation = Quaternion.LookRotation(tangent);
        }
#endregion

    }
}



using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SplineEditor.PathFollowing
{
    public class PathFollower : MonoBehaviour
    {
//-------Public Variables-------//
        public bool IsEnabled = false;
        public float Speed;
        public MovementMode MovementMode;
        [Range(0f, 1f), OnValueChanged(nameof(UpdatePosition))] public float NormalizedPosition;
//------Serialized Fields-------//
        [SerializeField, Required, PropertyOrder(-1)] private CatmullRom Spline;
//------Private Variables-------//
        private Action _reachedZero;
        private Action _reachedOne;
        private short _incrementMode = 1;
#region UNITY_METHODS

        private void Awake()
        {
            
        }

        private void Update()
        {
            if (!IsEnabled)
                return;
            CheckNormalizedPosition();
            NormalizedPosition = Mathf.Clamp01(NormalizedPosition + Speed * _incrementMode * Time.deltaTime);
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

        private short SetIncrementModeAtTheEnd()
        {
            if (MovementMode == MovementMode.PingPong)
                return -1;
            if (MovementMode == MovementMode.Default) IsEnabled = false;
            else if (MovementMode == MovementMode.ForwardLoop) NormalizedPosition = 0f;
            return 1;
        }

        private short SetIncrementModeAtTheStart()
        {
            return 1;
        }

        private void CheckNormalizedPosition()
        {
            if (NormalizedPosition == 0f)
                _incrementMode = SetIncrementModeAtTheStart();
            else if (NormalizedPosition == 1f) _incrementMode = SetIncrementModeAtTheEnd();
        }
#endregion

    }
}



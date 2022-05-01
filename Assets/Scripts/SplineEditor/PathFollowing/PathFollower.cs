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
        [Range(0f, 1f), OnValueChanged(nameof(UpdatePositionWithNormalizedValue)),
         ShowIf(nameof(PositionerMode), PositionerMode.Normalized)]
        public float NormalizedPosition;
        [MinValue(0f), OnValueChanged(nameof(UpdatePositionWithDistance)),
         ShowIf(nameof(PositionerMode), PositionerMode.Distance)]
        public float Distance;
//------Serialized Fields-------//
        [SerializeField, Required, PropertyOrder(-1)] private CatmullRom Spline;
        [SerializeField] private PositionerMode PositionerMode;
        [SerializeField] private MovementMode MovementMode;
//------Private Variables-------//
        private Action _reachedZero;
        private Action _reachedOne;
        private short _incrementMode = 1;
//------Debug------//
        [SerializeField, ReadOnly, TabGroup("Debug")] private float EstimatedFinishTime;
#region UNITY_METHODS
        private void Update()
        {
            if (!IsEnabled)
                return;
            if (PositionerMode == PositionerMode.Normalized)
            {
                CheckNormalizedPosition();
                NormalizedPosition = Mathf.Clamp01(NormalizedPosition + Speed * _incrementMode * Time.deltaTime);
                UpdatePositionWithNormalizedValue();
            }
            else if (PositionerMode == PositionerMode.Distance)
            {
                CheckDistance();
                Distance = Mathf.Clamp(Distance + Speed * _incrementMode * Time.deltaTime, 0f, Spline.TotalLength);
                UpdatePositionWithDistance();
            }
        }

#endregion


#region PUBLIC_METHODS

#endregion


#region PRIVATE_METHODS

#region Normalized Value Functions

        [Button(ButtonSizes.Large), TabGroup("Debug")]
        private void CalculateEstimatedFinishTime()
        {
            if(PositionerMode == PositionerMode.Distance)
                EstimatedFinishTime = Spline.TotalLength / Speed;
            else if (PositionerMode == PositionerMode.Normalized)
                EstimatedFinishTime = 1f / Speed;
        }
        
        private void UpdatePositionWithNormalizedValue()
        {
            var (targetPos, tangent) = Spline.GetPositionAndTangentFromNormalizedValue(NormalizedPosition);
            transform.position = targetPos;
            transform.rotation = Quaternion.LookRotation(tangent);
        }

        private short SetIncrementModeAtTheEnd()
        {
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (MovementMode == MovementMode.PingPong)
                return -1;
            if (MovementMode == MovementMode.Default) IsEnabled = false;
            else if (MovementMode == MovementMode.ForwardLoop)
            {
                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (PositionerMode == PositionerMode.Distance)
                    Distance = 0f;
                else if (PositionerMode == PositionerMode.Normalized) 
                    NormalizedPosition = 0f;
            }
            return 1;
        }

        private static short SetIncrementModeAtTheStart()
        {
            return 1;
        }

        private void CheckNormalizedPosition()
        {
            _incrementMode = NormalizedPosition switch
            {
                0f => SetIncrementModeAtTheStart(),
                1f => SetIncrementModeAtTheEnd(),
                _ => _incrementMode
            };
        }

        private void CheckDistance()
        {
            if (Distance == 0f)
                _incrementMode = SetIncrementModeAtTheStart();
            else if (Distance >= Spline.TotalLength)
                _incrementMode = SetIncrementModeAtTheEnd();
        }

#endregion
        

        #region Distance Functions
        private void UpdatePositionWithDistance()
        {
            var (targetPos, tangent) = Spline.GetPositionAndTangentFromDistance(Distance);
            transform.position = targetPos;
            transform.rotation = Quaternion.LookRotation(tangent);
        }
        #endregion
#endregion

    }
}



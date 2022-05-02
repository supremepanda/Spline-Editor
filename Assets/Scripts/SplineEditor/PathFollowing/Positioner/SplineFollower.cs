using Sirenix.OdinInspector;
using UnityEngine;

namespace SplineEditor.PathFollowing.Positioner
{
    public class SplineFollower : PositionerBase
    {
//-------Public Variables-------//
        public const short INCREMENT_FORWARD = 1;
        public const short INCREMENT_BACKWARD = -1;
        public short IncrementMode => _incrementMode;
        public Collider GetEventTriggerCollider => EventTriggerCollider;
        public bool IsEnabled = false;
        public float Speed;

//------Serialized Fields-------//
        [SerializeField] private MovementMode MovementMode;
        [SerializeField, Required] private Collider EventTriggerCollider;

//------Private Variables-------//
        private short _incrementMode = INCREMENT_FORWARD;
//------Debug------//
        [SerializeField, ReadOnly, TabGroup("Debug")] private float EstimatedFinishTime;

#region UNITY_METHODS
        private void Update()
        {
            if (!IsEnabled)
                return;
            CheckActionsForPositionerMode();
        }

#endregion


#region PUBLIC_METHODS

#endregion


#region PRIVATE_METHODS
        
        [Button(ButtonSizes.Large), TabGroup("Debug")]
        private void CalculateEstimatedFinishTime()
        {
            if(PositionerMode == PositionerMode.Distance)
                EstimatedFinishTime = Spline.TotalLength / Speed;
            else if (PositionerMode == PositionerMode.Normalized)
                EstimatedFinishTime = 1f / Speed;
        }
        private void CheckActionsForPositionerMode()
        {
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
        
        private short SetIncrementModeAtTheEnd()
        {
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (MovementMode == MovementMode.PingPong)
                return INCREMENT_BACKWARD;
            if (MovementMode == MovementMode.Default) IsEnabled = false;
            else if (MovementMode == MovementMode.ForwardLoop)
            {
                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (PositionerMode == PositionerMode.Distance)
                    Distance = 0f;
                else if (PositionerMode == PositionerMode.Normalized) 
                    NormalizedPosition = 0f;
            }
            return INCREMENT_FORWARD;
        }

        private static short SetIncrementModeAtTheStart()
        {
            return INCREMENT_FORWARD;
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

    }
}



using System;
using Sirenix.OdinInspector;
using SplineEditor.Controller.SplineController;
using SplineEditor.Events;
using SplineEditor.PathFollowing.Positioner.Base;
using SplineEditor.PathFollowing.Positioner.Modes;
using UnityEngine;

namespace SplineEditor.PathFollowing.Positioner
{
    public class SplineFollower : PositionerBaseWithXPos
    {
//-------Public Variables-------//
        public static Action<SplineFollower, float, short> OnPositionChanged;

        public const short INCREMENT_FORWARD = 1;
        public const short INCREMENT_BACKWARD = -1;
        public bool IsEnabled = false;
        public float Speed;

//------Serialized Fields-------//
        [SerializeField] private MovementMode MovementMode;
//------Private Variables-------//
        private short _incrementMode = INCREMENT_FORWARD;
//------Debug------//

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

        protected override void UpdatePositionWithDistance()
        {
            base.UpdatePositionWithDistance();
            OnPositionChanged?.Invoke(this, GetNormalizedPosition(), _incrementMode);
        }

        protected override void UpdatePositionWithNormalizedValue()
        {
            base.UpdatePositionWithNormalizedValue();
            OnPositionChanged?.Invoke(this, GetNormalizedPosition(), _incrementMode);
        }

        [Button(ButtonSizes.Large), TabGroup("Debug")]
        private float CalculateEstimatedFinishTime()
        {
            var estimatedFinishTime = PositionerMode switch
            {
                PositionerMode.Distance => Spline.TotalLength / Speed,
                PositionerMode.Normalized => 1f / Speed,
                _ => 0f
            };
            return estimatedFinishTime;
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

        private short SetIncrementModeAtTheStart()
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
            CheckRaisableEvents();
        }

        private void CheckDistance()
        {
            if (Distance == 0f)
                _incrementMode = SetIncrementModeAtTheStart();
            else if (Distance >= Spline.TotalLength)
                _incrementMode = SetIncrementModeAtTheEnd();
            CheckRaisableEvents();
        }

        private void CheckRaisableEvents()
        {
            Spline.TryGetComponent(out SplineController controller);
            if (controller == null)
                return;
            foreach (var eventPoint in controller.GetEventPoints)
            {
                eventPoint.TryGetComponent(out SplineEvent splineEvent);
                if(splineEvent == null)
                    continue;
                if (!IsEventAccessible(splineEvent))
                    splineEvent.ActiveFollowers.Remove(this);
                else if (!splineEvent.ActiveFollowers.Contains(this))
                    splineEvent.ActiveFollowers.Add(this);
            }
        }

        private bool IsEventAccessible(SplineEvent splineEvent)
        {
            return CheckEventForDirection(splineEvent) && CheckEventForPosition(splineEvent);
        }

        private bool CheckEventForDirection(SplineEvent splineEvent)
        {
            switch (splineEvent.GetEventTriggerMode)
            {
                case EventTriggerMode.OnlyForward when _incrementMode != INCREMENT_FORWARD:
                case EventTriggerMode.OnlyBackward when _incrementMode != INCREMENT_BACKWARD:
                    return false;
                case EventTriggerMode.TwoSided:
                default:
                    return true;
            }
        }

        private bool CheckEventForPosition(SplineEvent splineEvent)
        {
            var eventNormalizedValue = splineEvent.GetNormalizedPosition();
            var followerNormalizedValue = GetNormalizedPosition();
            switch (_incrementMode)
            {
                case INCREMENT_FORWARD when eventNormalizedValue < followerNormalizedValue:
                case INCREMENT_BACKWARD when eventNormalizedValue > followerNormalizedValue:
                    return false;
                default:
                    return true;
            }
        }
        
#endregion

    }
}



using Sirenix.OdinInspector;
using UnityEngine;

namespace SplineEditor.PathFollowing.Positioner
{
    public class SplineFollower : PositionerBase
    {
//-------Public Variables-------//
        public bool IsEnabled = false;
        public float Speed;

//------Serialized Fields-------//
        [SerializeField] private MovementMode MovementMode;


//------Private Variables-------//

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
                NormalizedPosition = Mathf.Clamp01(NormalizedPosition + Speed * IncrementMode * Time.deltaTime);
                UpdatePositionWithNormalizedValue();
            }
            else if (PositionerMode == PositionerMode.Distance)
            {
                CheckDistance();
                Distance = Mathf.Clamp(Distance + Speed * IncrementMode * Time.deltaTime, 0f, Spline.TotalLength);
                UpdatePositionWithDistance();
            }
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
            IncrementMode = NormalizedPosition switch
            {
                0f => SetIncrementModeAtTheStart(),
                1f => SetIncrementModeAtTheEnd(),
                _ => IncrementMode
            };
        }

        private void CheckDistance()
        {
            if (Distance == 0f)
                IncrementMode = SetIncrementModeAtTheStart();
            else if (Distance >= Spline.TotalLength)
                IncrementMode = SetIncrementModeAtTheEnd();
        }
#endregion

    }
}



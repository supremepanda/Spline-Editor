using Sirenix.OdinInspector;
using SplineEditor.PathFollowing.Positioner.Base.Base;
using SplineEditor.PathFollowing.Positioner.Modes;
using UnityEngine;

namespace SplineEditor.PathFollowing.Positioner.Base
{
    public class PositionerBaseWithXPos : PositionerBase
    {
//-------Public Variables-------//


//------Serialized Fields-------//
        [SerializeField, OnValueChanged(nameof(UpdateXPosition))]
        protected float XPosition = 0f;

//------Private Variables-------//


#region UNITY_METHODS


#endregion


#region PUBLIC_METHODS

#endregion


#region PRIVATE_METHODS

        protected override void UpdatePositionWithNormalizedValue()
        {
            var (targetPos, tangent) = Spline.GetPositionAndTangentFromNormalizedValue(NormalizedPosition,
                XPosition);
            transform.position = targetPos;
            transform.rotation = Quaternion.LookRotation(tangent);
        }

        protected override void UpdatePositionWithDistance()
        {
            var (targetPos, tangent) = Spline.GetPositionAndTangentFromDistance(Distance, XPosition);
            transform.position = targetPos;
            transform.rotation = Quaternion.LookRotation(tangent);
        }

        private void UpdateXPosition()
        {
            if (PositionerMode == PositionerMode.Distance)
                UpdatePositionWithDistance();
            else if (PositionerMode == PositionerMode.Normalized) UpdatePositionWithNormalizedValue();
        }
#endregion

    }
}



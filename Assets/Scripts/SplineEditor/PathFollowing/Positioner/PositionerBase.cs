using Sirenix.OdinInspector;
using UnityEngine;

namespace SplineEditor.PathFollowing.Positioner
{
    public abstract class PositionerBase : MonoBehaviour
    {
//-------Public Variables-------//
        [Range(0f, 1f), OnValueChanged(nameof(UpdatePositionWithNormalizedValue)),
         ShowIf(nameof(PositionerMode), PositionerMode.Normalized)]
        public float NormalizedPosition;
        [MinValue(0f), OnValueChanged(nameof(UpdatePositionWithDistance)),
         ShowIf(nameof(PositionerMode), PositionerMode.Distance)]
        public float Distance;
//------Serialized Fields-------//
        [SerializeField, Required, PropertyOrder(-1)] protected CatmullRom Spline;
        [SerializeField] protected PositionerMode PositionerMode;
//------Private Variables-------//
        protected short IncrementMode = 1;

#region UNITY_METHODS

#endregion


#region PUBLIC_METHODS

#endregion


#region PRIVATE_METHODS

        
#region Normalized Value Functions
        protected void UpdatePositionWithNormalizedValue()
        {
            var (targetPos, tangent) = Spline.GetPositionAndTangentFromNormalizedValue(NormalizedPosition);
            transform.position = targetPos;
            transform.rotation = Quaternion.LookRotation(tangent);
        }
#endregion
        
#region Distance Functions
        protected void UpdatePositionWithDistance()
        {
            var (targetPos, tangent) = Spline.GetPositionAndTangentFromDistance(Distance);
            transform.position = targetPos;
            transform.rotation = Quaternion.LookRotation(tangent);
        }
        #endregion
#endregion

    }
}



using System;
using Sirenix.OdinInspector;
using SplineEditor.Controller;
using SplineEditor.Controller.CatmullRomCalc;
using SplineEditor.PathFollowing.Positioner.Modes;
using UnityEngine;

namespace SplineEditor.PathFollowing.Positioner.Base.Base
{
    public abstract class PositionerBase : MonoBehaviour
    {
//-------Public Variables-------//
        public PositionerMode GetPositionerMode => PositionerMode;
        [Range(0f, 1f), OnValueChanged(nameof(UpdatePositionWithNormalizedValue)),
         ShowIf(nameof(PositionerMode), PositionerMode.Normalized)]
        public float NormalizedPosition;
        [MinValue(0f), OnValueChanged(nameof(UpdatePositionWithDistance)),
         ShowIf(nameof(PositionerMode), PositionerMode.Distance)]
        public float Distance;
//------Serialized Fields-------//
        [SerializeField, Required, PropertyOrder(-1)] protected CatmullRom Spline;
        [SerializeField, OnValueChanged(nameof(UpdateOnPositionerModeChanged))] 
        protected PositionerMode PositionerMode;
        [SerializeField] protected bool IsMover = true;
        [SerializeField, HideIf(nameof(IsMover))]
        protected Transform MoverTransform;
        [SerializeField, OnValueChanged(nameof(UpdatePosition))] protected float _yPosition = 0f;
//------Private Variables-------//

#region UNITY_METHODS

#endregion


#region PUBLIC_METHODS

        public float GetNormalizedPosition()
        {
            return PositionerMode switch
            {
                PositionerMode.Normalized => NormalizedPosition,
                PositionerMode.Distance => Spline.CalculateNormalizedValueUsingDistance(Distance),
                _ => 0f
            };
        }
        
        public void SetSpline(CatmullRom spline)
        {
            Spline = spline;
        }
#endregion


#region PRIVATE_METHODS

        
#region Normalized Value Functions

        private void UpdateOnPositionerModeChanged()
        {
            if (PositionerMode == PositionerMode.Distance)
                UpdatePositionWithDistance();
            else if (PositionerMode == PositionerMode.Normalized)
                UpdatePositionWithNormalizedValue();
        }
        
        protected virtual void UpdatePositionWithNormalizedValue()
        {
            var (targetPos, tangent) = Spline.GetPositionAndTangentFromNormalizedValue(NormalizedPosition,
                0f, _yPosition);
            if (IsMover)
            {
                transform.position = targetPos;
                transform.rotation = Quaternion.LookRotation(tangent);
            }
            else
            {
                MoverTransform.position = targetPos;
                MoverTransform.rotation = Quaternion.LookRotation(tangent);
            }
        }
#endregion
        
#region Distance Functions
        protected virtual void UpdatePositionWithDistance()
        {
            var (targetPos, tangent) = Spline.GetPositionAndTangentFromDistance(Distance, 0f, _yPosition);
            if (IsMover)
            {
                transform.position = targetPos;
                transform.rotation = Quaternion.LookRotation(tangent);
            }
            else
            {
                MoverTransform.position = targetPos;
                transform.rotation = Quaternion.LookRotation(tangent);
            }
        }
        
#endregion

        protected void UpdatePosition()
        {
            if (PositionerMode == PositionerMode.Distance)
                UpdatePositionWithDistance();
            else if (PositionerMode == PositionerMode.Normalized) UpdatePositionWithNormalizedValue();
        }
        
#endregion

    }
}



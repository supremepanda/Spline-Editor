using System.Collections.Generic;
using Sirenix.OdinInspector;
using SplineEditor.PathFollowing.Positioner;
using SplineEditor.PathFollowing.Positioner.Base.Base;
using UnityEngine;
using UnityEngine.Events;

namespace SplineEditor.Events
{
    public class SplineEvent : PositionerBase
    {
//-------Public Variables-------//
        public EventTriggerMode GetEventTriggerMode => EventTriggerMode;
        [ReadOnly] public List<SplineFollower> ActiveFollowers;
//------Serialized Fields-------//
        [SerializeField, PropertyOrder(999)] private UnityEvent<SplineFollower> Events;
        [SerializeField] private EventTriggerMode EventTriggerMode;
//------Private Variables-------//

#region UNITY_METHODS

        private void Awake()
        {
            SplineFollower.OnPositionChanged += CheckEventToRaise;
        }

        private void OnDestroy()
        {
            SplineFollower.OnPositionChanged -= CheckEventToRaise;
        }

#endregion


#region PUBLIC_METHODS
        
        [Button]
        public void Raise(SplineFollower follower, bool debugMode)
        {
            if (!debugMode) 
                ActiveFollowers.Remove(follower);
            Events?.Invoke(follower);
        }
#endregion


#region PRIVATE_METHODS

        private void CheckEventToRaise(SplineFollower follower, float normalizedValue, short incrementMode)
        {
            if (!ActiveFollowers.Contains(follower))
                return;
            var eventNormalizedPosition = GetNormalizedPosition();
            switch (incrementMode)
            {
                case SplineFollower.INCREMENT_FORWARD:
                {
                    if(eventNormalizedPosition <= normalizedValue)
                        Raise(follower, false);
                    break;
                }
                case SplineFollower.INCREMENT_BACKWARD:
                {
                    if(eventNormalizedPosition >= normalizedValue)
                        Raise(follower, false);
                    break;
                }
            }
        }
#endregion

    }
}



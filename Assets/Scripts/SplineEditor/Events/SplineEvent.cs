using Sirenix.OdinInspector;
using SplineEditor.PathFollowing.Positioner;
using UnityEngine;
using UnityEngine.Events;

namespace SplineEditor.Events
{
    public class SplineEvent : PositionerBase
    {
//-------Public Variables-------//


//------Serialized Fields-------//
        [SerializeField, PropertyOrder(999)] private UnityEvent<SplineFollower> Events;
        [SerializeField] private EventTriggerMode EventTriggerMode;
//------Private Variables-------//

#region UNITY_METHODS

#endregion


#region PUBLIC_METHODS

        private void OnTriggerEnter(Collider other)
        {
            other.TryGetComponent(out SplineFollower follower);
            if (follower == null)
                return;
            if (other != follower.GetEventTriggerCollider)
                return;
            Raise(follower);
        }

        [Button]
        public void Raise(SplineFollower follower)
        {
            if (!CheckEventTriggerMode(follower))
                return;
            Events?.Invoke(follower);
        }
#endregion


#region PRIVATE_METHODS

        private bool CheckEventTriggerMode(SplineFollower follower)
        {
            var currentIncrementMode = follower.IncrementMode;
            return EventTriggerMode switch
            {
                EventTriggerMode.OnlyForward => currentIncrementMode == SplineFollower.INCREMENT_FORWARD,
                EventTriggerMode.OnlyBackward => currentIncrementMode == SplineFollower.INCREMENT_BACKWARD,
                EventTriggerMode.TwoSided => true,
                _ => true
            };
        }
#endregion

    }
}



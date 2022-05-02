using Sirenix.OdinInspector;
using UnityEngine;

namespace SplineEditor.Removable
{
    [ExecuteAlways]
    public abstract class RemovableComponentBase : MonoBehaviour
    {
//-------Public Variables-------//


//------Serialized Fields-------//


//------Private Variables-------//
        protected SplineController SplineController;
        protected bool IsDestroyed = false;
        protected Transform Transform;
#region UNITY_METHODS

        protected virtual void OnEnable()
        {
            Transform = transform;
            if (transform.root == null)
                return;
            transform.root.TryGetComponent(out SplineController creator);
            SplineController = creator;
        }
        
        protected virtual void OnDestroy()
        {
            if (IsDestroyed)
                return;
            RemoveWithoutDestroy();
        }

#endregion


#region PUBLIC_METHODS

#endregion


#region PRIVATE_METHODS

        [Button(ButtonSizes.Large), GUIColor(.8f, .2f, .2f)]
        protected virtual void Remove()
        {
            if (SplineController == null)
                return;
            IsDestroyed = true;
        }

        protected virtual void RemoveWithoutDestroy()
        {
            if (SplineController == null)
                return;
            IsDestroyed = true;
        }
#endregion

    }
}



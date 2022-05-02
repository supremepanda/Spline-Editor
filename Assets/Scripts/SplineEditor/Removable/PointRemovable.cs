using UnityEngine;

namespace SplineEditor.Removable
{
    public class PointRemovable : RemovableComponentBase
    {
//-------Public Variables-------//


//------Serialized Fields-------//
        [SerializeField] private GameObject Renderer;

//------Private Variables-------//
#region UNITY_METHODS

        private void Start()
        {
            if (!Application.isPlaying)
                return;
            Destroy(Renderer);
        }

#endregion


#region PUBLIC_METHODS

#endregion


#region PRIVATE_METHODS

        protected override void Remove()
        {
            base.Remove();
            SplineController.RemovePoint(Transform, true);
        }

        protected override void RemoveWithoutDestroy()
        {
            base.RemoveWithoutDestroy();
            SplineController.RemovePoint(Transform, false);
        }

#endregion

    }
}



namespace SplineEditor.Removable
{
    public class SplineEventRemovable : RemovableComponentBase
    {
//-------Public Variables-------//


//------Serialized Fields-------//


//------Private Variables-------//


#region UNITY_METHODS
#endregion


#region PUBLIC_METHODS

#endregion


#region PRIVATE_METHODS

        protected override void Remove()
        {
            base.Remove();
            SplineController.RemoveEvent(Transform, true);
        }

        protected override void RemoveWithoutDestroy()
        {
            base.RemoveWithoutDestroy();
            SplineController.RemoveEvent(Transform, false);
        }

#endregion

    }
}



using Sirenix.OdinInspector;
using UnityEngine;

namespace SplineEditor
{
    [ExecuteAlways]
    public class Point : MonoBehaviour
    {
//-------Public Variables-------//


//------Serialized Fields-------//
        

//------Private Variables-------//
        private SplineCreator _splineCreator;
        private bool _isDestroyed = false;
        private Transform _transform;
#region UNITY_METHODS

        private void OnEnable()
        {
            _transform = transform;
        }

        private void OnDestroy()
        {
            if (_isDestroyed)
                return;
            RemovePointWithoutDestroyEnabled();
        }

#endregion


#region PUBLIC_METHODS

        public void SetSplineCreator(SplineCreator creator)
        {
            _splineCreator = creator;
        }
#endregion


#region PRIVATE_METHODS

        [Button(ButtonSizes.Large), GUIColor(.8f, .2f, .2f)]
        private void RemovePoint()
        {
            _isDestroyed = true;
            _splineCreator.RemovePoint(_transform, true);
        }

        private void RemovePointWithoutDestroyEnabled()
        {
            _isDestroyed = true;
            _splineCreator.RemovePoint(_transform, false);
        }
#endregion

    }
}



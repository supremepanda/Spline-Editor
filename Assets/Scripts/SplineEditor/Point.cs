using System;
using Extensions.Other;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SplineEditor
{
    [ExecuteAlways]
    public class Point : MonoBehaviour
    {
//-------Public Variables-------//


//------Serialized Fields-------//
        [SerializeField] private GameObject Renderer;

//------Private Variables-------//
        private SplineController _splineController;
        private bool _isDestroyed = false;
        private Transform _transform;
#region UNITY_METHODS

        private void Start()
        {
            if (!Application.isPlaying)
                return;
            Destroy(Renderer);
        }

        private void OnEnable()
        {
            _transform = transform;
            if (transform.parent == null)
                return;
            transform.parent.TryGetComponent(out SplineController creator);
            _splineController = creator;
        }

        private void OnDestroy()
        {
            if (_isDestroyed)
                return;
            RemovePointWithoutDestroyEnabled();
        }

#endregion


#region PUBLIC_METHODS

#endregion


#region PRIVATE_METHODS

        [Button(ButtonSizes.Large), GUIColor(.8f, .2f, .2f)]
        private void RemovePoint()
        {
            _isDestroyed = true;
            _splineController.RemovePoint(_transform, true);
        }

        private void RemovePointWithoutDestroyEnabled()
        {
            if (_splineController == null)
                return;
            _isDestroyed = true;
            _splineController.RemovePoint(_transform, false);
        }
#endregion

    }
}



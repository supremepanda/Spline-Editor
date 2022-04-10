using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SplineEditor
{
    [ExecuteInEditMode]
    public class SplineCreator : MonoBehaviour
    {
//-------Public Variables-------//
        public CatmullRom Spline => _spline;
        
//------Serialized Fields-------//
        [SerializeField] private Transform[] ControlPoints;

        [SerializeField, TabGroup("Config")] private UpdateMethod UpdateMethod;
        [SerializeField, Range(2, 25), TabGroup("Config")] private int Resolution;
        [SerializeField, TabGroup("Config")] private bool IsClosedLoop;
        
        [SerializeField, TabGroup("Draw")] private bool DrawNormals;
        [SerializeField, TabGroup("Draw"), ShowIf(nameof(DrawNormals)), Range(0, 20)]
        private float NormalExtrusion = 2f;
        [SerializeField, TabGroup("Draw"), ShowIf(nameof(DrawNormals))]
        private Color NormalColor = Color.red;

        [SerializeField, TabGroup("Draw")] private bool DrawTangent;
        [SerializeField, TabGroup("Draw"), ShowIf(nameof(DrawTangent)), Range(0, 20)]
        private float TangentExtrusion = 2f;
        [SerializeField, TabGroup("Draw"), ShowIf(nameof(DrawTangent))] private Color TangentColor = Color.cyan;
        
        [SerializeField, TabGroup("Draw")] private bool DrawSpline = true;
        [SerializeField, TabGroup("Draw"), ShowIf(nameof(DrawSpline))] private Color SplineColor = Color.white;
//------Private Variables-------//
        private CatmullRom _spline;
#region UNITY_METHODS

        private void Start()
        {
            UpdateSpline();
        }

        private void Update()
        {
#if UNITY_EDITOR
            CheckDrawings();

            if (UpdateMethod != UpdateMethod.OnUpdate)
                return;
            UpdateSpline();
#endif
        }

#endregion


#region PUBLIC_METHODS

        [Button(ButtonSizes.Large), GUIColor(.2f, .8f, .2f), TabGroup("Init")]
        public void InitializeSpline()
        {
            if (_spline == null)
                _spline = new CatmullRom(ControlPoints, Resolution, IsClosedLoop);
        }

        [Button(ButtonSizes.Large), GUIColor(.8f, .2f, .2f), TabGroup("Init")]
        public void RemoveSpline()
        {
            _spline = null;
        }

        [Button(ButtonSizes.Large), ShowIf("UpdateMethod", UpdateMethod.WithMethod)]
        public void UpdateSpline()
        {
            if (_spline != null)
            {
                _spline.UpdateControlPoints(ControlPoints);
                _spline.UpdateResolution(Resolution, IsClosedLoop);
            }
            else
            {
                InitializeSpline();
                UpdateSpline();
            }
        }
#endregion


#region PRIVATE_METHODS

        private void CheckDrawings()
        {
            if (_spline == null)
                return;
            if(DrawSpline)
                _spline.DrawSpline(SplineColor);
            if(DrawNormals)
                _spline.DrawNormals(NormalExtrusion, NormalColor);
            if(DrawTangent)
                _spline.DrawTangents(TangentExtrusion, TangentColor);
        }
#endregion

    }
}



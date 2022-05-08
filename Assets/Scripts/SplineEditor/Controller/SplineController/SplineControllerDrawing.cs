using Sirenix.OdinInspector;
using UnityEngine;

namespace SplineEditor.Controller.SplineController
{
    public partial class SplineController
    {
//-------Public Variables-------//


//------Serialized Fields-------//
        [SerializeField, TabGroup("Draw"), OnValueChanged(nameof(UpdateNormalDrawingConfig))]
        private bool DrawNormals;
        [SerializeField, TabGroup("Draw"), ShowIf(nameof(DrawNormals)), Range(0, 20), OnValueChanged(nameof(UpdateNormalDrawingConfig))]
        private float NormalExtrusion = 2f;
        [SerializeField, TabGroup("Draw"), ShowIf(nameof(DrawNormals)), OnValueChanged(nameof(UpdateNormalDrawingConfig))]
        private Color NormalColor = Color.red;
        [SerializeField, TabGroup("Draw"), ShowIf(nameof(DrawNormals)),
         OnValueChanged(nameof(UpdateNormalDrawingConfig))]
        private float NormalThickness = 1.2f;

        [SerializeField, TabGroup("Draw"), OnValueChanged(nameof(UpdateTangentDrawingConfig))] 
        private bool DrawTangent;
        [SerializeField, TabGroup("Draw"), ShowIf(nameof(DrawTangent)), Range(0, 20), OnValueChanged(nameof(UpdateTangentDrawingConfig))]
        private float TangentExtrusion = 2f;
        [SerializeField, TabGroup("Draw"), ShowIf(nameof(DrawTangent)), OnValueChanged(nameof(UpdateTangentDrawingConfig))]
        private Color TangentColor = Color.cyan;
        [SerializeField, TabGroup("Draw"), ShowIf(nameof(DrawTangent)),
         OnValueChanged(nameof(UpdateTangentDrawingConfig))]
        private float TangentThickness = 1.2f;
        
        [SerializeField, TabGroup("Draw"), OnValueChanged(nameof(UpdateSplineDrawingConfig))] private bool DrawSpline = true;
        [SerializeField, TabGroup("Draw"), ShowIf(nameof(DrawSpline)), OnValueChanged(nameof(UpdateSplineDrawingConfig))]
        private Color SplineColor = Color.white;
        [SerializeField, TabGroup("Draw"), ShowIf(nameof(DrawSpline)),
         OnValueChanged(nameof(UpdateSplineDrawingConfig))]
        private float SplineThickness = 2.5f;

        [SerializeField, TabGroup("Draw"), OnValueChanged(nameof(UpdateSphereDrawingConfig))] private bool DrawSphere = true;
        [SerializeField, TabGroup("Draw"), ShowIf(nameof(DrawSphere)), OnValueChanged(nameof(UpdateSphereDrawingConfig))]
        private Color SphereColor = Color.magenta;
        [SerializeField, TabGroup("Draw"), ShowIf(nameof(DrawSphere)), OnValueChanged(nameof(UpdateSphereDrawingConfig))]
        private float SphereRadius = .22f;

//------Private Variables-------//


#region UNITY_METHODS

#endregion


#region PUBLIC_METHODS

#endregion


#region PRIVATE_METHODS
        private void UpdateSplineDrawingConfig()
        {
            Spline.ActivateDrawSpline(DrawSpline);
            Spline.SetSplineColor(SplineColor);
            Spline.SetSplineThickness(SplineThickness);
        }

        private void UpdateNormalDrawingConfig()
        {
            Spline.ActivateDrawNormal(DrawNormals);
            Spline.SetNormalColor(NormalColor);
            Spline.SetNormalExtrusion(NormalExtrusion);
            Spline.SetNormalThickness(NormalThickness);
        }

        private void UpdateTangentDrawingConfig()
        {
            Spline.ActivateDrawTangent(DrawTangent);
            Spline.SetTangentColor(TangentColor);
            Spline.SetTangentExtrusion(TangentExtrusion);
            Spline.SetTangentThickness(TangentThickness);
        }

        private void UpdateSphereDrawingConfig()
        {
            Spline.ActivateDrawSphere(DrawSphere);
            Spline.SetSphereRadius(SphereRadius);
            Spline.SetSphereColor(SphereColor);
        }

#endregion

    }
}



using UnityEditor;
using UnityEngine;

namespace SplineEditor.Controller.CatmullRomCalc
{
    public partial class CatmullRom
    {
//-------Public Variables-------//


//------Serialized Fields-------//


//------Private Variables-------//
        private bool _drawSpline = false;
        private bool _drawNormal = false;
        private bool _drawTangent = false;
        private bool _drawSphere = false;
        private Color _splineColor;
        private Color _normalColor;
        private Color _tangentColor;
        private Color _sphereColor;
        private float _normalExtrusion;
        private float _tangentExtrusion;
        private float _splineThickness = 2.5f;
        private float _normalThickness = 1.2f;
        private float _tangentThickness = 1.2f;
        private float _sphereRadius = 1f;

#region UNITY_METHODS
        private void OnDrawGizmos()
        {
            if (_drawSpline) DrawSpline();
            if (_drawNormal) DrawNormals();
            if (_drawTangent) DrawTangents();
            if (_drawSphere) DrawSphere();
        }

#endregion


#region PUBLIC_METHODS
        public void ActivateDrawSpline(bool flag) => _drawSpline = flag;
        public void ActivateDrawNormal(bool flag) => _drawNormal = flag;
        public void ActivateDrawTangent(bool flag) => _drawTangent = flag;
        public void ActivateDrawSphere(bool flag) => _drawSphere = flag;
        public void SetSphereColor(Color color) => _sphereColor = color;
        public void SetSplineColor(Color color) => _splineColor = color;
        public void SetNormalColor(Color color) => _normalColor = color;
        public void SetTangentColor(Color color) => _tangentColor = color;
        public void SetNormalExtrusion(float extrusion) => _normalExtrusion = extrusion;
        public void SetTangentExtrusion(float extrusion) => _tangentExtrusion = extrusion;
        public void SetSplineThickness(float thickness) => _splineThickness = thickness;
        public void SetNormalThickness(float thickness) => _normalThickness = thickness;
        public void SetTangentThickness(float thickness) => _tangentThickness = thickness;
        public void SetSphereRadius(float radius) => _sphereRadius = radius;

#endregion


#region PRIVATE_METHODS
        private void DrawSpline()
        {
            if (!IsSplinePointsInitialized())
                return;
            Handles.color = _splineColor;
            for(var i = 0; i < _splinePoints.Length; i++)
            {
                if (i == _splinePoints.Length - 1 && _closedLoop)
                    Handles.DrawLine(_splinePoints[i].Position, _splinePoints[0].Position, _splineThickness);
                else if(i < _splinePoints.Length - 1)
                    Handles.DrawLine(_splinePoints[i].Position, _splinePoints[i + 1].Position, _splineThickness);
            }
        }
        
        private void DrawNormals()
        {
            if (!IsSplinePointsInitialized())
                return;
            Handles.color = _normalColor;
            for(var i = 0; i < _splinePoints.Length; i++)
                Handles.DrawLine(_splinePoints[i].Position,
                    _splinePoints[i].Position + _splinePoints[i].Normal * _normalExtrusion, _normalThickness);
        }

        private void DrawTangents()
        {
            if (!IsSplinePointsInitialized())
                return;
            Handles.color = _tangentColor;
            for(var i = 0; i < _splinePoints.Length; i++)
                Handles.DrawLine(_splinePoints[i].Position,
                    _splinePoints[i].Position + _splinePoints[i].Tangent * _tangentExtrusion, _tangentThickness);
        }

        private void DrawSphere()
        {
            if (!IsSplinePointsInitialized())
                return;
            Gizmos.color = _sphereColor;
            for (var ind = 0; ind < _controlPoints.Length; ind++)
            {
                Gizmos.DrawSphere(_controlPoints[ind], _sphereRadius);
            }
        }
#endregion

    }
}



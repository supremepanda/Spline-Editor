using System;
using Extensions.Other;
using UnityEditor;
using UnityEngine;

namespace SplineEditor
{
    public class CatmullRom : MonoBehaviour
    {
//-------Public Variables-------//
        public const int MIN_POINTS_LENGTH = 2;


//------Serialized Fields-------//


//------Private Variables-------//
        private const int MIN_RESOLUTION = 2;
        private int _resolution;
        private bool _closedLoop;
        private CatmullRomPoint[] _splinePoints;
        private Vector3[] _controlPoints;
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
            if (_drawSpline)
            {
                DrawSpline();
            }

            if (_drawNormal)
            {
                DrawNormals();
            }

            if (_drawTangent)
            {
                DrawTangents();
            }

            if (_drawSphere)
            {
                DrawSphere();
            }
        }

#endregion


#region PUBLIC_METHODS
        public static CatmullRomPoint Evaluate(Vector3 start, Vector3 end, Vector3 tanPoint0, Vector3 tanPoint1, float t)
        {
            var position = CatmullRomCalculations.CalculatePosition(start, end, tanPoint0, tanPoint1, t);
            var tangent = CatmullRomCalculations.CalculateTangent(start, end, tanPoint0, tanPoint1, t);            
            var normal = CatmullRomCalculations.NormalFromTangent(tangent);
            return new CatmullRomPoint(position, tangent, normal);
        }
        
        public void ActivateDrawSpline(bool flag)
        {
            _drawSpline = flag;
        }

        public void ActivateDrawNormal(bool flag)
        {
            _drawNormal = flag;
        }

        public void ActivateDrawTangent(bool flag)
        {
            _drawTangent = flag;
        }

        public void ActivateDrawSphere(bool flag)
        {
            _drawSphere = flag;
        }

        public void SetSphereColor(Color color)
        {
            _sphereColor = color;
        }
        
        public void SetSplineColor(Color color)
        {
            _splineColor = color;
        }

        public void SetNormalColor(Color color)
        {
            _normalColor = color;
        }

        public void SetTangentColor(Color color)
        {
            _tangentColor = color;
        }

        public void SetNormalExtrusion(float extrusion)
        {
            _normalExtrusion = extrusion;
        }

        public void SetTangentExtrusion(float extrusion)
        {
            _tangentExtrusion = extrusion;
        }

        public void SetSplineThickness(float thickness)
        {
            _splineThickness = thickness;
        }

        public void SetNormalThickness(float thickness)
        {
            _normalThickness = thickness;
        }

        public void SetTangentThickness(float thickness)
        {
            _tangentThickness = thickness;
        }

        public void SetSphereRadius(float radius)
        {
            _sphereRadius = radius;
        }
        public void InitializeCatmullRom(Transform[] controlPoints, int resolution, bool closedLoop)
        {
            if(!IsGivenPointsLengthValid(controlPoints.Length) || !IsGivenResolutionValid(resolution))
                return;
            _controlPoints = new Vector3[controlPoints.Length];
            for(var i = 0; i < controlPoints.Length; i++)
                _controlPoints[i] = controlPoints[i].position;
            _resolution = resolution;
            _closedLoop = closedLoop;
            GenerateSplinePoints();
        }
        public CatmullRomPoint[] GetSplinePoints()
        {
            if(_splinePoints == null) EditorDebug.LogError("Spline is not initialized!");
            return _splinePoints;
        }
        
        public void UpdateControlPoints(Transform[] controlPoints)
        {
            if (controlPoints == null)
            {
                EditorDebug.LogError("Control points can not be null!");
                return;
            }
            if(!IsGivenPointsLengthValid(controlPoints.Length))
                return;
            _controlPoints = new Vector3[controlPoints.Length];
            for(var i = 0; i < controlPoints.Length; i++)
                _controlPoints[i] = controlPoints[i].position;
            GenerateSplinePoints();
        }
        
        public void UpdateResolution(int resolution, bool closedLoop)
        {
            if(!IsGivenResolutionValid(resolution))
                return;
            _resolution = resolution;
            GenerateSplinePoints();
        }
        
        public void UpdateClosedLoop(bool closedLoop)
        {
            _closedLoop = closedLoop;
            GenerateSplinePoints();
        }

        
#endregion


#region PRIVATE_METHODS

        private static bool IsGivenResolutionValid(int resolution)
        {
            if (resolution >= MIN_RESOLUTION) 
                return true;
            EditorDebug.LogError($"too few resolution (min: {MIN_RESOLUTION})");
            return false;
        }

        private static bool IsGivenPointsLengthValid(int length)
        {
            if (length >= 2) 
                return true;
            EditorDebug.LogError($"Too few control points (min: {MIN_POINTS_LENGTH})");
            return false;
        }
        
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
        
        private bool IsSplinePointsInitialized()
        {
            return _splinePoints != null;
        }
        
        private void InitializeProperties()
        {
            int pointsToCreate;
            if (_closedLoop)
                pointsToCreate = _resolution * _controlPoints.Length;
            else
                pointsToCreate = _resolution * (_controlPoints.Length - 1);
            _splinePoints = new CatmullRomPoint[pointsToCreate];       
        }

        private Vector3 CalculateEndPoint(bool closedLoopFinalPoint, int currentPointInd)
        {
            return closedLoopFinalPoint ? _controlPoints[0] : _controlPoints[currentPointInd + 1];
        }

        private Vector3 CalculateTangentZero(int currentPointInd, Vector3 startPoint, Vector3 endPoint)
        {
            if (currentPointInd == 0) // Tangent M[k] = (P[k+1] - P[k-1]) / 2
                if (_closedLoop)
                    return endPoint - _controlPoints[^1];
                else
                    return endPoint - startPoint;
            return endPoint - _controlPoints[currentPointInd - 1];
        }

        private Vector3 CalculateTangentOne(int currentPointInd, Vector3 startPoint, Vector3 endPoint)
        {
            if (_closedLoop)
            {
                if (currentPointInd == _controlPoints.Length - 1)
                    return _controlPoints[(currentPointInd + 2) % _controlPoints.Length] - startPoint;
                if (currentPointInd == 0)
                    return _controlPoints[currentPointInd + 2] - startPoint;
                return _controlPoints[(currentPointInd + 2) % _controlPoints.Length] - startPoint;
            }
            if (currentPointInd < _controlPoints.Length - 2)
                return _controlPoints[(currentPointInd + 2) % _controlPoints.Length] - startPoint;
            return endPoint - startPoint;
        }

        private void CreateTesselationPoints(int currentPointInd, float pointStep, Vector3 startPoint, Vector3 endPoint,
            Vector3 tangent0, Vector3 tangent1)
        {
            for (var tesselatedPoint = 0; tesselatedPoint < _resolution; tesselatedPoint++)
            {
                var t = tesselatedPoint * pointStep;
                var point = Evaluate(startPoint, endPoint, tangent0, tangent1, t);
                _splinePoints[currentPointInd * _resolution + tesselatedPoint] = point;
            }
        }
#endregion
        private void GenerateSplinePoints()
        {
            InitializeProperties();
            Vector3 startPoint, endPoint;
            Vector3 tangent0, tangent1;
            var closedAdjustment = _closedLoop ? 0 : 1;
            for (var currentPoint = 0; currentPoint < _controlPoints.Length - closedAdjustment; currentPoint++)
            {
                var closedLoopFinalPoint = _closedLoop && currentPoint == _controlPoints.Length - 1;
                startPoint = _controlPoints[currentPoint];
                endPoint = CalculateEndPoint(closedLoopFinalPoint, currentPoint);
                tangent0 = CalculateTangentZero(currentPoint, startPoint, endPoint);
                tangent1 = CalculateTangentOne(currentPoint, startPoint, endPoint);
                tangent0 *= 0.5f;
                tangent1 *= 0.5f;
                var pointStep = 1.0f / _resolution;
                if (currentPoint == _controlPoints.Length - 2 && !_closedLoop || closedLoopFinalPoint)
                    pointStep = 1.0f / (_resolution - 1);
                CreateTesselationPoints(currentPoint, pointStep, startPoint, endPoint, tangent0, tangent1);
            }
        }
    }
}
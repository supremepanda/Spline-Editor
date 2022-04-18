using Extensions.Other;
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

#region UNITY_METHODS


#endregion


#region PUBLIC_METHODS
        public static CatmullRomPoint Evaluate(Vector3 start, Vector3 end, Vector3 tanPoint0, Vector3 tanPoint1, float t)
        {
            var position = CatmullRomCalculations.CalculatePosition(start, end, tanPoint0, tanPoint1, t);
            var tangent = CatmullRomCalculations.CalculateTangent(start, end, tanPoint0, tanPoint1, t);            
            var normal = CatmullRomCalculations.NormalFromTangent(tangent);
            return new CatmullRomPoint(position, tangent, normal);
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
        
        public void DrawSpline(Color color)
        {
            if (!IsSplinePointsInitialized())
                return;
            for(var i = 0; i < _splinePoints.Length; i++)
            {
                if (i == _splinePoints.Length - 1 && _closedLoop)
                    Debug.DrawLine(_splinePoints[i].Position, _splinePoints[0].Position, color);
                else if(i < _splinePoints.Length - 1)
                    Debug.DrawLine(_splinePoints[i].Position, _splinePoints[i + 1].Position, color);
            }
        }
        
        public void DrawNormals(float extrusion, Color color)
        {
            if (!IsSplinePointsInitialized())
                return;
            for(var i = 0; i < _splinePoints.Length; i++)
                Debug.DrawLine(_splinePoints[i].Position,
                    _splinePoints[i].Position + _splinePoints[i].Normal * extrusion, color);
        }

        public void DrawTangents(float extrusion, Color color)
        {
            if (!IsSplinePointsInitialized())
                return;
            for(var i = 0; i < _splinePoints.Length; i++)
                Debug.DrawLine(_splinePoints[i].Position,
                    _splinePoints[i].Position + _splinePoints[i].Tangent * extrusion, color);
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
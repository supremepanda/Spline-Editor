using Extensions.Other;
using UnityEngine;

namespace SplineEditor.Controller.CatmullRomCalc
{
    public partial class CatmullRom : MonoBehaviour
    {
//-------Public Variables-------//
        public const int MIN_POINTS_LENGTH = 2;
        public CatmullRomPoint[] SplinePoints => _splinePoints;
        public float TotalLength => _totalLength;
//------Serialized Fields-------//


//------Private Variables-------//
        private const int MIN_RESOLUTION = 2;
        private int _resolution;
        private bool _closedLoop;
        private CatmullRomPoint[] _splinePoints;
        private Vector3[] _controlPoints;
        private float _totalLength;
#region UNITY_METHODS

        

#endregion


#region PUBLIC_METHODS
        public static CatmullRomPoint Evaluate(Vector3 start, Vector3 end, Vector3 tanPoint0, Vector3 tanPoint1, float t)
        {
            var position = CatmullRomCalculations.CalculatePosition(start, end, tanPoint0, tanPoint1, t);
            var tangent = CatmullRomCalculations.CalculateTangent(start, end, tanPoint0, tanPoint1, t);            
            var normal = CatmullRomCalculations.NormalFromTangent(tangent);
            return new CatmullRomPoint(position, tangent, normal, 0f, 0f);
        }

        public (Vector3, Vector3) GetPositionAndTangentFromDistance(float distance, float xPos)
        {
            if(_totalLength <= 0f)
                CalculateTotalLength();
            distance = Mathf.Clamp(distance, 0f, _totalLength);
            var point1 = new CatmullRomPoint();
            var point2 = new CatmullRomPoint();
            for (var ind = 0; ind < _splinePoints.Length; ind++)
            {
                if (distance < _splinePoints[ind].DistanceToStart)
                {
                    point1 = _splinePoints[ind - 1];
                    point2 = _splinePoints[ind];
                    break;
                }
                if (distance == _splinePoints[ind].DistanceToStart)
                {
                    return (_splinePoints[ind].Position + _splinePoints[ind].Normal * -xPos, _splinePoints[ind].Tangent);
                }
            }
            var t = Mathf.InverseLerp(point1.DistanceToStart, point2.DistanceToStart, distance);
            return CalculateTargetPosAndTangent(point1, point2, t, xPos);
        }
        
        public (Vector3, Vector3) GetPositionAndTangentFromNormalizedValue(float value, float xPos)
        {
            var point1 = new CatmullRomPoint();
            var point2 = new CatmullRomPoint();
            for (var ind = 0; ind < _splinePoints.Length; ind++)
            {
                if (value < _splinePoints[ind].NormalizedValue)
                {
                    point1 = _splinePoints[ind - 1];
                    point2 = _splinePoints[ind];
                    break;
                }
                if (value == _splinePoints[ind].NormalizedValue)
                {
                    return (_splinePoints[ind].Position + _splinePoints[ind].Normal * -xPos, _splinePoints[ind].Tangent);
                }
            }
            var t = Mathf.InverseLerp(point1.NormalizedValue, point2.NormalizedValue, value);
            return CalculateTargetPosAndTangent(point1, point2, t, xPos);
        }
        
        public float CalculateNormalizedValueUsingDistance(float distance)
        {
            return Mathf.Clamp01(distance / _totalLength);
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
            UpdateClosedLoop(closedLoop, false);
            if(!IsGivenResolutionValid(resolution))
                return;
            _resolution = resolution;
            GenerateSplinePoints();
        }
        
        public void UpdateClosedLoop(bool closedLoop, bool generatePoints)
        {
            _closedLoop = closedLoop;
            if (!generatePoints)
                return;
            GenerateSplinePoints();
        }

        
#endregion


#region PRIVATE_METHODS
        private (Vector3, Vector3) CalculateTargetPosAndTangent(CatmullRomPoint point1, CatmullRomPoint point2, float t,
            float xPos)
        {
            var targetPos = Vector3.Lerp(point1.Position, point2.Position, t);
            var targetPosX = Vector3.Lerp(point1.Normal, point2.Normal, t) * -xPos;
            var tangent = Vector3.Lerp(point1.Tangent, point2.Tangent, t);
            return (targetPos + targetPosX, tangent + tangent * -xPos);
        }
        
        private static bool IsGivenResolutionValid(int resolution)
        {
            if (resolution >= MIN_RESOLUTION) 
                return true;
            EditorDebug.LogError($"too few resolution (min: {MIN_RESOLUTION})");
            return false;
        }

        private static bool IsGivenPointsLengthValid(int length)
        {
            return length >= 2;
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
                {
                    // ReSharper disable once UselessBinaryOperation
                    if (currentPointInd + 2 >= _controlPoints.Length)
                        return Vector3.zero;
                    return _controlPoints[currentPointInd + 2] - startPoint;
                }
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

        private void CalculateTotalLength()
        {
            var length = 0f;
            for (var ind = 0; ind < _splinePoints.Length; ind++)
            {
                if (ind == 0)
                    continue;
                length += Vector3.Distance(_splinePoints[ind].Position, _splinePoints[ind - 1].Position);
            }

            _totalLength = length;
        }

        private void CalculateNormalizedValuesOfPoints()
        {
            for (var ind = 0; ind < _splinePoints.Length; ind++)
            {
                if (ind == 0)
                {
                    _splinePoints[ind].NormalizedValue = 0f;
                    _splinePoints[ind].DistanceToStart = 0f;
                    continue;
                }
                var distance = Vector3.Distance(_splinePoints[ind].Position, _splinePoints[ind - 1].Position)
                               + _splinePoints[ind - 1].DistanceToStart;
                var normalizedValue = distance / _totalLength;
                _splinePoints[ind].NormalizedValue = normalizedValue;
                _splinePoints[ind].DistanceToStart = distance;
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
            CalculateTotalLength();
            CalculateNormalizedValuesOfPoints();
        }
    }
}
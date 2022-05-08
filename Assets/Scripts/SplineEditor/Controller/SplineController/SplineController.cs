using System.Collections.Generic;
using System.Linq;
using Extensions.Other;
using Sirenix.OdinInspector;
using SplineEditor.Controller.CatmullRomCalc;
using SplineEditor.Controller.Enums;
using UnityEditor;
using UnityEngine;

namespace SplineEditor.Controller.SplineController
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(CatmullRom))]
    public partial class SplineController : MonoBehaviour
    {
//-------Public Variables-------//
        public CatmullRom GetSpline => Spline;
        public List<Transform> GetEventPoints => EventPoints; 
//------Serialized Fields-------//
        [SerializeField, Required, PropertyOrder(-99)] private CatmullRom Spline;
        [SerializeField, ReadOnly, TabGroup("Debug"), PropertyOrder(1)] private List<Transform> ControlPoints;

        [SerializeField, TabGroup("Config"), PropertyOrder(-1)] private UpdateMethod UpdateMethod;
        [SerializeField, Range(2, 25), TabGroup("Config"), OnValueChanged(nameof(UpdateSpline))] private int Resolution;
        [SerializeField, TabGroup("Config"), OnValueChanged(nameof(UpdateSpline))] private bool IsClosedLoop;
        [SerializeField, TabGroup("Config")] private Direction PointDirection = Direction.XZ;
//------Private Variables-------//
        private const float DISTANCE_BETWEEN_TWO_POINTS = 2f;
        private const string POINT_NAME_PREFIX = "Point_";
        private GameObject _pointPrefab;
#region UNITY_METHODS

        private void OnEnable()
        {
            if (Spline == null)
                return;
            if (Application.isPlaying)
                return;
            UpdateSpline();
#if UNITY_EDITOR
            UpdateSplineDrawingConfig();
            UpdateNormalDrawingConfig();
            UpdateTangentDrawingConfig();
            UpdateSphereDrawingConfig();
#endif
        }

        private void Awake()
        {
            if (Spline == null)
                return;
            if (UpdateMethod != UpdateMethod.WithMethod)
                return;
            UpdateSpline();
        }

        private void Start()
        {
#if UNITY_EDITOR
            UpdateSplineDrawingConfig();
            UpdateNormalDrawingConfig();
            UpdateTangentDrawingConfig();
            UpdateSphereDrawingConfig();
#endif
        }

        private void Update()
        {
            if (Spline == null)
                return;
            if (UpdateMethod != UpdateMethod.OnUpdate)
                return;
            UpdateSpline();
        }

#endregion


#region PUBLIC_METHODS

        public void RemovePoint(Transform point, bool destroyEnabled)
        {
#if UNITY_EDITOR
            if (point == null)
                return;
            if (Spline == null)
                return;
            ControlPoints.Remove(point);
            UpdateSpline();
            UpdatePointNames();
            if (!destroyEnabled)
                return;
            DestroyImmediate(point.gameObject);
#endif
        }

        public void RemoveEvent(Transform eventPoint, bool destroyEnabled)
        {
#if UNITY_EDITOR
            if (eventPoint == null)
                return;
            if (Spline == null)
                return;
            EventPoints.Remove(eventPoint);
            UpdateEventNames();
            if (!destroyEnabled)
                return;
            DestroyImmediate(eventPoint.gameObject);
#endif
        }
#endregion
        
#region PRIVATE_METHODS

        [Button(ButtonSizes.Large), ShowIf("UpdateMethod", UpdateMethod.WithMethod)]
        private void UpdateSpline()
        {
            if (Spline == null)
                return;
            Spline.UpdateControlPoints(ControlPoints.ToArray());
            Spline.UpdateResolution(Resolution, IsClosedLoop);
        }
        [Button(ButtonSizes.Large), GUIColor(.2f, .8f, .2f)]
        private void AddPoint()
        {
            CreateNewPoint();
        }
        
        
        [Button(ButtonSizes.Large), GUIColor(.2f, .8f, .2f), PropertyOrder(-1)
        ,HideIf(nameof(Spline))]
        private void InitializeSpline()
        {
            if (Spline == null)
            {
                Spline = GetComponent<CatmullRom>();
                CreateInitialPoints();
                Spline.InitializeCatmullRom(ControlPoints.ToArray(), Resolution, IsClosedLoop);
            }
        }

        [Button(ButtonSizes.Large), GUIColor(.8f, .2f, .2f), TabGroup("Remove")]
        private void RemoveSpline()
        {
#if UNITY_EDITOR
            Spline = null;
            foreach (var point in ControlPoints)
            {
                if(point == null)
                    continue;
                DestroyImmediate(point.gameObject);
            }
            ControlPoints = new List<Transform>();
#endif
        }
        private void CreateInitialPoints()
        {
#if UNITY_EDITOR
            ControlPoints = new List<Transform>();
            for (var ind = 0; ind < CatmullRom.MIN_POINTS_LENGTH; ind++)
            {
                var point = CreatePoint();
                var target = ind * DISTANCE_BETWEEN_TWO_POINTS;
                var localPosition = point.transform.localPosition;
                localPosition = PointDirection switch
                {
                    Direction.XZ => localPosition.SetZ(target),
                    Direction.XY => localPosition.SetY(target),
                    _ => localPosition
                };
                point.transform.localPosition = localPosition;
                ControlPoints.Add(point.transform);
            }
#endif
            
        }

        private void CreateNewPoint()
        {
#if UNITY_EDITOR
            var point = CreatePoint();
            var lastPointPos = ControlPoints.Last().localPosition;
            var targetPos = PointDirection switch
            {
                Direction.XZ => lastPointPos.AddZ(DISTANCE_BETWEEN_TWO_POINTS),
                Direction.XY => lastPointPos.AddY(DISTANCE_BETWEEN_TWO_POINTS),
                _ => lastPointPos
            };
            point.transform.localPosition = targetPos;
            ControlPoints.Add(point.transform);
#endif
        }

        private GameObject CreatePoint()
        {
#if UNITY_EDITOR
            if (_pointPrefab == null) _pointPrefab = Resources.Load<GameObject>("Spline/Point");
            var point = PrefabUtility.InstantiatePrefab(_pointPrefab, transform) as GameObject;
            point.name = $"{POINT_NAME_PREFIX}{ControlPoints.Count}";
            return point; 
#else
            return null;
#endif
        }
        
        private void UpdatePointNames()
        {
            for (var ind = 0; ind < ControlPoints.Count; ind++)
            {
                ControlPoints[ind].gameObject.name = $"{POINT_NAME_PREFIX}{ind}";
            }
        }
#endregion

    }
}



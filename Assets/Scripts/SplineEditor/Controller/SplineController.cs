using System.Collections.Generic;
using System.Linq;
using Extensions.Other;
using Sirenix.OdinInspector;
using SplineEditor.Events;
using SplineEditor.PathFollowing.Positioner;
using UnityEditor;
using UnityEngine;

namespace SplineEditor.Controller
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(CatmullRom))]
    public class SplineController : MonoBehaviour
    {
//-------Public Variables-------//
        public CatmullRom GetSpline => Spline;
        public List<Transform> GetEventPoints => EventPoints; 
//------Serialized Fields-------//
        [SerializeField, Required] private CatmullRom Spline;
        [SerializeField, ReadOnly] private List<Transform> ControlPoints;
        [SerializeField, ReadOnly] private List<Transform> EventPoints;
        [SerializeField, HideInInspector] private Transform EventHandler;
        
        [SerializeField, TabGroup("Config")] private UpdateMethod UpdateMethod;
        [SerializeField, Range(2, 25), TabGroup("Config"), OnValueChanged(nameof(UpdateSpline))] private int Resolution;
        [SerializeField, TabGroup("Config"), OnValueChanged(nameof(UpdateSpline))] private bool IsClosedLoop;
        [SerializeField, TabGroup("Config")] private Direction PointDirection = Direction.XZ;
        
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
        
        [SerializeField, TabGroup("References")] private GameObject PointPrefab;
        [SerializeField, TabGroup("References")] private GameObject EventPrefab;
//------Private Variables-------//
        private const float DISTANCE_BETWEEN_TWO_POINTS = 2f;
        private const string POINT_NAME_PREFIX = "Point_";
        private const string EVENT_NAME_PREFIX = "Event_";
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

        [Button(ButtonSizes.Large), GUIColor(.9f, .7f, .2f)]
        private void AddEvent()
        {
            CreateEventPoint();
        }
        
        [Button(ButtonSizes.Large), GUIColor(.2f, .8f, .2f), PropertyOrder(-1)
        ,HideIf(nameof(Spline))]
        private void InitializeSpline()
        {
            if (Spline == null)
            {
                Spline = GetComponent<Controller.CatmullRom>();
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

        [Button(ButtonSizes.Large), GUIColor(.8f, .2f, .2f), TabGroup("Remove")]
        private void RemoveEvents()
        {
#if UNITY_EDITOR
            var eventPoints = EventPoints;
            for (var ind = 0; ind < eventPoints.Count; ind++) 
                RemoveEvent(eventPoints[ind], true);
            EventPoints = new List<Transform>();
            if (EventHandler == null)
                return;
            DestroyImmediate(EventHandler.gameObject);
            EventHandler = null;
#endif
        }
        private void InitializeEvents()
        {
            var eventTransform = new GameObject();
            eventTransform.name = "Events";
            eventTransform.transform.SetParent(transform);
            EventHandler = eventTransform.transform;
        }
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

        private void CreateInitialPoints()
        {
#if UNITY_EDITOR
            ControlPoints = new List<Transform>();
            for (var ind = 0; ind < Controller.CatmullRom.MIN_POINTS_LENGTH; ind++)
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

        private void CreateEventPoint()
        {
#if UNITY_EDITOR
            if(EventHandler == null)
                InitializeEvents();
            var eventPoint = PrefabUtility.InstantiatePrefab(EventPrefab, EventHandler) as GameObject;
            eventPoint.name = $"{EVENT_NAME_PREFIX}{EventPoints.Count}";
            eventPoint.TryGetComponent(out SplineEvent splineEvent);
            EventPoints.Add(eventPoint.transform);
            if (splineEvent == null)
                return;
            splineEvent.SetSpline(Spline);
#endif
        }

        private GameObject CreatePoint()
        {
#if UNITY_EDITOR
            var point = PrefabUtility.InstantiatePrefab(PointPrefab, transform) as GameObject;
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

        private void UpdateEventNames()
        {
            for (var ind = 0; ind < EventPoints.Count; ind++)
            {
                EventPoints[ind].gameObject.name = $"{EVENT_NAME_PREFIX}{ind}";
            }
        }
#endregion

    }
}



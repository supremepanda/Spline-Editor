using System;
using System.Collections.Generic;
using System.Linq;
using Extensions.Other;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace SplineEditor
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(CatmullRom))]
    public class SplineCreator : MonoBehaviour
    {
//-------Public Variables-------//
        public CatmullRom GetSpline => Spline;
        
//------Serialized Fields-------//
        [SerializeField, Required] private CatmullRom Spline;
        [SerializeField, ReadOnly] private List<Transform> ControlPoints;
        
        [SerializeField, TabGroup("Config")] private UpdateMethod UpdateMethod;
        [SerializeField, Range(2, 25), TabGroup("Config")] private int Resolution;
        [SerializeField, TabGroup("Config")] private bool IsClosedLoop;
        [SerializeField, TabGroup("Config")] private Direction PointDirection = Direction.XZ;
        
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

        [SerializeField, TabGroup("References")] private GameObject PointPrefab;
//------Private Variables-------//
        private const float DISTANCE_BETWEEN_TWO_POINTS = 2f;

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

        [Button(ButtonSizes.Large), GUIColor(.2f, .8f, .2f), TabGroup("Config")]
        public void AddPoint()
        {
            CreatePoint();
        }
        
        [Button(ButtonSizes.Large), GUIColor(.2f, .8f, .2f), TabGroup("Init")]
        public void InitializeSpline()
        {
            if (Spline == null)
            {
                Spline = GetComponent<CatmullRom>();
                CreateInitialPoints();
                Spline.InitializeCatmullRom(ControlPoints.ToArray(), Resolution, IsClosedLoop);
            }
        }

        [Button(ButtonSizes.Large), GUIColor(.8f, .2f, .2f), TabGroup("Init")]
        public void RemoveSpline()
        {
#if UNITY_EDITOR
            Spline = null;
            foreach (var point in ControlPoints) 
                DestroyImmediate(point.gameObject);
            ControlPoints = new List<Transform>();
#endif
        }

        [Button(ButtonSizes.Large), ShowIf("UpdateMethod", UpdateMethod.WithMethod)]
        public void UpdateSpline()
        {
            if (Spline == null)
                return;
            Spline.UpdateControlPoints(ControlPoints.ToArray());
            Spline.UpdateResolution(Resolution, IsClosedLoop);
        }
#endregion


#region PRIVATE_METHODS

        private void CheckDrawings()
        {
            if (Spline == null)
                return;
            if (DrawSpline) 
                Spline.DrawSpline(SplineColor);
            if(DrawNormals)
                Spline.DrawNormals(NormalExtrusion, NormalColor);
            if(DrawTangent)
                Spline.DrawTangents(TangentExtrusion, TangentColor);
        }

        private void CreateInitialPoints()
        {
#if UNITY_EDITOR
            ControlPoints = new List<Transform>();
            for (var ind = 0; ind < CatmullRom.MIN_POINTS_LENGTH; ind++)
            {
                var point = PrefabUtility.InstantiatePrefab(PointPrefab, transform) as GameObject;
                point.name = $"Point_{ind}";
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

        private void CreatePoint()
        {
#if UNITY_EDITOR
            var point = PrefabUtility.InstantiatePrefab(PointPrefab, transform) as GameObject;
            if (point == null)
                return;
            point.name = $"Point_{ControlPoints.Count}";
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
#endregion

    }
}



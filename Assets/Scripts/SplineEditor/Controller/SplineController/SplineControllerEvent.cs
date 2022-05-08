using System.Collections.Generic;
using Sirenix.OdinInspector;
using SplineEditor.Events;
using UnityEditor;
using UnityEngine;

namespace SplineEditor.Controller.SplineController
{
    public partial class SplineController
    {
//-------Public Variables-------//


//------Serialized Fields-------//
        [SerializeField, ReadOnly, TabGroup("Debug")] private List<Transform> EventPoints;
        [SerializeField, HideInInspector] private Transform EventHandler;

//------Private Variables-------//
        private const string EVENT_NAME_PREFIX = "Event_";
        private GameObject _eventPrefab;


#region UNITY_METHODS


#endregion


#region PUBLIC_METHODS

#endregion


#region PRIVATE_METHODS
        [Button(ButtonSizes.Large), GUIColor(.9f, .7f, .2f)]
        private void AddEvent()
        {
            CreateEventPoint();
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
        
        private void CreateEventPoint()
        {
#if UNITY_EDITOR
            if(EventHandler == null)
                InitializeEvents();
            if (_eventPrefab == null)
                _eventPrefab = Resources.Load<GameObject>("Spline/Event");
            var eventPoint = PrefabUtility.InstantiatePrefab(_eventPrefab, EventHandler) as GameObject;
            eventPoint.name = $"{EVENT_NAME_PREFIX}{EventPoints.Count}";
            eventPoint.TryGetComponent(out SplineEvent splineEvent);
            EventPoints.Add(eventPoint.transform);
            if (splineEvent == null)
                return;
            splineEvent.SetSpline(Spline);
#endif
        }
        private void UpdateEventNames()
        {
            for (var ind = 0; ind < EventPoints.Count; ind++)
                EventPoints[ind].gameObject.name = $"{EVENT_NAME_PREFIX}{ind}";
        }
#endregion

    }
}



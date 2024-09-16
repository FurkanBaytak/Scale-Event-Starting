using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = System.Random;

public class EventHandler : MonoBehaviour
{
    private static EventHandler instance;
    private List<Type> events = new List<Type>();
    
    public float perEventTime = 10;
    public float curentTimer;
    
    private Random Random = new Random();
    public TextMeshProUGUI timerText;
    public Timer timer;
    
    public static EventHandler GetInstance()
    {
        return instance;
    }

    public void Awake()
    {
        instance = this;
    }
    
    // Loads events on start
    // and adds them to the events list
    public void Start()
    {
        events.Clear();
        timer =  GameObject.FindGameObjectWithTag("Timer").GetComponent<Timer>();
        curentTimer = perEventTime;
        LoadEvents().ToList().ForEach(x => events.Add(x));
        Debug.Log(events.Count + " events loaded");
    }
    
    void Update()
    {
        if (curentTimer > 0)
        {
            curentTimer -= Time.deltaTime;
        }
        else
        {
            curentTimer = perEventTime;
            StartRandomEvent();
        }
    }

    public void StartRandomEvent()
    {
        if (events.Count == 0) LoadEvents().ToList().ForEach(x => events.Add(x));
        
        int randomEvent = Random.Next(0, events.Count);
        Type myType = events[randomEvent];
        StartCoroutine(StartEvent(myType));
    }
    
    // Starts the event of the given type
    // Pass Class Type as event type
    public IEnumerator StartEvent(Type myType)
    {
        foreach (Type type in events)
        {
            if (type == myType)
            {
                BaseEvent baseEvent = Activator.CreateInstance(type) as BaseEvent;
                float duration = baseEvent.GetEventDuration();
                baseEvent.StartEvent();
                timer.StartEvent(baseEvent);
                yield return new WaitForSeconds(duration);
                baseEvent.StopEvent();
            }
        }
    }
    
    // Stops the event of the given type
    // Pass Class Type as event type
    public void StopEvent(Type myType)
    {
        foreach (Type type in events)
        {
            if (type == myType)
            {
                BaseEvent baseEvent = Activator.CreateInstance(type) as BaseEvent;
                baseEvent.StopEvent();
            }
        }
        
        // Removing event to prevent it from being called again
        events.Remove(myType);
    }
    
    // Loads all events from the assemblies
    // and returns them as a list
    // 
    // Excludes BaseEvent class
    private IEnumerable<Type> LoadEvents()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(typeof(BaseEvent)))
            .Select(type => type);
    }
    
}

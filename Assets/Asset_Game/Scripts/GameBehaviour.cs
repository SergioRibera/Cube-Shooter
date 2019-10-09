using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehaviour : MonoBehaviour
{
    public class TimedEvent
    {
        public int index;
        public float TimeToExecute;
        public Callback Method;
    }
    private static List<TimedEvent> events = new List<TimedEvent>();
    public delegate void Callback();

    void Start(){}
    public void Update(){}
    void FixedUpdate()
    {
        if (events.Count == 0)
            return;

        for (int i = 0; i < events.Count; i++)
        {
            var timedEvent = events[i];
            if (timedEvent.TimeToExecute <= Time.time)
            {
                timedEvent.Method();
                events.Remove(timedEvent);
            }
        }
    }
    /*/// <summary>
    /// This Function Pause a moment determinate for a time and continue
    /// </summary>
    /// <param name="time">The time determinate for pause</param>
    public void Sleep(float time) {
        events.Add(new TimedEvent
        {
            Method = method,
            TimeToExecute = Time.time + inSeconds
        });
    }*/
    /// <summary>
    /// This Function Pause a moment determinate for a time and continue calling the Function d
    /// </summary>
    /// <param name="time">The time determinate for pause</param>
    /// <param name="d">Function call when the time is over</param>
    public static void Sleep(float time, Callback d) { events.Add(new TimedEvent { Method = d, TimeToExecute = Time.time + time, index = events.Count + 1 } ); }

    public void DeleteItem(Callback d)
    {
        foreach (var item in events)
        {
            if (item.Method == d)
                events.Remove(item);
        }
    }    
}
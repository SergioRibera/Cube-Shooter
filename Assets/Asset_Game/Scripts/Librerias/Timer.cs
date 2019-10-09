using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {
    public class TimedEvent
    {
        public float TimeToExecute;
        public Callback Method;
    }
    
    public static List<TimedEvent> events = new List<TimedEvent>();
    public delegate void Callback();
    /// <summary>
    /// Con  esta función Añades funciones a la lista de espera para ejecutarla (Hace falta que el script esté en un GameObject)
    /// </summary>
    /// <param name="method">Método al que se llamará</param>
    /// <param name="inSeconds">El tiempo en el que se lo llamará</param>
    public static void Add(Callback method, float inSeconds)
    {
        events.Add(new TimedEvent
        {
            Method = method,
            TimeToExecute = Time.time + inSeconds
        });
    }
    void Update () {
        if (events.Count == 0)
            return;

        for (int i = 0; i < events.Count; i++)
        {
            var timedEvent = events[i];
            if(timedEvent.TimeToExecute <= Time.time)
            {
                timedEvent.Method();
                events.Remove(timedEvent);
            }
        }
    }
}
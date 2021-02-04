using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class MyNotification : INotification, INotificationOptionProvider
{
    public PropertyName id
    {
        get { return new PropertyName("MyNotification"); }
    }

    NotificationFlags INotificationOptionProvider.flags
    {
        get
        {
            return NotificationFlags.TriggerOnce;
        }
    }
}

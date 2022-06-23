using UnityEngine;
using WwiseEvent = AK.Wwise.Event;


public class PostWwisePlayEvent : PostWwiseEventBase
{
     public WwiseEvent WwiseEvent_play;


    [ContextMenu(nameof(SendPlayEvent))]
    public override void SendPlayEvent()
    {
        PostEvent(WwiseEvent_play);
    }
}
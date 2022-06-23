using UnityEngine;
using WwiseEvent = AK.Wwise.Event;


/// <summary>
///     This allows distance attenuation
///     Modified from https://www.youtube.com/watch?v=HEvkBiz9JfQ&list=PL2YvvQu4Ub5uzib_IZJfTfhyQpIk0CXsA&index=1
/// </summary>
public abstract class PostWwiseEventBase : MonoBehaviour
{
    public abstract void SendPlayEvent();


    protected void PostEvent(WwiseEvent wwiseEvent)
    {
        PostWwiseEvent.PostEvent(wwiseEvent, gameObject);
    }
}
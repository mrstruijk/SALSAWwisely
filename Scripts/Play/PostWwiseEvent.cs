using UnityEngine;
using Event = AK.Wwise.Event;


public static class PostWwiseEvent
{
    public static void PostEvent(Event wwiseEvent, GameObject gameObject)
    {
        if (!wwiseEvent.IsValid())
        {
            Debug.LogWarning("Wwise event not valid");

            return;
        }

        AkSoundEngine.PostEvent(wwiseEvent.Name, gameObject);
    }


    public static void PostEvent(string wwiseEvent, GameObject gameObject)
    {
        if (!ak.wwise.core.soundbank.generated.Contains(wwiseEvent))
        {
            Debug.LogWarning("Wwise event not valid");

            return;
        }

        AkSoundEngine.PostEvent(wwiseEvent, gameObject);
    }
}
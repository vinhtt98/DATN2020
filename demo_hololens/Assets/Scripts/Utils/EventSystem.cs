using System.Collections;
using System.Collections.Generic;

public class DispatchedEvent
{
    public enum EventType
    {
    }
    public enum EventArg
    {
    }
    private Dictionary<EventArg, string> args;

    public DispatchedEvent(Dictionary<EventArg, string> args)
    {
        this.args = args;
    }

    public string get(EventArg key)
    {
        string res;
        if (args.TryGetValue(key, out res))
        {
            return res;
        }
        return null;
    }
}

// A delegate type for hooking up change notifications.
public delegate void DispatchedEventHandler(object sender, DispatchedEvent e);

// A class that works just like ArrayList, but sends event
// notifications whenever the list changes.
public class ListWithDispatchedEvent : ArrayList
{
    #region SINGLETON PATTERN
    private ListWithDispatchedEvent()
    {
    }
    private static ListWithDispatchedEvent instance = null;
    public static ListWithDispatchedEvent Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ListWithDispatchedEvent();
            }
            return instance;
        }
    }
    #endregion
    // An event that clients can use to be notified whenever the
    // elements of the list change.
    public event DispatchedEventHandler MyEventHandler;

    // Invoke the Changed event; called whenever list changes
    public virtual void DispatchEvent(DispatchedEvent e)
    {
        if (MyEventHandler != null)
            //you raise the event here.
            MyEventHandler(this, e);
    }
}

class EventListenerTemplate
{
    private ListWithDispatchedEvent List;

    public EventListenerTemplate(ListWithDispatchedEvent list)
    {
        List = list;
        // Add "ListChanged" to the Changed event on "List".
        //This is how we subscribe to the event created in ListWithChangedEvent class
        List.MyEventHandler += new DispatchedEventHandler(ListChanged);
    }

    // This will be called whenever the list changes.
    private void ListChanged(object sender, DispatchedEvent e)
    {
        //This is called when the event fires
    }
}

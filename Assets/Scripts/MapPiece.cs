using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPiece : MonoBehaviour
{
    public Transform nextStart;
    public MapPiece previous;
    public MapPiece next;
    public int endSize;
    private bool notDestroyed = true;

    public void destroy()
    {
        if(previous != null)
        {
            previous.next = null;
        }
        if(next != null)
        {
            next.previous = null;
        }
        Destroy(gameObject);
    }

    public void clear()
    {
        notDestroyed = false;
        if(previous != null && previous.notDestroyed)
        {
            previous.clear();
        }
        if(next != null && next.notDestroyed)
        {
            next.clear();
        }
        Destroy(gameObject);
    }
}

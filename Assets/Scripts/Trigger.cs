using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    [SerializeField] VoidEvent endReachedEvent;
    [SerializeField] VoidEvent resetEvent;
    private bool nextUngenerated = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && nextUngenerated)
        {
            nextUngenerated = false;
            endReachedEvent.RaiseEvent();
        }
    }
    private void Start()
    {
        resetEvent?.Subscribe(reset);
    }

    private void reset()
    {
        nextUngenerated = true;
    }
}

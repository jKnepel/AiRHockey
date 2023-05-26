using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARControllerBehaviour : MonoBehaviour
{
    [SerializeField]
    private Transform parent;
    [SerializeField]
    private Vector3 offset = new Vector3(0, 0, 0);
    //[SerializeField]
    //private float speed = 20;
    //Add an offset from center of the Playfield
    private Vector3 offsetInit = new Vector3(0,0.1f,0);
    private Rigidbody rigidbody;
    private int _layerMask;
    private bool selected = false;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        _layerMask = (1 << LayerMask.NameToLayer("Barrier")) | (1 << LayerMask.NameToLayer("Player Barrier")) | (1 << LayerMask.NameToLayer("Puck"));
    }

    public void ResetPosition(Vector3 position)
    {
        transform.position = position + offsetInit;
    }

    public void IsSelected(Single single)
    {
        selected = !selected;
        if(selected)
        {
            Debug.Log("Is Selected");   
        }
    }

    public void IsDeselected(Single single)
    {
        Debug.Log("Is Deselected");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*
        if(parent.gameObject.activeSelf)
        {
            Vector3 newPosition = parent.position + offset;
            Vector3 direction = newPosition - transform.position;
            float delta = Vector3.Distance(transform.position, newPosition);
            if (!Physics.Raycast(transform.position, direction, delta, _layerMask))
                rigidbody.MovePosition(newPosition);
        }
        */
    }
}

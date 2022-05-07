using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocking : MonoBehaviour
{

    private Renderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }


    // Update is called once per frame
    void Update()
    {

        if (_renderer == null)
            return;

        if (!_renderer.isVisible)
            return;

        Debug.Log(gameObject.name + " is visible!");



    }
}

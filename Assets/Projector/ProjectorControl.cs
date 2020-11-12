using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectorControl : MonoBehaviour
{
    private bool _onMouseDraging = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            _onMouseDraging = true;
        if (Input.GetMouseButtonUp(0))
            _onMouseDraging = false;
        if (_onMouseDraging)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit, 100))
            {
                transform.position = raycastHit.point + Vector3.up * 4;
            }
        }
    }
}

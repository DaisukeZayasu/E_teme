using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class whalerotate : MonoBehaviour
{
    [SerializeField] float rotate;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Horizontal") > 0)
        {

            transform.Rotate(new Vector3(0, rotate, 0));
        }
        if (Input.GetAxis("Horizontal") < 0)
        {

            transform.Rotate(new Vector3(0, -rotate, 0));
        }
    }
}

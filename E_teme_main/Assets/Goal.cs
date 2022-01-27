using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour
{
    public GameObject sea;
    public GameObject Clear;

    private void Start()
    {
        Clear.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == sea.name)
        {
            Clear.GetComponent<Text>();
            Clear.SetActive(true);
        }
    }
}

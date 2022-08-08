using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public float x;
    public float y;

    private void Update()
    {
        gameObject.transform.position = new Vector3(x, y, 0);
    }
}

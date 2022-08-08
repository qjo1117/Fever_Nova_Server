using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public float x;
    public float y;
    public float m_r;
    private void Update()
    {
        gameObject.transform.SetPositionAndRotation(new Vector3(x, y, 0), Quaternion.Euler(0, m_r, 0));
    }
}

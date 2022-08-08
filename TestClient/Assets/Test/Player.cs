using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PacketMoveData moveData;
    private void Update()
    {
        gameObject.transform.SetPositionAndRotation(
            new Vector3(moveData.m_positionX, moveData.m_positionY, moveData.m_positionZ), 
            new Quaternion(moveData.m_rotationX, moveData.m_rotationY, moveData.m_rotationZ, moveData.m_rotationW));
    }
}

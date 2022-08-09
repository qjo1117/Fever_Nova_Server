using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PacketMoveData moveData;

    private void Update()
    {
        gameObject.transform.SetPositionAndRotation(
            new Vector3(moveData.m_position.x, moveData.m_position.y, moveData.m_position.z),
            new Quaternion(moveData.m_rotation.x, moveData.m_rotation.y, moveData.m_rotation.z, moveData.m_rotation.w));
    }
}

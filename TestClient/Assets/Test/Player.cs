using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool isJump;
    public Rigidbody m_rigidbody;
    public PacketMoveData moveData;
    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            m_rigidbody.angularVelocity = Vector3.zero;
            isJump = false;
        }
    }

    public void PositionAndRotationWrite(ref PacketMoveData _data)
    {
        _data.m_position.x = transform.position.x;
        _data.m_position.y = transform.position.y;
        _data.m_position.z = transform.position.z;

        _data.m_rotation.x = transform.rotation.x;
        _data.m_rotation.y = transform.rotation.y;
        _data.m_rotation.z = transform.rotation.z;
        _data.m_rotation.w = transform.rotation.w;
    }
    //public void RotationPairing()
    //{
    //    gameObject.transform.rotation = new Quaternion(moveData.m_rotation.x, moveData.m_rotation.y, moveData.m_rotation.z, moveData.m_rotation.w);
    //}
    //public void PositionPairing()
    //{
    //    gameObject.transform.position = new Vector3(moveData.m_position.x, moveData.m_position.y, moveData.m_position.z);
    //}
    public void PositionAndRotationRead(in PacketMoveData _data)
    {
        moveData = _data;
        m_rigidbody.angularVelocity = Vector3.zero;
        gameObject.transform.SetPositionAndRotation(
            new Vector3(_data.m_position.x, _data.m_position.y, _data.m_position.z),
            new Quaternion(_data.m_rotation.x, _data.m_rotation.y, _data.m_rotation.z, _data.m_rotation.w));
    }
}

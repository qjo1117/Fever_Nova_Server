using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // 이속, 회전 속도
    public int id;
    public const float moveSpeed = 20.0f;
    public const float rotateSpeed = 50.0f;
    public const float jumpPower = 50.0f;

    public Rigidbody rigbody;
    public Transform trans;

    public Vector3 currPos = Vector3.zero;
    public Quaternion currRot = Quaternion.identity;

    public bool isJump = false;


    public Animator animaotr;
    private void Awake()
    {
        rigbody = GetComponent<Rigidbody>();
        trans = GetComponent<Transform>();
        rigbody.centerOfMass = new Vector3(0.0f, -0.5f, 0.0f);// 무게중심 변경
        currPos = trans.position;
        currRot = trans.rotation;
        animaotr = GetComponentInChildren<Animator>();
    }

    private void Update()
    { 
        trans.position = Vector3.Lerp(trans.position, currPos, Time.deltaTime * moveSpeed);
        trans.rotation = Quaternion.Slerp(trans.rotation, currRot, Time.deltaTime * rotateSpeed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            rigbody.velocity = Vector3.zero;
            rigbody.angularVelocity = Vector3.zero;
            isJump = false;
        }
    }

    //public void PositionAndRotationWrite(ref PacketMoveData _data)
    //{
    //    _data.m_position.x = transform.position.x;
    //    _data.m_position.y = transform.position.y;
    //    _data.m_position.z = transform.position.z;

    //    _data.m_rotation.x = transform.rotation.x;
    //    _data.m_rotation.y = transform.rotation.y;
    //    _data.m_rotation.z = transform.rotation.z;
    //    _data.m_rotation.w = transform.rotation.w;
    //}
    //public void RotationPairing()
    //{
    //    gameObject.transform.rotation = new Quaternion(moveData.m_rotation.x, moveData.m_rotation.y, moveData.m_rotation.z, moveData.m_rotation.w);
    //}
    //public void PositionPairing()
    //{
    //    gameObject.transform.position = new Vector3(moveData.m_position.x, moveData.m_position.y, moveData.m_position.z);
    //}
    //public void PositionAndRotationRead(in PacketMoveData _data)
    //{
    //    moveData = _data;
    //    m_rigidbody.angularVelocity = Vector3.zero;
    //    gameObject.transform.SetPositionAndRotation(
    //        new Vector3(_data.m_position.x, _data.m_position.y, _data.m_position.z),
    //        new Quaternion(_data.m_rotation.x, _data.m_rotation.y, _data.m_rotation.z, _data.m_rotation.w));
    //}
}

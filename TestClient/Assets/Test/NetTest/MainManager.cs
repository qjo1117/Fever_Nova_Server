using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{

    private static MainManager m_instance = null;
    public static MainManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject mainM = GameObject.Find("@MainManager");
                m_instance = mainM.GetComponent<MainManager>();
            }
            return m_instance;
        }
    }


    private NetWorkManager m_network = new NetWorkManager();
    public static NetWorkManager Network { get => Instance.m_network; }




    private void Awake()
    {
        Screen.SetResolution(900, 600, false);
    }
    public FollowCam followCam;
    public GameObject playerUnit;
    public Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();

    #region 플레이어 컨트롤 변수
    public Player mainPlayer;
    float X;
    float Z;
    bool walkDown;
    bool runDown;
    bool jumpDown;
    public Vector3 lookDir;
    public Vector3 mainPlayerPos;
    public Quaternion mainPlayerRot;

    public PacketMoveData m_mainPlayerData;
    #endregion

    #region 플레이어 컨트롤 함수
    void GetInput()
    {
        X = Input.GetAxis("Horizontal");
        Z = Input.GetAxis("Vertical");
        walkDown = Input.GetKey(KeyCode.LeftShift);
        jumpDown = Input.GetKeyDown(KeyCode.Space);

        if(Input.GetKeyDown(KeyCode.T))
        {
            m_network.Session.Write((int)E_PROTOCOL.Test);
        }
    }
    void Move()
    {
        lookDir = new Vector3(X, 0, Z).normalized;
        runDown = lookDir != Vector3.zero ? true : false;
        if (walkDown)
        {
            mainPlayerPos = Vector3.Lerp(mainPlayerPos, mainPlayerPos + lookDir, Time.deltaTime * Player.moveSpeed * 0.3f);
        }
        else
        {
            mainPlayerPos = Vector3.Lerp(mainPlayerPos, mainPlayerPos + lookDir, Time.deltaTime * Player.moveSpeed);

        }
        //mainPlayerPos += lookDir * Player.moveSpeed * Time.deltaTime;
    }
    void Turn()
    {
        if (lookDir != Vector3.zero)
        {
            mainPlayerRot = Quaternion.LookRotation(lookDir);
        }
    }
    void Jump()
    {
        if (jumpDown && !mainPlayer.isJump)
        {
            // 브로드 케스트로 AddForce 점프

            //mainPlayer.rigbody.AddForce(Vector3.up * Player.jumpPower, ForceMode.Impulse);
            mainPlayer.isJump = true;
        }
    }

    public List<TestListData> TestL;
    void TestProcess()
    {
        m_network.Session.GetListData<TestListData>(out TestL);
    }
    #endregion

    

    void Start()
    {
        m_network.Register(E_PROTOCOL.STC_SPAWN, SpawnProcess);
        m_network.Register(E_PROTOCOL.STC_MOVE, MoveProcess);
        m_network.Register(E_PROTOCOL.STC_OUT, OutProcess);
        m_network.Register(E_PROTOCOL.Test, TestProcess);
        m_network.Initialize();
    }
    void Update()
    {
        #region 플레이어 컨트롤러
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            m_network.End();
            //m_network.protocolThread.Join();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit(); // 어플리케이션 종료
#endif
        }
        if (mainPlayer != null)
        {
            GetInput();
            Move();
            Turn();
            Jump();
        }

        #endregion

        #region 메세지 처리 루프
        m_network.UpdateRecvProcess();
        #endregion
    }


    const int m_SendTimeCounterF = 5;
    int m_sendTimeCounter = 0;
    private void FixedUpdate()
    {
        if (mainPlayer != null)
        {
            if (m_SendTimeCounterF <= m_sendTimeCounter)
            {
                m_mainPlayerData.m_position.x = mainPlayerPos.x;
                m_mainPlayerData.m_position.y = mainPlayerPos.y;
                m_mainPlayerData.m_position.z = mainPlayerPos.z;

                m_mainPlayerData.m_rotation.x = mainPlayerRot.x;
                m_mainPlayerData.m_rotation.y = mainPlayerRot.y;
                m_mainPlayerData.m_rotation.z = mainPlayerRot.z;
                m_mainPlayerData.m_rotation.w = mainPlayerRot.w;

                if (walkDown && runDown)
                {
                    m_mainPlayerData.m_state = 1;
                }
                else if (runDown)
                {
                    m_mainPlayerData.m_state = 2;
                }
                else
                {
                    m_mainPlayerData.m_state = 0;
                }

                m_network.Session.Write((int)E_PROTOCOL.CTS_MOVE, m_mainPlayerData);
                m_sendTimeCounter = 0;
            }
            else
            {
                ++m_sendTimeCounter;
            }
        }
    }

    /*=================<기능 함수(TEST용 -> 클라에 적용시에 각자 해당하는 메니저에서 유사한 기능의 함수를 제작하는 것이 좋음)>=====================*/
    void SpawnProcess()
    {
        ListData liddata;
        m_network.Session.GetData<ListData>(out liddata);

        for (int i = 0; i < liddata.m_size; i++)
        {
            if (liddata.m_list[i] == -1)
            {
                continue;
            }
            if (!players.ContainsKey(liddata.m_list[i]))
            {
                GameObject temp = GameObject.Instantiate(playerUnit, Vector3.zero, Quaternion.identity);
                temp.GetComponent<Player>().id = liddata.m_list[i];
                players.Add(liddata.m_list[i], temp);
                temp.SetActive(true);
                if (liddata.m_list[i] != m_network.ClientId)
                {
                    temp.GetComponent<Player>().rigbody.isKinematic = true;
                }
                else
                {
                    temp.GetComponent<Player>().rigbody.isKinematic = false;
                }

            }
        }
        if (mainPlayer == null)
        {
            m_mainPlayerData.m_id = m_network.ClientId;
            m_mainPlayerData.m_state = 0;
            m_mainPlayerData.m_move.x = 15;
            m_mainPlayerData.m_move.y = 25;
            m_mainPlayerData.m_animing = 35;



            mainPlayer = players[m_network.ClientId].GetComponent<Player>();

            mainPlayerPos = mainPlayer.currPos;
            mainPlayerRot = mainPlayer.currRot;

            followCam.target = mainPlayer.transform;
        }
    }
    void MoveProcess()
    {
        PacketMoveData lData;
        m_network.Session.GetData<PacketMoveData>(out lData);
        if (players.ContainsKey(lData.m_id))
        {
            players[lData.m_id].GetComponent<Player>().currPos =
                new Vector3(lData.m_position.x, lData.m_position.y, lData.m_position.z);
            players[lData.m_id].GetComponent<Player>().currRot =
                new Quaternion(lData.m_rotation.x, lData.m_rotation.y, lData.m_rotation.z, lData.m_rotation.w);
            if (lData.m_state == 1)
            {
                players[lData.m_id].GetComponent<Player>().animaotr.SetBool("IsRun", true);
                players[lData.m_id].GetComponent<Player>().animaotr.SetBool("IsWalk", true);
            }
            else if (lData.m_state == 2)
            {
                players[lData.m_id].GetComponent<Player>().animaotr.SetBool("IsRun", true);
                players[lData.m_id].GetComponent<Player>().animaotr.SetBool("IsWalk", false);
            }
            else
            {
                players[lData.m_id].GetComponent<Player>().animaotr.SetBool("IsRun", false);
                players[lData.m_id].GetComponent<Player>().animaotr.SetBool("IsWalk", false);
            }

        }
    }
    void OutProcess()
    {
        IDData liddata;
        m_network.Session.GetData<IDData>(out liddata);
        Destroy(players[liddata.m_id]);
        players.Remove(liddata.m_id);
    }
    /*======================================*/
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;


public class GameManager : MonoBehaviour
{
    public int m_NumRoundsToWin = 5;
    public float m_StartDelay = 3f;
    public float m_EndDelay = 3f;
    public Text m_MessageText;
    public GameObject m_TankPrefab;
    public TankManager[] m_Tanks;
    public GameObject m_P1Camera;
    public GameObject m_P2Camera;


    private int m_RoundNumber;
    private WaitForSeconds m_StartWait;
    private WaitForSeconds m_EndWait;
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;


    private void Start()
    {
        m_StartWait = new WaitForSeconds (m_StartDelay);
        m_EndWait = new WaitForSeconds (m_EndDelay);

        SpawnAllTanks();

        StartCoroutine (GameLoop ());
    }


    private void SpawnAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].m_Instance =
                Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
            m_Tanks[i].m_PlayerNumber = i + 1;
            m_Tanks[i].Setup();
            if (i == 0)
            {
                var vc = m_P1Camera.GetComponent<CinemachineVirtualCamera>();
                vc.Follow = m_Tanks[0].m_Instance.transform;
                vc.LookAt = m_Tanks[0].m_Instance.transform; 
                m_P1Camera.transform.position = m_Tanks[0].m_Instance.transform.Find("CameraTransform").position;
                m_P1Camera.transform.rotation = m_Tanks[0].m_Instance.transform.Find("CameraTransform").rotation;
            }                
            else if (i == 1)
            {
                var vc = m_P2Camera.GetComponent<CinemachineVirtualCamera>();
                vc.Follow = m_Tanks[1].m_Instance.transform;
                vc.LookAt = m_Tanks[1].m_Instance.transform; 
                m_P2Camera.transform.position = m_Tanks[1].m_Instance.transform.Find("CameraTransform").position;
                m_P2Camera.transform.rotation = m_Tanks[1].m_Instance.transform.Find("CameraTransform").rotation;
            }
        }
    }


    private IEnumerator GameLoop ()
    {
        yield return StartCoroutine (RoundStarting ());

        yield return StartCoroutine (RoundPlaying());

        yield return StartCoroutine (RoundEnding());

        if (m_GameWinner != null)
        {
            SceneManager.LoadScene (0);
        }
        else
        {
            StartCoroutine (GameLoop ());
        }
    }


    private IEnumerator RoundStarting ()
    {
        ResetAllTanks ();
        DisableTankControl ();

        m_RoundNumber++;
        m_MessageText.text = "Раунд " + m_RoundNumber;

        yield return m_StartWait;
    }


    private IEnumerator RoundPlaying ()
    {
        EnableTankControl ();

        m_MessageText.text = string.Empty;

        while (!OneTankLeft())
        {
            yield return null;
        }
    }


    private IEnumerator RoundEnding ()
    {
        DisableTankControl ();

        m_RoundWinner = null;

        m_RoundWinner = GetRoundWinner ();

        if (m_RoundWinner != null)
            m_RoundWinner.m_Wins++;

        m_GameWinner = GetGameWinner ();

        string message = EndMessage ();
        m_MessageText.text = message;

        yield return m_EndWait;
    }

    private bool OneTankLeft()
    {
        int numTanksLeft = 0;

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft <= 1;
    }
        

    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                return m_Tanks[i];
        }

        return null;
    }

    private TankManager GetGameWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];
        }
        return null;
    }

    private string EndMessage()
    {
        string message = "Ничья!";

        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " побеждает в раунде!";

        message += "\n\n\n\n";

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " побед\n";
        }

        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " побеждает в игре!";

        return message;
    }

    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
    }


    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }


    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }
}

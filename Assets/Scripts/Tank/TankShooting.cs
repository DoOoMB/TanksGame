using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;              
    public GameObject m_ShellExplosionPrefab;                   
    public Transform m_FireTransform;           
    public AudioSource m_ShootingAudio;         
    public AudioClip m_ChargingClip;            
    public AudioClip m_FireClip;                    

    public float m_ReloadCooldown = 2f;
    private float m_CurrentCooldownTime = 2f;
    private string m_FireButton;                

    private void Start ()
    {
        // The fire axis is based on the player number.
        m_FireButton = "Fire" + m_PlayerNumber;

    }


    private void Update ()
    {
        if (Input.GetButtonDown(m_FireButton) && m_CurrentCooldownTime >= m_ReloadCooldown)
        {
            Fire();
            m_ShootingAudio.clip = m_ChargingClip;
            m_ShootingAudio.Play ();
            m_CurrentCooldownTime = 0f;
        }
        if (m_ReloadCooldown >= m_CurrentCooldownTime) m_CurrentCooldownTime += Time.deltaTime;
    }


    private void Fire ()
    {
        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();
        Ray ray = new Ray(m_FireTransform.position, m_FireTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit Hit))
        {
            GameObject explosionInstance = 
                Instantiate(m_ShellExplosionPrefab, Hit.point, m_FireTransform.rotation);
        }
    }

    private void OnDrawGizmos()
    {
        Ray ray = new Ray(transform.position, m_FireTransform.forward);
        Gizmos.color = Color.green;
        Physics.Raycast(ray, out RaycastHit Hit);
        Debug.DrawRay(ray.origin, ray.direction, Color.green);
        Gizmos.DrawSphere(Hit.point, 1f);
    }
}

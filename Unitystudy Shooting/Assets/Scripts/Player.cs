using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    //player
    public Transform    m_trans;              
    CharacterController m_chara;           
    public float               m_movSpeed  = 3.0f;   
    float               m_gravity   = 2.0f;   
    public int          m_life      = 5;
    //camera
    Transform           m_camTrans;                     
    Vector3             m_camRot;             
    float               m_camHeight = 1.4f;
    //gun
    Transform m_gunpoint;
    //shoot mask
    public LayerMask m_layer;
    //shoot effect
    public Transform m_fx;
    //audio
    public AudioClip m_audio;
    //shoot timer
    float m_shootTimer;



	void Start () {
        //get player COM
        m_trans = this.transform;
        m_chara = this.GetComponent<CharacterController>();
        //set camera
        m_camTrans = Camera.main.transform;
        m_camRot = m_camTrans.eulerAngles;
        //lock mouse
        Cursor.lockState = CursorLockMode.Locked;
        //get gunpoint
        m_gunpoint = m_camTrans.Find("M16/weapon/gunpoint").transform;
	}
	
	


	void Update () {
        if (m_life <= 0) return;
        ControlMove();
        ControlCamera();
        RenewShootTimer();
        PlayerShoot();
    }



    private void OnDrawGizmos() {
        DrawIconForPlayer();
    }




    public void OnDamage(int damage) {
        m_life -= damage;
        //renewUI
        GameManager.Instance.RenewLife(m_life);
        //cancel lock mouse
        if (m_life <= 0)
            Cursor.lockState = CursorLockMode.None;
    }

    #region Extra Method

    void ControlMove() {
        float xm = 0, ym = 0, zm = 0;                                       //var to control move
        ym = calcuGravity(ym);                                              //calcu y
        InputMoveDir(ref xm, ref zm);                                       //calcu WASD
        m_chara.Move(m_trans.TransformDirection(new Vector3(xm, ym, zm)));  //actual move
    }

    private float calcuGravity(float ym) {
        //gravity act
        ym -= m_gravity * Time.deltaTime;
        return ym;
    }

    private void InputMoveDir(ref float xm, ref float zm) {
        //input
        if (Input.GetKey(KeyCode.W)) {
            zm += m_movSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S)) {
            zm -= m_movSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A)) {
            xm -= m_movSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D)) {
            xm += m_movSpeed * Time.deltaTime;
        }
    }

    private void DrawIconForPlayer() {
        Gizmos.DrawIcon(m_trans.position, "Spawn.tif");
    }

    private void ControlCamera() {
        float rh, rv;
        GetMouseMoveDis(out rh, out rv);
        RotatingCamera(rh, rv);
        SyncPlayerfaceToCameraface();
        RenewCameraPos();
    }

    private void RenewCameraPos() {
        //renew camera pos
        m_camTrans.position = m_trans.TransformPoint(0, m_camHeight, 0);
    }

    private void SyncPlayerfaceToCameraface() {
        //sync playerFace to cameraFace
        Vector3 camrot = m_camRot;
        camrot.x = 0; camrot.z = 0;
        m_trans.eulerAngles = camrot;
    }

    private void RotatingCamera(float rh, float rv) {
        //rotating camera
        m_camRot.x -= rv;
        m_camRot.y += rh;
        m_camTrans.eulerAngles = m_camRot;
    }

    private static void GetMouseMoveDis(out float rh, out float rv) {
        //get mouse moveDis
        rh = Input.GetAxis("Mouse X");
        rv = Input.GetAxis("Mouse Y");
    }

    private void PlayerShoot() {
        if (!IsShoot()) return;
        ResetShootTimer();
        PlayShootClip();
        GameManager.Instance.RenewAmmo(1);                  //sub ammo
        RaycastHit info;
        bool hit = IsHit(out info);
        if (!hit) return;
        //shoot suceed
        if (IsShootTarget(ref info)) {
            Enemy enemy = info.transform.GetComponent<Enemy>();
            enemy.OnDamage(1);
        }
        ShootEffect(info);
    }

    private void ShootEffect(RaycastHit info) {
        //create effect
        Instantiate(m_fx, info.point, info.transform.rotation);
    }

    private static bool IsShootTarget(ref RaycastHit info) {
        return info.transform.tag.CompareTo("enemy") == 0;
    }

    private void RenewShootTimer() {
        //renew shoot timer
        m_shootTimer -= Time.deltaTime;
    }

    private bool IsHit(out RaycastHit hitinfo) {

        return Physics.Raycast(m_gunpoint.position, m_camTrans.TransformDirection(Vector3.forward), out hitinfo, 100, m_layer);
    }
    private void ResetShootTimer() {
        m_shootTimer = 0.1f;
    }

    private void PlayShootClip() {
        this.GetComponent<AudioSource>().PlayOneShot(m_audio);
    }

    private bool IsShoot() {
        return Input.GetMouseButton(0) && m_shootTimer <= 0;
    }
    #endregion
}

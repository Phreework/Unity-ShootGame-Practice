using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {

    Transform       m_trans;
    Player          m_player;
    NavMeshAgent    m_agent;
    Animator        m_ani;
    public float    m_movSpd = 2.5f;
    public float    m_rotSpd = 5.0f;
    float           m_timer = 2;    //a timer
    public int      m_life = 15;
    protected       EnemySpawn m_spawn;
	// Use this for initialization
	void Start () {
        //get COM
        m_trans = this.transform;
        m_ani = this.GetComponent<Animator>();

        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        m_agent = GetComponent<NavMeshAgent>();
        //set nav agent
        m_agent.speed = m_movSpd;
        m_agent.SetDestination(m_player.m_trans.position);          //get findRoad COM
	}

    // Update is called once per frame
    void Update () {
        //player dead
        if (m_player.m_life <= 0) return;
        //renew timer
        m_timer -= Time.deltaTime;
        //get animation state
        AnimatorStateInfo stateInfo = m_ani.GetCurrentAnimatorStateInfo(0);


        StateIdle(stateInfo);

        StateRun(stateInfo);

        StateAttack(stateInfo);

        StateDeath(stateInfo);
    }



    public void OnDamage(int damage) {
        m_life -= damage;
        if (m_life <= 0) {
            m_ani.SetBool("death", true);
            m_agent.ResetPath();
        }
    }
    public void Init(EnemySpawn spawn) {
        m_spawn = spawn;
        m_spawn.m_enemyCount++;
    }








    #region Extra Method
    #region state
    void StateIdle(AnimatorStateInfo stateInfo) {
        //idle and not in turning
        if (WhatAnimationIs(stateInfo, "Base Layer.idle")) {
            m_ani.SetBool("idle", false);
            if (m_timer > 0) return;
            if (PlayerIntoAttackRange()) {
                StopAndIntoAttackState();
            } else {
                ResetTimer(1);
                AgentFindPlayer();
                m_ani.SetBool("run", true);
            }
        }
    }
    private void StateAttack(AnimatorStateInfo stateInfo) {
        //attack and not in turning
        if (WhatAnimationIs(stateInfo, "Base Layer.attack")) {
            //face player
            RotateTo();
            m_ani.SetBool("attack", false);
            //animation end,to idle
            if (stateInfo.normalizedTime >= 1.0f) {
                m_ani.SetBool("idle", true);
                ResetTimer(2); 
                m_player.OnDamage(1);
            }

        }
    }

    private void StateRun(AnimatorStateInfo stateInfo) {
        //run and not in turning
        if (WhatAnimationIs(stateInfo, "Base Layer.run")) {
            m_ani.SetBool("run", false);
            if (m_timer < 0) {
                AgentFindPlayer();
                ResetTimer(1);
            }
            //if dis<1.5 attack
            if (PlayerIntoAttackRange()) {
                StopAndIntoAttackState();
            }
        }
    }
    private void StateDeath(AnimatorStateInfo stateInfo) {
        //run and not in turning
        if (WhatAnimationIs(stateInfo, "Base Layer.death")) {
            m_ani.SetBool("death", false);
            if (stateInfo.normalizedTime>=1.0f) {
                AddScore();
                EnemyCountChange();
                Destroy(this.gameObject);
            }
        }
    }

    private void EnemyCountChange() {
        m_spawn.m_enemyCount--;
    }


    #endregion
    private static void AddScore() {
        GameManager.Instance.RenewScore(100);
    }
    void RotateTo() {
        Vector3 targetdir = m_player.m_trans.position - m_trans.position;   //get target dir
        Vector3 newDir = Vector3.RotateTowards(transform.forward,targetdir,m_rotSpd*Time.deltaTime,0.0f);   //calcu new dir
        m_trans.rotation = Quaternion.LookRotation(newDir);     //rotate to new dir
    }
    private bool WhatAnimationIs(AnimatorStateInfo stateInfo, string AniState) {
        return stateInfo.fullPathHash == Animator.StringToHash(AniState) && !m_ani.IsInTransition(0);
    }
    private bool PlayerIntoAttackRange() {
        return Vector3.Distance(m_trans.position, m_player.m_trans.position) <= 1.5f;
    }
    private void ResetTimer(float time) {
        m_timer = time;
    }

    private void StopAndIntoAttackState() {
        m_agent.ResetPath();
        m_ani.SetBool("attack", true);
    }
    private void AgentFindPlayer() {
        m_agent.SetDestination(m_player.m_trans.position);
    }
    #endregion
}

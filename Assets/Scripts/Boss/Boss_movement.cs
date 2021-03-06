using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Boss_movement : StateMachineBehaviour
{
    public float attackDistance;
    public float walkDistance;

    private float _animationBlend;
    private float _targetSpeed;

    private Transform _player;
    private Animator _animator;
    private Boss _boss;
    private NavMeshAgent _navMeshAgent;

    private string lastTrigger = "";
    
    private int _speedIdHash = Animator.StringToHash("Speed");

    [SerializeField] private List<string> possibleAttacks;
    
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _animator = animator;
        _boss = animator.GetComponent<Boss>();
        _navMeshAgent = animator.GetComponent<NavMeshAgent>();
        _navMeshAgent.isStopped = false;
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        SpeedOnPlayerDistance();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _boss.LookAtTarget(_player.transform.position);
        
        float distance = SpeedOnPlayerDistance();

        float cooldown = animator.GetFloat("Cooldown");

        if (cooldown <= 0 && distance <= attackDistance)
        {
            _navMeshAgent.ResetPath();
            var nextAttack = possibleAttacks[Random.Range(0, possibleAttacks.Count)];
            _boss.Attack(nextAttack);
            lastTrigger = nextAttack;
        }
        else
        {
            animator.SetFloat("Cooldown", cooldown - Time.deltaTime);
            
            if (distance > attackDistance)
            {
                Vector3 playerPos = _player.transform.position;
               _navMeshAgent.SetDestination(playerPos);
            }
            else
            {
                _navMeshAgent.ResetPath();
            }
        }

        _animationBlend = Mathf.Lerp(_animationBlend, _targetSpeed, Time.deltaTime);
        
        _animator.SetFloat(_speedIdHash, _animationBlend);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _navMeshAgent.isStopped = true;
        _animator.ResetTrigger(lastTrigger);
    }

    private float SpeedOnPlayerDistance()
    {
        float distanceToPlayer = Vector3.Distance(_animator.transform.position, _player.transform.position);

        if (distanceToPlayer > walkDistance)
            _targetSpeed = 1f;
        else if (distanceToPlayer > attackDistance)
            _targetSpeed = 0.5f;
        else
            _targetSpeed = 0f;

        return distanceToPlayer;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}

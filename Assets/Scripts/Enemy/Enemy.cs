using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public abstract class Enemy : MonoBehaviour
{
    public static event Action<Enemy> OnEnemyCreated = delegate { };

    protected Stats Stats;
    private Hittable _hittable;
    private Damageable _damageable;

    [SerializeField] protected GameObject enemyHead;
    [SerializeField] protected float viewDistance = 5f;
    [SerializeField] protected float fieldOfView = 70f;
    protected float initialFOV;

    [SerializeField] protected float chaseSpeed = 3f;
    [SerializeField] protected float walkSpeed = 1.5f;
    [SerializeField] protected float chaseCooldown = 5f;

    [SerializeField] protected Transform _spawn;
    protected Quaternion InitialOrientation;

    protected Transform PlayerTransform;
    protected ThirdPersonController Player;

    protected NavMeshAgent NavMeshAgent;

    protected float AnimationBlend;
    public bool backstabbed { get; private set; }
    protected bool mindControlled;
    public float speedChangeRate = 10.0f;

    protected Animator _animator;

    [SerializeField] private List<string> targetsWhileMindControlled = new List<string>{ "Enemy" };
    [SerializeField] private List<string> normalTargets = new List<string> { "Player", "MindControlled" };

    protected void Start()
    {
        Stats = GetComponent<Stats>();
        _hittable = GetComponent<Hittable>();
        _damageable = GetComponent<Damageable>();
        _damageable.InitializeMaxHealth((int) Stats.GetStatValue(StatName.Health));

        NavMeshAgent = GetComponent<NavMeshAgent>();
        PlayerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        
        Player = PlayerTransform.GetComponent<ThirdPersonController>();

        _spawn.position = transform.position;
        InitialOrientation = transform.rotation;
        mindControlled = false;
        backstabbed = false;
        initialFOV = fieldOfView;

        _animator = GetComponent<Animator>();
        
        OnEnemyCreated.Invoke(this);
    }

    protected void Update()
    {
        AnimationBlend = Mathf.Lerp(AnimationBlend, NavMeshAgent.speed, Time.deltaTime * speedChangeRate);
    }

    protected bool DetectTarget()
    {
        Vector3? targetPos = GetTargetPos();
        if (targetPos == null) return false;

        Vector3 rayDirection = (Vector3) targetPos - enemyHead.transform.position;

        // if target is within view distance
        if (Vector3.Magnitude(rayDirection) > viewDistance)
            return false;
        
        // target not inside field of view cone
        if (Vector3.Angle(transform.forward, new Vector3(rayDirection.x, 0f, rayDirection.z)) > fieldOfView / 2.0f) 
            return false;
        
        // and no obstacles in the way
        if (Physics.Raycast(enemyHead.transform.position, rayDirection, out var hit, viewDistance))
        {
            if (!mindControlled)
            {
                return normalTargets.Contains(hit.transform.tag);
            }
            return targetsWhileMindControlled.Contains(hit.transform.tag);
        }
        
        return false;
    }

    protected Vector3? GetTargetPos()
    {
        var headPos = Player.playerHeadTransform.position;
        var playerPos = 
            new Vector3(headPos.x, headPos.y - 0.5f,
                headPos.z); // TODO: If the y position is too high, enemy aggro messes up and if the player stays still and a melee enemy gets near, the melee enemy slides inside the player
        
        if (!mindControlled)
        {
            return GetClosestWithTags(normalTargets).position + new Vector3(0, 0.5f, 0);
        }

        Transform closestEnemy = GetClosestWithTags(targetsWhileMindControlled);

        if (closestEnemy != null)
        {
            return closestEnemy.position + new Vector3(0, 0.5f, 0);
        }

        return null;
    }

    private Transform GetClosestWithTags(List<string> tags)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (string tag in tags)
        {
            foreach(GameObject potentialTarget in GameObject.FindGameObjectsWithTag(tag))
            {
                if(GameObject.ReferenceEquals(this.gameObject, potentialTarget)) continue;
                Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if(dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget.transform;
                }
            }
        }
        
        return bestTarget;
    }

    public void SetFOV(float fov)
    {
        fieldOfView = fov;
    }

    protected void LookAtTarget()
    {
        Vector3? targetPos = GetTargetPos();

        if (targetPos != null)
        {
            Quaternion rotation = transform.rotation;
            Quaternion lookRotation = Quaternion.Euler(rotation.eulerAngles.x,
                Quaternion.LookRotation((Vector3) targetPos - transform.position).eulerAngles.y,
                rotation.eulerAngles.z);

            // TODO: change hardcoded lerp speed
            rotation = Quaternion.Lerp(rotation, lookRotation,Time.deltaTime * 5f);
            transform.rotation = rotation;
        }
    }

    public void Backstab(float damage)
    {
        backstabbed = true;
        _animator.SetBool("Backstab", true);
        _hittable.Hit(damage);
    }

    public void EndBackstab()
    {
        _animator.SetBool("Backstab", false);
        backstabbed = false;
    }
    
    public void MindControl()
    {
        mindControlled = true;
        gameObject.tag = "MindControlled";
        ChangeTargetsMindControl(targetsWhileMindControlled);
        StopMovement();
    }

    protected abstract void ChangeTargetsMindControl(List<string> newTargets);

    public void SetupHealthBar(Canvas canvas)
    {
        GameObject healthBar = _damageable.healthBar.gameObject;

        healthBar.transform.SetParent(canvas.transform);

        // Use Vector3.down for rotation so that bar drains from right to left
        FaceCamera faceCamera = healthBar.AddComponent<FaceCamera>();
        faceCamera.targetCamera = Camera.main;
        faceCamera.worldUp = Vector3.down;

        FollowTarget follow = healthBar.AddComponent<FollowTarget>();
        follow.target = transform;
        follow.offset = Vector3.up * 1.9f; // TODO: use enemy height to determine health bar position
    }

    public abstract void StopMovement();
}

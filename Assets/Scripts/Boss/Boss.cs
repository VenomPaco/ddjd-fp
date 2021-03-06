using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] private int _maxHealth;

    private Damageable _damageable;

    private Animator _animator;
    private AttackerBoss _attacker;

    public float defaultAttackCooldown = 2f;

    public bool HasNextPhase = false;

    [SerializeField] private GameObject nextPhaseCutscene;
    
    // Start is called before the first frame update
    void Start()
    {
        _damageable = GetComponent<Damageable>();
        _damageable.InitializeMaxHealth(_maxHealth);

        _animator = GetComponent<Animator>();
        _attacker = GetComponent<AttackerBoss>();

        _animator.SetFloat("Cooldown", defaultAttackCooldown);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Attack(string nextAttack)
    {
        _animator.SetTrigger(nextAttack);
    }

    public void LookAtTarget(Vector3 target)
    {
        Quaternion lookRotation = Quaternion.Euler(transform.rotation.eulerAngles.x,
            Quaternion.LookRotation(target - transform.position).eulerAngles.y,
            transform.rotation.eulerAngles.z);
        transform.rotation =
            Quaternion.Lerp(transform.rotation, lookRotation,
                Time.deltaTime * 5f);
    }

    public void LookAtPlayer()
    {
        LookAtTarget(GameObject.FindWithTag("Player").transform.position);
    }

    public void Reset()
    {
        _damageable.RestoreToMaxHealth();
    }

    public void ChangePhase()
    {
        if (HasNextPhase)
        {
            nextPhaseCutscene.SetActive(true);
        }
        else
            _animator.SetTrigger("Die");
    }
}

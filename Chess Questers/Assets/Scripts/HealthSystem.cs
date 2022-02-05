using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthSystem : MonoBehaviour
{

    [SerializeField] private UnityEvent onInit;
    [SerializeField] private UnityEvent onDamage;
    [SerializeField] private UnityEvent onDeath = new UnityEvent();

    public UnityEvent OnInit => onInit;
    public UnityEvent OnDamage => onDamage;
    public UnityEvent OnDeath => onDeath;


    public float MaxHealth;
    public float CurrentHealth;


    private void Awake()
    {
    }

    // Start is called before the first frame update
    private void Start()
    {
        // todo: set health properly.
        MaxHealth = 10;
        CurrentHealth = MaxHealth;
    }

    public bool ChangeHealth(float change)
    {
        if (change == 0)
        {
            return false;
        }

        CurrentHealth += change;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);

        OnDamage.Invoke();

        if (CurrentHealth == 0f)
        {
            OnDeath.Invoke();
        }

        return true;
    }



}

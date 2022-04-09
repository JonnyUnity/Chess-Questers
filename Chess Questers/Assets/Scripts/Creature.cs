using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    public int ID;

    public string Name { get; protected set; }
    public bool IsFriendly { get; protected set; }

    public int Health { get; protected set; }
    public int MaxHealth { get; protected set; }

    // Move Class properties
    public MoveClass MoveClass;
    public string MoveClassText { get; protected set; }

    public ActionClass[] Actions { get; protected set; }

    public int CharacterModel { get; protected set; }

    public GridCell OccupiedCell;
    protected Vector3 TargetPosition;
    public GridCell TargetCell;

    protected int TargetX;
    protected int TargetY;
    protected float MoveSpeed = 10f;

    public Vector3 Position;
    public int CellX;
    public int CellY;
    public int CurrentFacing;

    private Camera _cam;
    private ActionClass _selectedAction;

    public Transform Transform { get; protected set; }
    protected Quaternion _orientation;

    [SerializeField] protected GameObject _creatureInfo;
    [SerializeField] protected TMPro.TextMeshProUGUI _nameText;

    public CharacterStatesEnum State;

    private void Awake()
    {
        _cam = Camera.main;
        Transform = transform;
        _orientation = transform.rotation;

    }

    protected virtual void OnEnable()
    {
        BattleEvents.OnTakeDamage += TakeDamage;
    }

    protected virtual void OnDisable()
    {
        BattleEvents.OnTakeDamage -= TakeDamage;
    }

    private void TakeDamage(int enemyID, int healthChange)
    {
        if (ID != enemyID)
            return;

        Health -= healthChange; // assume no healing actions for now...

        if (Health <= 0)
        {
            Health = 0;
            // audio, fx to be handled by those subscribed to the event.
            BattleEvents.CharacterDied(ID, false);
            Destroy(gameObject);
        }
    }


    public void UpdatePosition(int cellX, int cellY, Vector3 position, int currentFacing)
    {
        CellX = cellX;
        CellY = cellY;
        Position = position;
        CurrentFacing = currentFacing;
    }


    protected virtual void Update()
    {

        _creatureInfo.transform.LookAt(_cam.transform);
        _creatureInfo.transform.rotation = _cam.transform.rotation;

        //if (State != CharacterStatesEnum.MOVING) return;

        //Vector3 direction = (TargetPosition - Transform.position).normalized;
        //Transform.position += MoveSpeed * Time.deltaTime * direction;

        //if (Vector3.Distance(transform.position, TargetPosition) < 0.1f)
        //{
        //    Transform.SetPositionAndRotation(TargetPosition, _orientation);

        //    //SetPosition(TargetX, TargetY);
        //    State = CharacterStatesEnum.IDLE;
        //}

    }

    public void DoMove(Vector3 position, int x, int y)
    {
        TargetX = x;
        TargetY = y;

        TargetPosition = position;
        State = CharacterStatesEnum.MOVING;
    }

    public virtual int GetAttackDamage()
    {
        return 0;
    }

}

//public enum CharacterStatesEnum
//{
//    IDLE,
//    MOVING,
//    ATTACKING
//}

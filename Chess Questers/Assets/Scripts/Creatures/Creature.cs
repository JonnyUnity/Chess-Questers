using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Creature : MonoBehaviour
{

    // Creature variables
    public string CreatureName { get; private set; }
    public int CurrentHP { get; private set; }
    public int MaxHP { get; private set; }
    public int Speed { get; private set; }
    public int Initiative { get; private set; }
    public bool IsEnemy { get; private set; }
    public MoveClass MoveClass { get; private set; }

    public Sprite PortraitSprite { get; private set; }

    public HealthSystem HealthSystem { get; private set; }

    // Grid variables
    public int X { private set; get; }
    public int Y { private set; get; }

    public CreatureStatesEnum State;

    private GameObject Body;
    private GameObject Head;

    private BattleSystem BattleSystem;

    private Vector3 TargetPosition;
    public GridCell TargetCell;

    private int TargetX;
    private int TargetY;
    private float MoveSpeed = 10f;

    [SerializeField] private GameObject SelectedCircle;

    public GridCell OccupiedCell;

    private Transform _transform;
    private Quaternion _orientation;

    public UnityEvent<Creature> OnDeath = new UnityEvent<Creature>();

    public void Awake()
    {
        Body = GameObject.Find("Body");
        Head = GameObject.Find("Head");
        _transform = transform;
        _orientation = transform.rotation;

        HealthSystem = GetComponent<HealthSystem>();
    }

    private void Start()
    {
        HealthSystem.OnDeath.AddListener(CreatureDied);
    }


    public void CreatureDied()
    {
        OnDeath.Invoke(this);
        Destroy(gameObject, 5f);
        
    }

    public void Init(string name, int hp, MoveClass moveClass, Sprite sprite, bool isEnemy, GridCell cell)
    {
        State = CreatureStatesEnum.IDLE;

        CreatureName = name;
        MaxHP = hp;
        CurrentHP = hp;
        MoveClass = moveClass;
        PortraitSprite = sprite;
        IsEnemy = isEnemy;
        OccupiedCell = cell;
        //SetPosition(cell.X, cell.Y);

       // cell.SetOccupied();
    }


    public void RollInitiative()
    {
        Initiative = Random.Range(1, 20);
    }


    public void SetColour(Color c)
    {
        Body.GetComponent<Renderer>().material.color = c;
        Head.GetComponent<Renderer>().material.color = c;
    }

    //public void SetPosition(int x, int y)
    //{
    //    X = x;
    //    Y = y;
    //}

    public Vector3 GetPosition()
    {
        return new Vector3(X, 0.1f, Y);
    }

    public int GetAttackDamage(AttackClass attack)
    {
        return 10;
    }


    public void ToggleSelected(bool toggle)
    {
        SelectedCircle.SetActive(toggle);
    }

    public void LookAtCell(Vector3 pos)
    {
        Quaternion lookRotation = Quaternion.LookRotation((pos - transform.position).normalized) * Quaternion.Euler(0, -90, 0);
        _transform.rotation = lookRotation;
    }

    public void Update()
    {

        if (State != CreatureStatesEnum.MOVING) return;

        Vector3 direction = (TargetPosition - _transform.position).normalized;
        _transform.position += MoveSpeed * Time.deltaTime * direction;

        if (Vector3.Distance(transform.position, TargetPosition) < 0.1f)
        {
            _transform.SetPositionAndRotation(TargetPosition, _orientation);

            //SetPosition(TargetX, TargetY);
            State = CreatureStatesEnum.IDLE;
        }
        
    }

  
    public void DoMove(Vector3 position, int x, int y)
    {
        TargetX = x;
        TargetY = y;

        TargetPosition = position;
        State = CreatureStatesEnum.MOVING;
    }

}

public enum CreatureStatesEnum
{
    IDLE,
    MOVING,
    ATTACKING
}

public enum MoveTypeEnum
{
    Queen,
    Bishop,
    Rook,
    Knight,
    Checker,
    Random
}

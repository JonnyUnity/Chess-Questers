using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Creature : MonoBehaviour
{
    public int ID;

    public string Name { get; protected set; }
    public bool IsFriendly { get; protected set; }

    public int Health { get; protected set; }
    public int MaxHealth { get; protected set; }
    public int ActionsPerTurn { get; protected set; }
    public int ActionsRemaining;

    // Move Class properties
    public NewBattleAction MoveAction;
    public string MoveClassText { get; protected set; }

    public List<NewBattleAction> Actions { get; protected set; }
    public List<NewBattleAction> AvailableActions
    {
        get
        {
            return Actions.Where(w => w.IsActive).ToList();
        }
    }

    public int CreatureModelID { get; protected set; }

    public GridCell OccupiedCell;
    protected Vector3 TargetPosition;
    public GridCell TargetCell;

    protected int TargetX;
    protected int TargetY;
    protected float MoveSpeed = 5f;
    protected float TotalMoveTime;
    private float CurrentMoveTime;

    private Animator _animator;


    public Vector3 Position;
    public int CellX;
    public int CellY;
    public int CurrentFacing;

    private Camera _cam;
    //private ActionClass _selectedAction;

    public Transform Transform { get; protected set; }
    protected Quaternion _orientation;

    [SerializeField] protected GameObject _creatureInfo;
    [SerializeField] protected Slider _healthSlider;
    [SerializeField] protected TMPro.TextMeshProUGUI _nameText;

    public Faction Faction;


    protected Sprite _portraitSprite;
    public Sprite PortraitSprite => _portraitSprite;

    public int Initiative { get; private set; }

    public CharacterStatesEnum State;

    private void Awake()
    {
        _cam = Camera.main;
        Transform = transform;
        _orientation = transform.rotation;
        Actions = new List<NewBattleAction>();
        _animator = GetComponent<Animator>();
    }

    protected virtual void OnEnable()
    {
        BattleEvents.OnTakeDamage += TakeDamage;
        //BattleEvents.OnCreatureMoved += UpdatePositionNew;
        BattleEvents.OnStartCombat += ResetActions;
        BattleEvents.OnPassTurn += PassTurn;
    }



    protected virtual void OnDisable()
    {
        BattleEvents.OnTakeDamage -= TakeDamage;
        //BattleEvents.OnCreatureMoved -= UpdatePositionNew;
        BattleEvents.OnStartCombat -= ResetActions;
        BattleEvents.OnPassTurn -= PassTurn;
    }

    public void ResetLook()
    {
        Transform.rotation = _orientation;
    }

    public void LookAtTarget(GridCell cell)
    {
        Transform.LookAt(cell.transform.position);
    }

    private void ResetActions()
    {
        MoveAction.StartOfBattle();
        foreach (var action in Actions)
        {
            action.StartOfBattle();
        }
        ActionsRemaining = ActionsPerTurn;
    }

    public void UpdateActionCooldowns()
    {
        MoveAction.EndOfTurn();
        foreach (var action in Actions)
        {
            action.EndOfTurn();
        }
    }


    public void SetInitiative(int initiative)
    {
        Initiative = initiative;
    }


    private void TakeDamage(Creature creature, int healthChange)
    {
        if (creature != this)
            return;

        Health -= healthChange; // assume no healing actions for now...
        //_healthSlider.value = Health;

        if (Health <= 0)
        {
            Health = 0;
            // audio, fx to be handled by those subscribed to the event.
            BattleEvents.CharacterDied(creature);
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

    public void UpdatePositionNew(Creature creature)
    {
        if (creature != this)
            return;


        CellX = TargetX;
        CellY = TargetY;
        Position = TargetPosition;

    }



    //protected virtual void Update()
    //{

    //    if (State != CharacterStatesEnum.MOVING) return;

    //    //Vector3 direction = (TargetPosition - Transform.position).normalized;
    //    //Transform.position += MoveSpeed * Time.deltaTime * direction;

    //    float t = CurrentMoveTime / TotalMoveTime;



    //    Transform.position = Vector3.Lerp(Transform.position, TargetPosition, t);
    //    CurrentMoveTime += Time.deltaTime;

    //    if (Vector3.Distance(Transform.position, TargetPosition) < 0.1f)
    //    {
    //        Transform.SetPositionAndRotation(TargetPosition, _orientation);

    //        UpdatePosition(TargetX, TargetY, TargetPosition, CurrentFacing);
    //        State = CharacterStatesEnum.IDLE;
    //    }

    //}

    public virtual void DoAction(ActionResult actionResult)
    {
        Debug.Log("Action started");
        BattleEvents.ActionStarted(actionResult.Action);

        StartCoroutine(DoActionCoroutine(actionResult));

        Debug.Log("Action finished");
        BattleEvents.ActionFinished();

        //var action = actionResult.Action;

        //Debug.Log("ACtion started!");
        //BattleEvents.ActionStarted(action);

        //if (action.IsMove)
        //{
        //    TargetX = actionResult.X;
        //    TargetY = actionResult.Y;
        //    TargetPosition = actionResult.Cell.Position;

        //    StartCoroutine(MoveCoroutine());

        //}

        //if (action.IsAction)
        //{
        //    Debug.Log("Actioning!");
        //    StartCoroutine(ActionCoroutine(actionResult));

        //    //foreach (Creature creature in actionResult.Creatures)
        //    //{
        //    //    BattleEvents.TakeDamage(creature, actionResult.Damage);
        //    //}

        //}

        //action.DoAction();

        //Debug.Log("Action finished");
        //BattleEvents.ActionFinished();

    }


    protected IEnumerator DoActionCoroutine(ActionResult actionResult)
    {
        var action = actionResult.Action;
        

        if (action.IsMove)
        {
            TargetX = actionResult.X;
            TargetY = actionResult.Y;
            TargetPosition = actionResult.Cell.Position;


            yield return StartCoroutine(MoveCoroutine(action));
            
            //MoveCoroutine();
        }


        if (action.IsAction)
        {
            Debug.Log("Actioning!");
            yield return StartCoroutine(ActionCoroutine(actionResult));
            
            //foreach (Creature creature in actionResult.Creatures)
            //{
            //    BattleEvents.TakeDamage(creature, actionResult.Damage);
            //}

        }

        action.DoAction();

        yield return null;

        
    }


    public virtual void DoMove(Vector3 position, int x, int y)
    {
        TargetX = x;
        TargetY = y;

        TargetPosition = position;

        

        State = CharacterStatesEnum.MOVING;

        //StartCoroutine(MoveCoroutine());
        



    }


    private IEnumerator MoveCoroutine(NewBattleAction action)
    {

        Vector3 startPosition = Transform.position;

        float dist = Vector3.Distance(Transform.position, TargetPosition);

        TotalMoveTime = dist / MoveSpeed;
        CurrentMoveTime = 0f;

        State = CharacterStatesEnum.MOVING;
        Debug.Log("Started moving!");
        _animator.SetBool("IsMoving", true);

        while (CurrentMoveTime < TotalMoveTime)
        {
            if (action.IsJumpingMove)
            {
                Transform.position = Vector3.Slerp(startPosition, TargetPosition, CurrentMoveTime / TotalMoveTime);
            }
            else
            {
                Transform.position = Vector3.Lerp(startPosition, TargetPosition, CurrentMoveTime / TotalMoveTime);
            }
            CurrentMoveTime += Time.deltaTime;
            yield return null;
        }
        _animator.SetBool("IsMoving", false);

        // change facing?
        int diffX = TargetX - CellX;
        int diffY = TargetY - CellY;
        Debug.Log("DiffX = " + diffX + " DiffY = " + diffY);
        if (Math.Abs(diffX) >= Math.Abs(diffY))
        {
            if (diffX > 0)
            {
                _orientation = Quaternion.Euler(0, 90, 0);
            }
            else
            {
                _orientation = Quaternion.Euler(0, -90, 0);
            }

        }
        else
        {
            if (diffY > 0)
            {
                _orientation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                _orientation = Quaternion.Euler(0, 180, 0);
            }
        }


        Transform.SetPositionAndRotation(TargetPosition, _orientation);
        

        Debug.Log("Finished moving!");
        yield return new WaitForSeconds(1f);
        State = CharacterStatesEnum.IDLE;

        UpdatePosition(TargetX, TargetY, TargetPosition, CurrentFacing);
    }


    private IEnumerator ActionCoroutine(ActionResult actionResult)
    {
        Debug.Log("Started action!");
        //yield return new WaitForSeconds(4f);

        // do character action animation...
        var action = actionResult.Action;
        if (!string.IsNullOrEmpty(action.ActionAnimationTrigger))
        {
            _animator.SetTrigger(action.ActionAnimationTrigger);
        }


        foreach (Creature creature in actionResult.Creatures)
        {
            BattleEvents.TakeDamage(creature, actionResult.Action.Damage);
        }

        // set new facing
        int diffX = actionResult.X - CellX;
        int diffY = actionResult.Y - CellY;
        Debug.Log("DiffX = " + diffX + " DiffY = " + diffY);
        if (Math.Abs(diffX) >= Math.Abs(diffY))
        {
            if (diffX > 0)
            {
                _orientation = Quaternion.Euler(0, 90, 0);
            }
            else
            {
                _orientation = Quaternion.Euler(0, -90, 0);
            }

        }
        else
        {
            if (diffY > 0)
            {
                _orientation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                _orientation = Quaternion.Euler(0, 180, 0);
            }
        }


        Debug.Log(_animator.GetCurrentAnimatorStateInfo(0).length + " _ " + _animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length + _animator.GetCurrentAnimatorStateInfo(0).normalizedTime);

        Transform.rotation = _orientation;

        Debug.Log("finished action!");
        

    }

    public virtual int GetAttackDamage()
    {
        return 0;
    }


    private void PassTurn()
    {
        ActionsRemaining = 0;
        BattleEvents.TurnOver();
    }





}

public enum CharacterStatesEnum
{
    IDLE,
    MOVING,
    ATTACKING
}

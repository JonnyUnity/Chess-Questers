using JFlex.ChessQuesters.Core.ScriptableObjects;
using JFlex.ChessQuesters.Encounters.Battle;
using JFlex.ChessQuesters.Encounters.Battle.Grid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    protected Vector3 TargetPosition;
    public GridCell TargetCell;

    protected int TargetX;
    protected int TargetY;
    protected float MoveSpeed = 5f;
    protected float TotalMoveTime;
    private float CurrentMoveTime;

    private Animator _animator;


    public Vector3 Position;
    public int X;
    public int Y;
    public int CurrentFacing;

    private Camera _cam;
    //private ActionClass _selectedAction;

    public Transform Transform { get; protected set; }
    protected Quaternion _orientation;

    [SerializeField] protected GameObject _creatureInfo;
    [SerializeField] protected Slider _healthSlider;
    [SerializeField] protected TMPro.TextMeshProUGUI _nameText;

    [SerializeField] protected GameObject _materialObject;

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
        BattleEvents.OnStartCombat += ResetActions;
        BattleEvents.OnPassTurn += PassTurn;
    }



    protected virtual void OnDisable()
    {
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


    //public bool TakeDamageNew(int healthChange)
    //{
    //    //Debug.Log("TakeDamageNew Start!");
    //    StartCoroutine(CreatureSuffersDamage(healthChange));

    //    //Debug.Log("TakeDamageNew Finish!");
    //    return true;
    //}

    //private void TakeDamage(Creature creature, int healthChange)
    //{
    //    if (creature != this)
    //        return;

    //    StartCoroutine(CreatureSuffersDamage(healthChange));

    //    //Health -= healthChange; // assume no healing actions for now...
    //    ////_healthSlider.value = Health;

    //    //if (Health <= 0)
    //    //{
    //    //    Health = 0;
    //    //    // audio, fx to be handled by those subscribed to the event.

    //    //    // death animation!
    //    //    StartCoroutine(CreatureDied());

            
    //    //}
    //}


    public IEnumerator CreatureSuffersDamage(int healthChange)
    {
        Debug.Log("CreatureSuffersDamage Start!");
        State = CharacterStatesEnum.BEING_ATTACKED;

        // apply any modifiers to incomning damage, e.g. resistances, vulnerabilities

        Health -= healthChange; // assume no healing actions for now...
        BattleEvents.TakeDamage(this, healthChange);

        if (Health <= 0)
        {
            Health = 0;
            _animator.SetTrigger("Dies");
            // audio, fx to be handled by those subscribed to the event.

            // death animation!
            //Debug.Log("Creature died start!");
            ////yield return StartCoroutine(CreatureDied());
            
            //Debug.Log("Creature died finish!");

        }
        else
        {
            _animator.SetTrigger("Hurt");
            Debug.Log("Damage resolved for " + Name);
            //BattleEvents.TakenDamageResolved(this);
        }


        yield return new WaitUntil(() => State != CharacterStatesEnum.BEING_ATTACKED);
        BattleEvents.TakenDamageResolved(this);

        // raise event that damage has been resolved.
        Debug.Log("CreatureSuffersDamage Finish!");


    }


    private void DamageResolved()
    {
        State = CharacterStatesEnum.IDLE;
    }


    private void CreatureDied()
    {
        //yield return new WaitForSeconds(2f);

        Faction.Friendlies.Remove(this);
        State = CharacterStatesEnum.IDLE;

        BattleEvents.CharacterDied(this);

        LeanTween.alpha(_materialObject, 0, 1f);

        Destroy(gameObject, 4f);

    }


    public void UpdatePosition(int cellX, int cellY, Vector3 position, int currentFacing)
    {
        X = cellX;
        Y = cellY;
        Position = position;
        CurrentFacing = currentFacing;
    }

    //public void UpdatePositionNew(Creature creature)
    //{
    //    if (creature != this)
    //        return;


    //    X = TargetX;
    //    Y = TargetY;
    //    Position = TargetPosition;

    //}


    //public virtual void DoAction(ActionResult actionResult)
    //{
    //    StartCoroutine(DoTurnCoroutine(actionResult));
    //}


    protected IEnumerator DoTurnCoroutine(ActionResult actionResult)
    {

        var action = actionResult.Action;
        BattleEvents.ActionStarted(action);
        Debug.Log("Action started");

        if (action.IsMove)
        {
            TargetX = actionResult.X;
            TargetY = actionResult.Y;
            TargetPosition = actionResult.Cell.Position;

            yield return StartCoroutine(MoveCoroutine(action));            
        }


        if (action.IsAction)
        {
            Debug.Log("Actioning!");
            yield return StartCoroutine(ActionCoroutine(actionResult));
        }

        action.DoAction();

        Debug.Log("Action finished");
        BattleEvents.ActionFinished();

        //yield return null;

    }


    public virtual void DoMove(Vector3 position, int x, int y)
    {
        TargetX = x;
        TargetY = y;

        TargetPosition = position;

        

        State = CharacterStatesEnum.MOVING;


    }


    private IEnumerator MoveCoroutine(NewBattleAction action)
    {

        Vector3 startPosition = Transform.position;

        float dist = Vector3.Distance(Transform.position, TargetPosition);

        TotalMoveTime = dist / MoveSpeed;
        CurrentMoveTime = 0f;

        State = CharacterStatesEnum.MOVING;
        Debug.Log("Started moving!");
        Transform.LookAt(TargetPosition);

        if (!string.IsNullOrEmpty(action.MoveAnimationTrigger))
        {
            if (action.IsJumpingMove)
            {
                _animator.SetTrigger(action.MoveAnimationTrigger);
            }
            else
            {
                _animator.SetBool(action.MoveAnimationTrigger, true);
            }            
        }

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
        if (!string.IsNullOrEmpty(action.MoveAnimationTrigger))
        {
            if (!action.IsJumpingMove)
            {
                _animator.SetBool(action.MoveAnimationTrigger, false);
            }                
        }

        Reorientate(TargetX, TargetY);

        Transform.SetPositionAndRotation(TargetPosition, _orientation);
        
        Debug.Log("Finished moving!");
        yield return new WaitForSeconds(1f);
        
        State = CharacterStatesEnum.IDLE;

        UpdatePosition(TargetX, TargetY, TargetPosition, CurrentFacing);
        BattleEvents.CreatureMoved(this);
    }


    private IEnumerator ActionCoroutine(ActionResult actionResult)
    {
        Debug.Log("Started action!");



        // do character action animation...
        var action = actionResult.Action;
        int damage = action.RollDamage;

        if (!string.IsNullOrEmpty(action.ActionAnimationTrigger))
        {
            _animator.SetTrigger(action.ActionAnimationTrigger);
        }


        foreach (Creature creature in actionResult.Creatures)
        {
            //BattleEvents.TakeDamage(creature, actionResult.Action.Damage);
            //creature.TakeDamageNew(action.Damage);
            StartCoroutine(creature.CreatureSuffersDamage(damage));

            // wait for damage to resolve before continuing.

        }
        //yield return new WaitWhile(() => actionResult.Creatures.Select(s => s.State == CharacterStatesEnum.BEING_ATTACKED).Any());

        yield return new WaitWhile(() => BattleSystem.Instance.State == BattleStatesEnum.RESOLVING_PLAYER_ACTION);

        Reorientate(actionResult.X, actionResult.Y);

        //yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length + 1);



        Transform.rotation = _orientation;

        Debug.Log("finished action!");
        

    }

    private void Reorientate(int targetX, int targetY)
    {
        int diffX = targetX - X;
        int diffY = targetY - Y;
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
    ATTACKING,
    BEING_ATTACKED,
    PARTY_SELECT
}
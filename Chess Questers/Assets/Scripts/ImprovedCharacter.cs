using System;
using System.Linq;
using UnityEngine;


public class ImprovedCharacter : MonoBehaviour
{
    public int ID;
    public int EnemyID;

    public string Name { get; private set; }
    public bool IsFriendly { get; private set; }

    public int Health { get; private set; }
    public int MaxHealth { get; private set; }

    // Move Class properties
    public MoveClass MoveClass;
    public string MoveClassText { get; private set; }

    public Color MoveClassColor => MoveClass.DebugColor;

    // Action Class properties
    public ActionClass[] Actions { get; private set; }
    public string ActionsText => string.Join(",", Actions.Select(s => s.Name).ToArray());


    public int CharacterModel { get; private set; }


    public GridCell OccupiedCell;
    private Vector3 TargetPosition;
    public GridCell TargetCell;

    private int TargetX;
    private int TargetY;
    private float MoveSpeed = 10f;

    public Vector3 Position;
    public int CellX;
    public int CellY;
    public int CurrentFacing;


    private ActionClass _selectedAction;

    public Transform Transform { get; private set; }
    private Quaternion _orientation;

    public CharacterStatesEnum State;


    private Camera _cam;
    [SerializeField] private GameObject _characterInfo;
    [SerializeField] private TMPro.TextMeshProUGUI _nameText;

    private void Awake()
    {
        _cam = Camera.main;
        Transform = transform;
        _orientation = transform.rotation;

        BattleEvents.OnPlayerActionSelected += SetSelectedAction;
        BattleEvents.OnTakeDamage += TakeDamage;
       
    }

    private void OnDisable()
    {
        BattleEvents.OnPlayerActionSelected -= SetSelectedAction;
        BattleEvents.OnTakeDamage -= TakeDamage;
    }


    public void Init(string name, int characterModel, MoveClass moveClass, ActionClass[] actions, int maxHealth)
    {
        Name = name;
        _nameText.text = Name;
        IsFriendly = true;

        CharacterModel = characterModel;
        MoveClass = moveClass;
        MoveClassText = moveClass.name;

        Actions = actions;

        Health = maxHealth;
        MaxHealth = maxHealth;
    }

    public void InitFromCharacterData(CharacterJsonData data)
    {
        ID = data.ID;
        Name = data.Name;
        _nameText.text = Name;
        IsFriendly = data.IsFriendly;
        CharacterModel = data.CharacterModel;
        MoveClass = GameManager.Instance.GetMoveClassWithID(data.MoveClassID);
        MoveClassText = MoveClass.name;

        Actions = GameManager.Instance.GetActionsWithIDs(data.Actions);
        Health = data.Health;
        MaxHealth = data.MaxHealth;

        CellX = data.CellX;
        CellY = data.CellY;
        Position = data.Position;
        CurrentFacing = data.CurrentFacing;
    }

    public void InitFromEnemyData(EnemyJsonData data)
    {
        ID = data.ID;
        EnemyID = data.EnemyID;
        Name = data.Name;
        _nameText.text = Name;
        IsFriendly = data.IsFriendly;
        CharacterModel = data.CharacterModel;
        MoveClass = GameManager.Instance.GetMoveClassWithID(data.MoveClassID);
        MoveClassText = MoveClass.name;

        Actions = GameManager.Instance.GetActionsWithIDs(data.Actions);
        Health = data.Health;
        MaxHealth = data.MaxHealth;

        CellX = data.CellX;
        CellY = data.CellY;
        Position = data.Position;
        CurrentFacing = data.CurrentFacing;
    }


    //public void InitFromEnemyData(Enemy data)
    //{
    //    ID = data.ID;
    //    Name = data.Name;
    //    IsFriendly = false;
    //    CharacterModel = data.CharacterModel;
    //    MoveClass = data.MoveClass;
    //    MoveClassText = MoveClass.name;

    //    Actions = data.Actions;
    //    Health = data.Health;
    //    MaxHealth = data.Health;
    //}
    


    public void SetSelectedAction(int characterID, ActionClass action, int x, int y)
    {
        if (ID != characterID)
            return;

        _selectedAction = Actions.Where(w => w.ID == action.ID).Single();
    }

    public int GetAttackDamage()
    {
        // action base damage
        // (+  any modifiers?)
        return _selectedAction.Damage;
    }


    public void TakeDamage(int characterID, int healthChange)
    {
        if (ID != characterID)
            return;

        Health -= healthChange; // assume no healing actions for now...

        if (Health <= 0)
        {
            // audio, fx to be handled by those subscribed to the event.
            BattleEvents.CharacterDied(ID);
            Destroy(gameObject);
        }

    }




    public void UpdateHealth(int currentHealth)
    {
        Health = currentHealth;
    }


    public void UpdatePosition(int cellX, int cellY, Vector3 position, int currentFacing)
    {
        CellX = cellX;
        CellY = cellY;
        Position = position;
        CurrentFacing = currentFacing;
    }

    public void Update()
    {

        _characterInfo.transform.LookAt(_cam.transform);
        _characterInfo.transform.rotation = _cam.transform.rotation;

        if (State != CharacterStatesEnum.MOVING) return;

        Vector3 direction = (TargetPosition - Transform.position).normalized;
        Transform.position += MoveSpeed * Time.deltaTime * direction;

        if (Vector3.Distance(transform.position, TargetPosition) < 0.1f)
        {
            Transform.SetPositionAndRotation(TargetPosition, _orientation);

            //SetPosition(TargetX, TargetY);
            State = CharacterStatesEnum.IDLE;
        }

    }


    public void DoMove(Vector3 position, int x, int y)
    {
        TargetX = x;
        TargetY = y;

        TargetPosition = position;
        State = CharacterStatesEnum.MOVING;
    }


    public void LookAtCell(Vector3 pos)
    {
        Quaternion lookRotation = Quaternion.LookRotation((pos - transform.position).normalized) * Quaternion.Euler(0, -90, 0);
        Transform.rotation = lookRotation;
    }


}

public enum CharacterStatesEnum
{
    IDLE,
    MOVING,
    ATTACKING
}
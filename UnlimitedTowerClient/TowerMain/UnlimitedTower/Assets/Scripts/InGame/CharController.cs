using System.Collections;
using UnityEngine;

public class CharController : MonoBehaviour
{
    
    private Battle_Character_Status _status = null;
    // for lazy setting
    public Battle_Character_Status status {
        set {

            if(_status == null && healthGaze != null)
            {
                healthGaze.SetMaxHealth(value.maxHp);
                UpdateHealthUI = () =>
                {
                    healthGaze.SetHealth(value.nowHp);
                };
            }
            _status = value;
        }
        get {
            return _status;
        }
    }

    private Vector3 charMovement;
    private Vector3 charDirection;
    private Vector3 charFirstDirection;
    private  Vector3 charFirsrPos;

    private Transform charTransform;
    private Animator charAnimator;

    private GameObject targetObject;
    private CharacterAction charAction;


    public STATE_TYPE stateType;
    public FORMATION_TYPE formationType;
    public SIZE_TYPE charSize;

    public int battleDicIndex;

    public GameObject QuadSelectionObject;


    #region UIs

    static private readonly string _HEALTHGAZE_PREFAB_PATH = "Prefabs/UIs/HealthGaze";
    static private GameObject prefHealthGaze = null;

    private InGameCharHealthGaze healthGaze = null;
    private System.Action UpdateHealthUI = null;
    #endregion

    private void Awake()
    {
        //create health gaze on individual char / and init
        prefHealthGaze = prefHealthGaze ?? Resources.Load(_HEALTHGAZE_PREFAB_PATH) as GameObject;
        var goHealthGaze = Instantiate(prefHealthGaze, transform) as GameObject;

        healthGaze = goHealthGaze != null ? goHealthGaze.GetComponent<InGameCharHealthGaze>() : null;

        //value setup from component
        charTransform = GetComponent<Transform>();
        charAnimator = GetComponent<Animator>();
        charFirstDirection = charTransform.forward;
        charFirsrPos = charTransform.position;

        charAnimator.speed = DEFINE.ANIMATION_SPEED;

        stateType = STATE_TYPE.IDLE; 
    }

    private void Update()
    {
        if (UpdateHealthUI != null)
            UpdateHealthUI();
    }

    public void SetFirstPosition()
    {
        charFirsrPos = charTransform.position;
    }

    IEnumerator IDLE()
    {
        do
        {
            yield return null; 
        }
        while (stateType == STATE_TYPE.IDLE);
    }

    IEnumerator RUN()
    {
        do
        {
            yield return null;
            Move(MOVE_TYPE.RUN, charTransform.position, targetObject.transform.position);
        }
        while (stateType == STATE_TYPE.RUN);

        charAnimator.SetTrigger("IsAttacking");
        yield return StartCoroutine(stateType.ToString());
    }

    IEnumerator ATK()
    {
        //do
        //{  
        //    yield return null;
        //}
        //while (!charAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack"));

        // 공격 애니메이션 재생.
        do
        {
            yield return null;                         
        }
        while (charAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack")&&
        charAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime<=1.0f);
    

        SetDirection(charTransform.position, charFirsrPos);
        
        charAnimator.SetBool("IsRunning", true);
        stateType = STATE_TYPE.RETURN;
        yield return StartCoroutine(stateType.ToString());
    }

    IEnumerator RETURN()
    {
        do
        {
            yield return null;
            Move(MOVE_TYPE.RETURN, charTransform.position, charFirsrPos);        
        }
        while (stateType == STATE_TYPE.RETURN);
        charAnimator.SetBool("IsRunning", false);
  

        // 내 턴을 마치고 다른 사람한테 넘긴다.
        BattleManager.Inst.ReadyNextTurn();
        if (BattleManager.Inst.CheckAttackCycle())
            yield break;

       yield return StartCoroutine(stateType.ToString());
       
    }
    // 공격 받은 상태
    IEnumerator HIT()
    {
        do
        {
            yield return null;
        } while (charAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit") &&
        charAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);

        stateType = STATE_TYPE.IDLE;
        yield return StartCoroutine(stateType.ToString());
    }
    IEnumerator DIE()
    {
        do
        {
            yield return null;
        } while (charAnimator.GetCurrentAnimatorStateInfo(0).IsName("Death") &&
        charAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);

        //stateType = STATE_TYPE.NONE;
        yield break;
    }


    private bool Move(MOVE_TYPE moveType, Vector3 myPos, Vector3 targetPos)
    {
        float distance = Vector3.Distance(myPos, targetPos);

        if (moveType == MOVE_TYPE.RUN)
        {
            if (distance < DEFINE.STOP_DISTANCE)
            {
               stateType = STATE_TYPE.ATK;     
                return true;
            }
        }
        else
        {
            if (distance <= DEFINE.MIN_DISTANCE) //거리 보정
            {
                charTransform.position =charFirsrPos;
               stateType = STATE_TYPE.IDLE;


                Rotate(charFirstDirection, gameObject);
                return true;
            }
        }

        charMovement = charDirection;
        charMovement.y = 0;
        charMovement = charMovement.normalized * DEFINE.MOVING_SPEED * DEFINE.ANIMATION_SPEED* Time.deltaTime;
        charTransform.position += charMovement;
        // TODO : rigidbogy 적용했을 때.
        // _rigidbody.MovePosition(charTransform.position + movement);
        return false;
    }



    private void AttackEvent()
    {
      switch(charAction.actionType)
        {
            case ACTION_TYPE.None:
                break;
            case ACTION_TYPE.Attack:
                {
                    Debug.Log(formationType.ToString() + " : 공격 성공");
                    BattleManager.Inst.CheckCharBeHit(charAction.formationType, charAction.myIndex, charAction.targetIndex);                
                    break;
                }
            case ACTION_TYPE.Defence:
                break;
            case ACTION_TYPE.Skill_1:
                break;
            case ACTION_TYPE.Skill_2:
                break;
            case ACTION_TYPE.Skill_3:
                break;
        }

    }
 
    void SetDirection(Vector3 myPos, Vector3 targetPos)
    {
        myPos.y = targetPos.y = 0;
        charDirection = (targetPos - myPos).normalized;
        Rotate(charDirection, gameObject);
    }
    public void Rotate(Vector3 dir, GameObject myObject)
    {
        Quaternion characterTargetRotation = Quaternion.LookRotation(dir);
      
        charTransform.localRotation = characterTargetRotation;


        //TODO : rigidbogy 적용했을 때.
        //_rigidbody.MoveRotation(characterTargetRotation);
    }


    public void PlayHitAnimation()
    {
        stateType = STATE_TYPE.HIT;
        charAnimator.SetTrigger("IsHit");
        StartCoroutine(stateType.ToString());
    }

    public void PlayDeadAnimation()
    {
        stateType = STATE_TYPE.DIE;
        charAnimator.SetTrigger("IsDying");
        StartCoroutine(stateType.ToString());
    }


    public void StartMyTurn(STATE_TYPE charState, GameObject _targetObject, CharacterAction _charAction)
    {
        stateType = charState;
        targetObject = _targetObject;
        charAction = _charAction;
        SetDirection(charTransform.position, _targetObject.transform.position);

        charAnimator.SetBool("IsRunning", true);
        StartCoroutine(stateType.ToString());
    }




    //public void PlayHitAnimation(CHAR_TYPE charType, int targetIndex)
    //{
    //    if (charType != CHAR_TYPE.PLAYER)
    //    {
    //        BattleManager.Inst.playerObject[targetIndex].GetComponent<CharController>().SetHit();
    //    }
    //    else
    //    {
    //        BattleManager.Inst.enemyObject[targetIndex].GetComponent<CharController>().SetHit();
    //    }
    //}


    // 배틀 매니저 함수를 호출해서 체력이 깍이도록 설정.
    // 배틀 매니저+컨트롤러 합치기.
    // 마우스 클릭은 업데이트에서만 하도록.
    // 플래그 설정 및 배틀 상태일 때만 클릭 되도록.
    // 코루틴 두지말자.

    //  TODO : Sample Code if Delete
    void SelectAction()
    {
        switch (BattleManager.Inst.turnActionList[BattleManager.Inst.AttackOrder].actionType)
        {
            case ACTION_TYPE.None:
                break;
            case ACTION_TYPE.Attack:
                break;
         
        }
    }
}

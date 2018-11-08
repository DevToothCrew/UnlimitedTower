using System.Collections;
using UnityEngine;

public class CharController : MonoBehaviour
{
    
    private Vector3 _movement;
    private Vector3 _direction;
    private Vector3 _firstDir;
    private Vector3 _firstPos;

    private Transform _transform;
    private Animator _animator;

   private GameObject _targetObject;
   private CharacterAction _charAction;


    public STATE_TYPE stateType;
    public CHAR_TYPE charType;
    public int battleDicIndex;

    public GameObject circleObject;

   
    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _animator = GetComponent<Animator>();
        _firstDir = _transform.forward;
        _firstPos = _transform.position;
        
        stateType = STATE_TYPE.IDLE; 
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
            Move(MOVE_TYPE.RUN, _transform.position, _targetObject.transform.position);
        }
        while (stateType == STATE_TYPE.RUN);

        _animator.SetTrigger("IsAttacking");
        yield return StartCoroutine(stateType.ToString());
    }

    IEnumerator ATK()
    {
        //do
        //{  
        //    yield return null;
        //}
        //while (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"));

        // 공격 애니메이션 재생.
        do
        {
            yield return null;                         
        }
        while (_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")&&
        _animator.GetCurrentAnimatorStateInfo(0).normalizedTime<=1.0f);
    

        SetDirection(_transform.position, _firstPos);
        
        _animator.SetBool("IsRunning", true);
        stateType = STATE_TYPE.RETURN;
        yield return StartCoroutine(stateType.ToString());
    }

    IEnumerator RETURN()
    {
        do
        {
            yield return null;
            Move(MOVE_TYPE.RETURN, _transform.position, _firstPos);        
        }
        while (stateType == STATE_TYPE.RETURN);
        _animator.SetBool("IsRunning", false);
  

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
        } while (_animator.GetCurrentAnimatorStateInfo(0).IsName("Hit") &&
        _animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);

        stateType = STATE_TYPE.IDLE;
        yield return StartCoroutine(stateType.ToString());
    }
    IEnumerator DIE()
    {
        do
        {
            yield return null;
        } while (_animator.GetCurrentAnimatorStateInfo(0).IsName("Death") &&
        _animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);

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
                _transform.position =_firstPos;
               stateType = STATE_TYPE.IDLE;


                Rotate(_firstDir, gameObject);
                return true;
            }
        }

        _movement = _direction;
        _movement.y = 0;
        _movement = _movement.normalized * DEFINE.MOVING_SPEED * Time.deltaTime;
        _transform.position += _movement;
        // TODO : rigidbogy 적용했을 때.
        // _rigidbody.MovePosition(_transform.position + movement);
        return false;
    }



    private void AttackEvent()
    {
      switch(_charAction.actionType)
        {
            case ACTION_TYPE.None:
                break;
            case ACTION_TYPE.Attack:
                {
                    Debug.Log(charType.ToString() + " : 공격 성공");
                    BattleManager.Inst.DecreaseHp(_charAction.charType, _charAction.myIndex, _charAction.targetIndex);                
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
        _direction = (targetPos - myPos).normalized;
        Rotate(_direction, gameObject);
    }
    public void Rotate(Vector3 dir, GameObject myObject)
    {
        Quaternion characterTargetRotation = Quaternion.LookRotation(dir);
      
        _transform.localRotation = characterTargetRotation;


        //TODO : rigidbogy 적용했을 때.
        //_rigidbody.MoveRotation(characterTargetRotation);
    }


    public void PlayHitAnimation()
    {
        stateType = STATE_TYPE.HIT;
        _animator.SetTrigger("IsHit");
        StartCoroutine(stateType.ToString());
    }

    public void PlayDeadAnimation()
    {
        stateType = STATE_TYPE.DIE;
        _animator.SetTrigger("IsDying");
        StartCoroutine(stateType.ToString());
    }


    public void StartMyTurn(STATE_TYPE charState, GameObject targetObject, CharacterAction charAction)
    {
        stateType = charState;
        _targetObject = targetObject;
        _charAction = charAction;
        SetDirection(_transform.position, _targetObject.transform.position);

        _animator.SetBool("IsRunning", true);
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
        switch (BattleManager.Inst.turnActionList[BattleManager.Inst.attackOrder].actionType)
        {
            case ACTION_TYPE.None:
                break;
            case ACTION_TYPE.Attack:
                break;
         
        }
    }
}

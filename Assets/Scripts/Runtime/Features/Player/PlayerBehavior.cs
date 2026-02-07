using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TopdownMotor2D))]
public class PlayerBehavior : MonoBehaviour
{   
    [SerializeField] TopdownMotor2D _topdownMotor2D; 
    [SerializeField] CameraFollow2D _cameraFollow2D;
    [SerializeField] AnimationControl _animationControl;
    [SerializeField] MouseCursorFollow _mouseCursorFollow;
    [SerializeField] Hitbox2D _hitbox2D;
    [SerializeField] SpriteFlash2D _spriteFlash2D;

    public AnimationControl animationControl => _animationControl;
    public MouseCursorFollow mouseCursorFollow => _mouseCursorFollow;
    public Hitbox2D hitbox2D => _hitbox2D;
    public SpriteFlash2D spriteFlash2D => _spriteFlash2D;


    [Header("Attack (plug & play)")]
    [SerializeField] MonoBehaviour _attackBehaviour; // drag component implement IAttackModule
    IAttackModule _attack;
    public IAttackModule attack => _attack;

    bool _doneInit;  
    GameState _gameState;
    public GameState gameState => _gameState;

    public IEnumerator Init(GameState gameState)
    {           
        _gameState = gameState;
        _doneInit = false;
         // cache attack module
        _attack = _attackBehaviour as IAttackModule;
        if (_attack == null)
        {
            Debug.LogError($"AttackBehaviour must implement IAttackModule on {name}");
        }
        else
        {
            _attack.Init(this);
        }

        yield return _topdownMotor2D.Init(this);
        yield return _hitbox2D.Init(this);
        yield return _spriteFlash2D.Init();
        _doneInit = true;

    }

    void Update()
    {   
        if(!_doneInit)
            return;

        _attack?.Tick();

        if (Input.GetMouseButtonDown(0))
            _attack?.TryAttack();

        _topdownMotor2D.UpdateMove();
    }

    void FixedUpdate()
    {
          if(!_doneInit)
            return;

        _topdownMotor2D.FixedUpdateMove();
    }

    #region Set
    /// <summary>
    /// đổi state/weapon runtime: gọi hàm này
    /// </summary>
    /// <param name="newAttackBehaviour"></param>
    public void SetAttackModule(MonoBehaviour newAttackBehaviour)
    {
        _attackBehaviour = newAttackBehaviour;
        _attack = _attackBehaviour as IAttackModule;

        if (_attack == null)
        {
            Debug.LogError("New attack behaviour does not implement IAttackModule");
            return;
        }
        _attack.Init(this);
    }
    #endregion

    #region Get
    public void GetAttackmodule()
    {
        
    }
    #endregion
}

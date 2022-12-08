using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BikeHandling : MonoBehaviour
{
    [Header("Bike Info")]
    [SerializeField] float _balance = 0;
    [SerializeField] float _balanceSmootheness = 0.125f;
    float _smoothedBalance = 0;

    [Header("Player Values")]
    [SerializeField] float _playerInfluence = 5;
    [SerializeField] float _playerPushForce = 1;
    [SerializeField] KeyCode[] leftPedal,rightPedal,leftPedalAZERTY;
    [SerializeField] int leftPedalIndex,rightPedalIndex;

    [Header("Tip Factor")]
    [SerializeField] float _fallingForce = 5;
    [SerializeField] float _fallingDir = 1;
    [SerializeField] float _angleAffectPorcentage = 0.10f;
    [SerializeField] bool _pullingLeft = false;
    [SerializeField] bool _pullingRight = false;

    BikeMovement bMove;

    void Start()
    {
        bMove = GetComponent<BikeMovement>();
        _balance = 0; //Random.Range(1,_fallingForce) * (Random.Range(-1,3) <= 0 ? -1 : 1);    
        rightSide = 2;
    }

    void Update()
    {
        //if(!GameManager.instance.RunStarted()) return;

        PlayerInput();
        bMove.UpdateBalance(_balance);
    }

    void FixedUpdate()
    {
        if(!GameManager.instance.RunStarted()) return;

        if(_pullingLeft){
            AddBalance(_playerPushForce * -1 * Time.deltaTime);
            bMove.GuidaoAddRot(-1 * Time.deltaTime);
        }

        if(_pullingRight){
            AddBalance(_playerPushForce * 1 * Time.deltaTime);
            bMove.GuidaoAddRot(1 * Time.deltaTime);
        }

        FallingForce();

        _smoothedBalance = Mathf.Lerp(_smoothedBalance,_balance,_balanceSmootheness * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0,transform.eulerAngles.y,_smoothedBalance);
    }

    void FallingForce(){
        var fallingForce = Time.deltaTime * _fallingForce * _fallingDir * (Mathf.Abs(_balance * 2) * _angleAffectPorcentage);
        _fallingDir = _balance > 0 ? 1 : -1;
        AddBalance(fallingForce);
    }

    int rightSide = 2;
    void PlayerInput(){
        var isQWERTY = qwerty ? leftPedal : leftPedalAZERTY;
        if(Input.GetKeyDown(isQWERTY[leftPedalIndex]) && (rightSide == 0 || rightSide == 2)){
            AddBalance(_playerInfluence * -1);
            leftPedalIndex += 1;
            
            bMove.MovePedals();
            
            if(leftPedal.Length-1 == leftPedalIndex){
                rightSide = 1;
                leftPedalIndex = 0;
                bMove.PedalBike();
            }
        }
    
        if(Input.GetKeyDown(rightPedal[rightPedalIndex]) && (rightSide == 1 || rightSide == 2)){
            AddBalance(_playerInfluence * 1);
            rightPedalIndex += 1;

            bMove.MovePedals();

            if(rightPedal.Length-1 == rightPedalIndex){
                rightSide = 0;
                rightPedalIndex = 0;
                bMove.PedalBike();
            }
        }

        _pullingLeft = Input.GetKey(isQWERTY[3]);
        _pullingRight = Input.GetKey(rightPedal[3]);

        UIManager.Instance.InputHoldUI(0,_pullingLeft);
        UIManager.Instance.InputHoldUI(1,_pullingRight);
        UIManager.Instance.InputHoldUI(2,!(rightSide == 2 || rightSide == 0));
        UIManager.Instance.InputHoldUI(3,!(rightSide == 2 || rightSide == 1));
    }

    void AddBalance(float amount){
        _balance += amount;
        _balance = Mathf.Clamp(_balance,-90,90);
        if(_balance == -90 || _balance == 90){
           GameManager.instance.EndRun();
        }
    }

    public void ResetValues(){
        rightSide = 2;
        leftPedalIndex = 0;
        rightPedalIndex = 0;
        _balance = 0;
        _smoothedBalance = 0;
        transform.position = new Vector3(0,0.5f,-111);
        transform.rotation = Quaternion.Euler(0,-180,0);
    }

    [Header("UI BS")]
    [SerializeField] Color selectedColor,unselectedColor;
    [SerializeField] Image qwertyBtn,azertyBtn;
    [SerializeField] bool qwerty = true;
    public void SetInputLayout(int index){
        qwerty = index == 0;
        qwertyBtn.color = qwerty ? selectedColor : unselectedColor;
        azertyBtn.color = !qwerty ? selectedColor : unselectedColor;
        UIManager.Instance.UpdateInputText(qwerty);
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.InputSystem; // Thư viện này chứa các class để xử lý input mới của Unity

public class PlayerInput : MonoBehaviour
{
    public FrameInput FrameInput {get; private set;}
    private PlayerInputActions _playerInputActions;
    private InputAction _move;
    private InputAction _jump;

    private void Awake(){
        _playerInputActions = new PlayerInputActions();
        _move = _playerInputActions.Player.Move;
        _jump = _playerInputActions.Player.Jump;
    }

    private void OnEnable(){ // Đây là phương thức được gọi khi script được kích hoạt
       _playerInputActions.Enable(); //Kích hoạt Input System
    }

    private void OnDisable(){ // Đây là phương thức được gọi khi script bị vô hiệu hóa
        _playerInputActions.Disable(); //Vô hiệu hóa Input System
    }

    private void Update(){
       FrameInput = GatherInput(); // Cập nhật thường xuyên giá trị của FrameInput
    }
 
    //Hàm này sẽ trả về những input của người chơi
    private FrameInput GatherInput(){
        return new FrameInput{
            Move = _move.ReadValue<Vector2>(), // Lấy giá trị của InputAction _move
            Jump = _jump.WasPressedThisFrame(), // Lấy giá trị của InputAction _jump
        };
    }
}

public struct FrameInput{
    public Vector2 Move; // Vector2 này sẽ chứa giá trị của trục x và y của input move
    public bool Jump; // Biến này sẽ chứa giá trị của input jump
}
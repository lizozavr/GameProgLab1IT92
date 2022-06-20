using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : BaseState
{
    private PlayerController _playerController;
    public JumpState(PlayerController playerController)
    {
        _playerController = playerController;
    }
    public override void Enter()
    {
        _playerController.Jump();
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }
}

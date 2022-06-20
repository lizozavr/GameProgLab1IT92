using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftRoadState : BaseState
{
    private PlayerController _playerController;
    public LeftRoadState(PlayerController playerController)
    {
        _playerController = playerController;
    }
    public override void Enter()
    {
        _playerController.MoveHorizontal(-(_playerController.laneChangeSpeed));
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }
}

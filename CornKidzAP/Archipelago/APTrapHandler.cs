using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CornKidzAP.Archipelago;

public class APTrapHandler : MonoBehaviour
{
    private readonly Queue<Traps> _traps = new();
    private PlayerCtrl _player;
    private float _cooldownCounter;

    /// <summary>
    /// trap cooldown in seconds
    /// </summary>
    [SerializeField] private int cooldown = 1;

    private void Start()
    {
        _player = GetPlayer();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private PlayerCtrl GetPlayer()
    {
        return GameObject.FindWithTag("Player").GetComponent<PlayerCtrl>();
    }

    public void AddTrap(Traps trap)
    {
        _traps.Enqueue(trap);
    }

    private bool CanRunTrap(Traps trap)
    {
        if(GameCtrl.instance.currentWorld < 0)
            return false;
        //ToDo: we might need some more state checking here
        return trap switch
        {
            Traps.SlapTrap => !_player.bNoHurt && _player.plState != PlayerCtrl.State.dig && NotInEnterOrExit(_player.plState) && _player.hurtTimer == 0 && GameCtrl.instance.HP > 0,
            Traps.SlipTrap => _player.bOnFloor && NotInEnterOrExit(_player.plState), 
            Traps.SkidTrap =>  _player.bOnFloor && NotInEnterOrExit(_player.plState) && _player.ctrlDir.sqrMagnitude > 0.25 /*50% in any direction*/,
            _ => throw new ArgumentOutOfRangeException(nameof(trap), trap, null)
        };
    }

    private static bool NotInEnterOrExit(PlayerCtrl.State state)
    {
        return state is not (PlayerCtrl.State.exit or PlayerCtrl.State.enter or PlayerCtrl.State.mirrorexit or PlayerCtrl.State.mirrorenter);
    }

    private void Update()
    {
        if(GameCtrl.instance.bPause)
            return;
        //advance cooldown
        if (_cooldownCounter > 0)
        {
            _cooldownCounter -= Time.deltaTime;
            return;
        }

        if(_traps.Count <= 0)
            return; // no traps to process
        
        //check player
        if(!_player)
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation 
            // only called if player has been destroyed so it's fine
            _player = GetPlayer();
        if(!_player)
            return; //player destroyed
        
        // check if trap can run
        var trap = _traps.Dequeue();
        if (!CanRunTrap(trap))
        {
            //trap can't run so put at end of queue and try again later
            _traps.Enqueue(trap);
            return;
        }

        //process trap
        switch (trap)
        {
            case Traps.SlapTrap:
                _player.Hurt(_player.transform.position + Random.insideUnitSphere);
                break;
            case Traps.SlipTrap:
                _player.plState = PlayerCtrl.State.slip;
                break;
            case Traps.SkidTrap:
                _player.plState = PlayerCtrl.State.skid;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        _cooldownCounter = cooldown; //reset cooldown
    }
}
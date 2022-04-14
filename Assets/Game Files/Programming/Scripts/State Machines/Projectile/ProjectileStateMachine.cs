using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileStateMachine : MonoBehaviour
{
    public ProjectileObject ProjectileObject => GetComponent<ProjectileObject>();

    public ProjectileState CurrentState;

    bool busyChange;

    public void OnUpdate()
    {
        CurrentState?.OnUpdate(ProjectileObject);
    }

    public void OnFixedUpdate()
    {
        CurrentState?.OnFixedUpdate(ProjectileObject);
        CurrentState?.HandleState(ProjectileObject);
    }

    public void ChangeState(ProjectileState newState)
    {
        if (!busyChange)
            ChangeStateWait(newState);
    }

    void ChangeStateWait(ProjectileState newState)
    {
        busyChange = true;
        CurrentState.OnExit(ProjectileObject);
        CurrentState = newState;
        CurrentState.OnEnter(ProjectileObject);
        busyChange = false;
    }
}
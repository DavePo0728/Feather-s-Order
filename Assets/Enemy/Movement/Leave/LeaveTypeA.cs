using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveTypeA : ILeaveBehaviour
{
    Tweener leaveTweener;
    public void Leave(EnemyMove enemyMove)
    {
        if (enemyMove.gameObject !=null) 
        {
            leaveTweener = enemyMove.transform.DOMove(enemyMove.leavePoint, 1.0f).SetEase(Ease.Linear);
            leaveTweener.Play();
            leaveTweener.OnComplete(() => { enemyMove.DestroyNow(); });
        }
    }
    public bool CheckLeaveStatus()
    {
        if (leaveTweener.IsPlaying())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void StopLeave()
    {
        if (leaveTweener != null&&leaveTweener.IsPlaying())
            leaveTweener.Kill();
    }
    public void ParalyzePause()
    {
        leaveTweener.Pause();
    }
    public void ParalyzeRecover()
    {
        leaveTweener.Play();
    }
}

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
            leaveTweener = enemyMove.transform.DOMove(enemyMove.leavePoint, enemyMove.leaveTime).SetEase(Ease.Linear);
            leaveTweener.Play();
            leaveTweener.OnComplete(() => { enemyMove.DestroyNow(); });
        }
    }
    public bool CheckLeaveStatus()
    {
        if (leaveTweener == null)
        {
            return false;
        }
        else
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

    }
    public void StopLeave()
    {
        if (leaveTweener != null )
        {
            if(leaveTweener.IsPlaying())
                leaveTweener.Kill();
        }
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

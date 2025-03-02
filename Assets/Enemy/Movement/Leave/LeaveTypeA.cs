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
            leaveTweener.OnComplete(() => { enemyMove.DestroyNow(); });
        }
    }
    public void StopLeave()
    {
        if (leaveTweener != null&&leaveTweener.IsPlaying())
            leaveTweener.Kill();
    }
}

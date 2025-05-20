using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTypeC : IMoveBehaviour
{
    public Tweener onMoveC;
    float pointWaitTime;
    float pathCount;
    public void Move(EnemyMove enemyMove)
    {
        // 先取出等待時間，方便後面使用
        float waitTime = enemyMove.pointWaitTime;
        Vector3[] path = enemyMove.moveC_PathList;
        float moveDuration = enemyMove.moveTime;

        // 如果沒有路徑或物件不存在就直接離場
        if (path == null || path.Length == 0 || enemyMove.gameObject == null)
        {
            enemyMove.CallLeave();
            return;
        }

        // 建立一個空的 Sequence
        Sequence seq = DOTween.Sequence();

        // 依序把「移動 → 等待」加入到 Sequence 裡
        foreach (var targetPos in path)
        {
            // 1) Append 一段移動 tween
            seq.Append(enemyMove.transform
                .DOMove(targetPos, moveDuration)
                .SetEase(Ease.Linear)
            );
            // 2) Append Interval 等待
            seq.AppendInterval(waitTime);
        }

        // 全部走完後呼叫離場
        seq.AppendCallback(() => enemyMove.CallLeave());

        // 啟動 Sequence
        seq.Play();
    }
    public bool CheckMoveStatus()
    {
        if (onMoveC == null)
        {
            return false;
        }
        else
        {
            return onMoveC.IsPlaying();
        }
    }
    public void StopMove()
    {
        if (onMoveC != null && onMoveC.IsPlaying())
            onMoveC.Kill();
    }
    public void ParalyzePause()
    {
        onMoveC.Pause();
    }
    public void ParalyzeRecover()
    {
        onMoveC.Play();
    }
    public void OnDrawGizmos()
    {

    }
}

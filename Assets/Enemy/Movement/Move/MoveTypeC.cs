using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MoveTypeC : IMoveBehaviour
{
    public Tweener onMoveC;
    int pointCount = 0;
    float endPointWaitTime;
    public void Move(EnemyMove enemyMove)
    {
        // 先取出等待時間，方便後面使用
        float waitTime = enemyMove.pointWaitTime;
        Vector3[] path = enemyMove.moveC_PathList;
        float moveDuration = enemyMove.moveTime;
        int loopStartIndex = enemyMove.loopStartIndex;
        int loopEndIndex = enemyMove.loopEndIndex;
        int loopTime = enemyMove.loopTime;
        
        // 如果沒有路徑或物件不存在就直接離場
        if (path == null || path.Length == 0 || enemyMove.gameObject == null)
        {
            Debug.LogError("MoveTypeC: No path or enemyMove object is null. Calling Leave directly.");
            enemyMove.CallLeave();
            return;
        }

        // 建立一個空的 Sequence
        Sequence startSequence = DOTween.Sequence();
        Sequence loopSequence = DOTween.Sequence();
        Sequence endSequence = DOTween.Sequence();

        // 依序把「移動 → 等待」加入到 Sequence 裡
        for (int i = 0; i <= loopEndIndex; i++)
        {
            int currentNodeIndex = i;
            // 1) Append 一段移動 tween
            startSequence.Append(enemyMove.transform
                .DOMove(path[i], moveDuration)
                .SetEase(Ease.Linear)
                .OnComplete(() => { Debug.Log($"[startSequence] 已到達節點 index = {currentNodeIndex}"); })
            );
            // 2) Append Interval 等待
            startSequence.AppendInterval(waitTime);
        }
        for (int i = loopStartIndex; i <= loopEndIndex; i++)
        {
            int currentNodeIndex = i;
            if(currentNodeIndex == enemyMove.pointIndex[pointCount])
            {
                moveDuration = enemyMove.pointMoveTime[pointCount];
                waitTime = enemyMove.betweenPointWaitTime[pointCount];
                endPointWaitTime = enemyMove.endPointWaitTime[pointCount];
                pointCount++;
            }
            else
            {
                moveDuration = enemyMove.moveTime;
                waitTime = enemyMove.pointWaitTime;
            }
            // 1) Append 一段移動 tween
            loopSequence.Append(enemyMove.transform
                .DOMove(path[i], moveDuration)
                .SetEase(Ease.Linear)
                .OnComplete(() => { Debug.Log($"[loopSequence] 已到達節點 index = {currentNodeIndex}"); })
            );
            // 2) Append Interval 等待
            loopSequence.AppendInterval(waitTime);
        }
        for (int i = loopEndIndex; i <= path.Length - 1; i++)
        {
            // 1) Append 一段移動 tween
            endSequence.Append(enemyMove.transform
                .DOMove(path[i], moveDuration)
                .SetEase(Ease.Linear)
            );
            // 2) Append Interval 等待
            endSequence.AppendInterval(waitTime);
        }
        startSequence.AppendCallback(() => loopSequence.Play());
        loopSequence.SetLoops(loopTime, LoopType.Restart);
        //loopSequence.OnStepComplete(() => Debug.Log(loopSequence.CompletedLoops()));
        loopSequence.OnComplete(() => endSequence.Play());
        // 全部走完後呼叫離場
        endSequence.AppendCallback(() => enemyMove.CallLeave());

        // 啟動 Sequence
        startSequence.Play();
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

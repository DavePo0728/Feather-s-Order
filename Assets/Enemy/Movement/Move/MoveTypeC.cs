using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MoveTypeC : IMoveBehaviour
{
    Sequence startSequence = DOTween.Sequence();
    Sequence loopSequence = DOTween.Sequence();
    Sequence endSequence = DOTween.Sequence();
    int startPointCount = 0;
    int loopPointCount = 0;
    float endPointWaitTime;
    bool startPause = false,loopPause=false,endPause=false;
    [SerializeField]
    MoveStatus moveStatus;
    loopType loopType;
    private enum MoveStatus
    {
        Start,
        Loop,
        End
    }
    public void Move(EnemyMove enemyMove)
    {
        // 先取出等待時間，方便後面使用
        float waitTime = enemyMove.pointWaitTime;
        Vector3[] path = enemyMove.moveC_PathList;
        float moveDuration = enemyMove.moveTime;
        int loopStartIndex = enemyMove.loopStartIndex;
        int loopEndIndex = enemyMove.loopEndIndex;
        int loopTime = enemyMove.loopTime;
        loopType = enemyMove.loopType;

        // 如果沒有路徑或物件不存在就直接離場
        if (path == null || path.Length == 0 || enemyMove.gameObject == null)
        {
            Debug.LogError("MoveTypeC: No path or enemyMove object is null. Calling Leave directly.");
            enemyMove.CallLeave();
            return;
        }
        // 依序把「移動 → 等待」加入到 Sequence 裡
        for (int i = 0; i <= loopEndIndex; i++)
        {
            int currentNodeIndex = i;
            if (enemyMove.pointIndex.Count > 0)
            {
                if (currentNodeIndex >= loopStartIndex && currentNodeIndex <= loopEndIndex)
                {
                    if (currentNodeIndex == enemyMove.pointIndex[startPointCount])
                    {
                        moveDuration = enemyMove.pointMoveTime[startPointCount];
                        waitTime = enemyMove.betweenPointWaitTime[startPointCount];
                        endPointWaitTime = enemyMove.endPointWaitTime[startPointCount];
                        startPointCount++;
                    }
                    else
                    {
                        moveDuration = enemyMove.moveTime;
                        waitTime = enemyMove.pointWaitTime;
                    }
                }
                // 1) Append 一段移動 tween
                startSequence.Append(enemyMove.transform
                .DOMove(path[i], moveDuration)
                .SetEase(Ease.Linear)
                .OnComplete(() => { Debug.Log($"[startSequence] 已到達節點 index = {currentNodeIndex}"); })
                .OnStart(() =>
                {
                    moveStatus = MoveStatus.Start;
                    Debug.Log($"[startSequence] 開始移動到節點 index = {currentNodeIndex}" + $"MoveStatus: {moveStatus}");
                })
                );
                if (loopType == loopType.Yoyo)
                {
                    startSequence.SetLoops(1, LoopType.Yoyo);
                    Debug.Log($"[startSequence] 設定為 Yoyo 循環");
                }
                // 2) Append Interval 等待
                if (i == enemyMove.pointIndex[startPointCount - 1])
                {
                    startSequence.AppendInterval(endPointWaitTime);
                }
                else
                {
                    startSequence.AppendInterval(waitTime);
                }
            }
            else
            {
                moveDuration = enemyMove.moveTime;
                waitTime = enemyMove.pointWaitTime;
                startSequence.Append(enemyMove.transform
                .DOMove(path[i], moveDuration)
                .SetEase(Ease.Linear)
                .OnComplete(() => { Debug.Log($"[startSequence] 已到達節點 index = {currentNodeIndex}"); })
                .OnStart(() =>
                {
                    moveStatus = MoveStatus.Start;
                    Debug.Log($"[startSequence] 開始移動到節點 index = {currentNodeIndex}"+$"MoveStatus: {moveStatus}");
                })
                );
                startSequence.AppendInterval(waitTime);
            }
        }
        //
        // loop Sequence
        //
        if (loopTime > 0 || loopTime == -1)
        {
            if (enemyMove.pointIndex.Count > 0)
            {
                for (int i = loopStartIndex; i <= loopEndIndex; i++)
                {
                    int currentNodeIndex = i;
                    if (currentNodeIndex == enemyMove.pointIndex[loopPointCount])
                    {
                        moveDuration = enemyMove.pointMoveTime[loopPointCount];
                        waitTime = enemyMove.betweenPointWaitTime[loopPointCount];
                        endPointWaitTime = enemyMove.endPointWaitTime[loopPointCount];
                        loopPointCount++;
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
                        .OnStart(() =>
                        {
                            moveStatus = MoveStatus.Loop;
                            Debug.Log($"[loopSequence] 開始移動到節點 index = {currentNodeIndex}" + $"MoveStatus: {moveStatus}");
                        })
                    );
                    if (i == enemyMove.pointIndex[loopPointCount - 1])
                    {
                        loopSequence.AppendInterval(endPointWaitTime);
                    }
                    else
                    {
                        loopSequence.AppendInterval(waitTime);
                    }
                    // 2) Append Interval 等待
                }
                switch (loopType)
                {
                    case loopType.Loop:
                        loopSequence.SetLoops(loopTime, LoopType.Restart);
                        break;
                    case loopType.Yoyo:
                        loopSequence.SetLoops(loopTime, LoopType.Yoyo);
                        break;
                    default:
                        Debug.LogError("MoveTypeC: Invalid loop type specified.");
                        return;
                }
                loopSequence.OnComplete(() => endSequence.Play());
            }
            else
            {
                for (int i = loopStartIndex; i <= loopEndIndex; i++)
                {
                    int currentNodeIndex = i;
                    // 1) Append 一段移動 tween
                    loopSequence.Append(enemyMove.transform
                        .DOMove(path[i], moveDuration)
                        .SetEase(Ease.Linear)
                        .OnComplete(() => { Debug.Log($"[loopSequence] 已到達節點 index = {currentNodeIndex}"); })
                        .OnStart(() =>
                        {
                            moveStatus = MoveStatus.Loop;
                            Debug.Log($"[loopSequence] 開始移動到節點 index = {currentNodeIndex}" + $"MoveStatus: {moveStatus}");
                        })
                    );
                    // 2) Append Interval 等待
                    loopSequence.AppendInterval(waitTime);
                }
                switch (loopType)
                {
                    case loopType.Loop:
                        loopSequence.SetLoops(loopTime, LoopType.Restart);
                        break;
                    case loopType.Yoyo:
                        loopSequence.SetLoops(loopTime, LoopType.Yoyo);
                        break;
                    default:
                        Debug.LogError("MoveTypeC: Invalid loop type specified.");
                        return;
                }
                loopSequence.OnComplete(() => endSequence.Play());
            }
        }
        for (int i = loopEndIndex; i <= path.Length - 1; i++)
        {
            int currentNodeIndex = i;
            // 1) Append 一段移動 tween
            endSequence.Append(enemyMove.transform
                .DOMove(path[i], moveDuration)
                .SetEase(Ease.Linear)
                .OnStart(() =>
                {
                    moveStatus = MoveStatus.End;
                    Debug.Log($"[endSequence] 開始移動到節點 index = {currentNodeIndex}" + $"MoveStatus: {moveStatus}");
                })
            );
            // 2) Append Interval 等待
            endSequence.AppendInterval(waitTime);
        }
        if (loopTime > 0)
        {
            startSequence.AppendCallback(() => loopSequence.Play());
        }else if(loopTime == 0)
        {
            startSequence.AppendCallback(() => endSequence.Play());
        }else if (loopTime == -1)
        {
            startSequence.AppendCallback(() => loopSequence.Play());
        }
        // 全部走完後呼叫離場
        endSequence.AppendCallback(() => enemyMove.CallLeave());

        // 啟動 Sequence
        startSequence.Play();
    }
    public bool CheckMoveStatus()
    {
        if (startSequence.IsPlaying()||loopSequence.IsPlaying()||endSequence.IsPlaying())
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public void StopMove()
    {
        if (startSequence.IsPlaying())
            startSequence.Kill();
        if (loopSequence.IsPlaying())
            loopSequence.Kill();
        if (endSequence.IsPlaying())
            endSequence.Kill();
    }
    public void ParalyzePause()
    {
        if (startSequence.IsPlaying())
        {
            startSequence.Pause();
            startPause = true;
        }
        if (loopSequence.IsPlaying())
        {
            loopSequence.Pause();
            loopPause = true;
        }
        if (endSequence.IsPlaying())
        {
            endSequence.Pause();
            endPause = true;
        }

    }
    public void ParalyzeRecover()
    {
        if(startPause)
        {
            startSequence.Play();
            startPause = false;
        }
        if (loopPause)
        {
            loopSequence.Play();
            loopPause = false;
        }
        if (endPause)
        {
            endSequence.Play();
            endPause = false;
        }
    }
    public void OnDrawGizmos()
    {

    }
}

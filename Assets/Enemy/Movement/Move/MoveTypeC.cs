using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MoveTypeC : IMoveBehaviour
{
    Sequence startSequence = DOTween.Sequence();
    Sequence startBackSequence = DOTween.Sequence();
    Sequence loopSequence = DOTween.Sequence();
    Sequence endSequence = DOTween.Sequence();
    int startPointCount = 0;
    int loopPointCount = 0;
    float endPointWaitTime;
    bool startPause = false,loopPause=false,endPause=false;
    [SerializeField]
    MoveStatus moveStatus;
    loopType loopType;
    Vector3[] path;
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
        path = enemyMove.moveC_PathList;
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
                Debug.Log($"[start sequence] 有變速");
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
                //.OnComplete(() => { Debug.Log($"[startSequence] 已到達節點 index = {currentNodeIndex}"); })
                .OnStart(() =>
                {
                    moveStatus = MoveStatus.Start;
                    startPause = false;
                    //Debug.Log($"[startSequence] 開始移動到節點 index = {currentNodeIndex}" + $"MoveStatus: {moveStatus}");
                })
                );
                // 2) Append Interval 等待
                if (i == enemyMove.pointIndex[startPointCount - 1])
                {
                    startSequence.AppendInterval(endPointWaitTime);
                    Debug.Log($"[start sequence] 已到達變速節點 index = {currentNodeIndex}，等待時間: {endPointWaitTime}秒，下一個變速節點索引: {startPointCount}");
                }
                else
                {
                    startSequence.AppendInterval(waitTime);
                }
            }
            else
            {
                Debug.Log($"[start sequence] 沒有變速");
                moveDuration = enemyMove.moveTime;
                waitTime = enemyMove.pointWaitTime;
                startSequence.Append(enemyMove.transform
                .DOMove(path[i], moveDuration)
                .SetEase(Ease.Linear)
                //.OnComplete(() => { Debug.Log($"[startSequence] 已到達節點 index = {currentNodeIndex}"); })
                .OnStart(() =>
                {
                    moveStatus = MoveStatus.Start;
                    startPause = false;
                   // Debug.Log($"[startSequence] 開始移動到節點 index = {currentNodeIndex} Index Data: {path[currentNodeIndex]} MoveStatus: {moveStatus}");
                })
                );
                
                startSequence.AppendInterval(waitTime);
            }
        }
        if (loopTime != 0)
        {
            if(loopType == loopType.Yoyo)
            {
                
                startSequence.OnComplete(() => startBackSequence.Play());
                
            }
            else
            {
                startSequence.OnComplete(() => loopSequence.Play());
            }
        }
        else if (loopTime == 0)
        {
            if (loopType == loopType.Yoyo)
            {
                startSequence.OnComplete(() => startBackSequence.Play());
            }
            else
            {
                startSequence.OnComplete(() => endSequence.Play());
            }

        }
        //
        // start Back Sequence
        //
        if (loopType == loopType.Yoyo)
        {
            for (int i = loopEndIndex-1; i >= 0; i--)
            {              
                int currentNodeIndex = i;
                if (enemyMove.pointIndex.Count > 0)
                {
                    Debug.Log($"[start back sequence] 有變速");
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
                    startBackSequence.Append(enemyMove.transform
                    .DOMove(path[i], moveDuration)
                    .SetEase(Ease.Linear)
                    //.OnComplete(() => { Debug.Log($"[startSequence] 已到達節點 index = {currentNodeIndex}"); })
                    .OnStart(() =>
                    {
                        moveStatus = MoveStatus.Start;
                        startPause = false;
                        //Debug.Log($"[startBackSequence] 開始移動到節點 index = {currentNodeIndex}" + $"MoveStatus: {moveStatus}");
                    })
                    );
                    // 2) Append Interval 等待
                    if (i == enemyMove.pointIndex[startPointCount - 1])
                    {
                        startBackSequence.AppendInterval(endPointWaitTime);
                    }
                    else
                    {
                        startBackSequence.AppendInterval(waitTime);
                    }
                }
                else
                {
                    Debug.Log($"[start back sequence] 沒有變速");
                    moveDuration = enemyMove.moveTime;
                    waitTime = enemyMove.pointWaitTime;
                    startSequence.Append(enemyMove.transform
                    .DOMove(path[i], moveDuration)
                    .SetEase(Ease.Linear)

                    //.OnComplete(() => { Debug.Log($"[startBackSequence] 已到達節點 index = {currentNodeIndex}"); })
                    .OnStart(() =>
                    {
                        moveStatus = MoveStatus.Start;
                        startPause = false;
                        //Debug.Log($"[startBackSequence] 開始移動到節點 index = {currentNodeIndex}" + $"MoveStatus: {moveStatus}");
                    })
                    );

                    startBackSequence.AppendInterval(waitTime);
                }
            }
            if (loopTime != 0)
            {
                startBackSequence.OnComplete(() => loopSequence.Play());
            }
            else if (loopTime == 0)
            {
                startBackSequence.OnComplete(() => endSequence.Play());
            }
        }
        //
        // loop Sequence
        //
        if (loopTime > 0 || loopTime == -1)
        {
            //Debug.Log($"[loopSequence] 循環次數: {loopTime}，循環類型: {loopType}");
            if (enemyMove.pointIndex.Count > 0)
            {
                for (int i = loopStartIndex; i <= loopEndIndex; i++)
                {
                    int currentNodeIndex = i;
                    if(i ==0&&loopType ==loopType.Yoyo)
                        i=1;
                    
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
                        //.OnComplete(() => { Debug.Log($"[loopSequence] 已到達節點 index = {currentNodeIndex}"); })
                        .OnStart(() =>
                        {
                            moveStatus = MoveStatus.Loop;
                            loopPause = false;
                            //Debug.Log($"[loopSequence] 循環次數: {loopTime}，循環類型: {loopType}");
                            //Debug.Log($"[loopSequence] 開始移動到節點 index = {currentNodeIndex}" + $"MoveStatus: {moveStatus}");
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
                    if (i == 0 && loopType == loopType.Yoyo)
                        i = 1;
                    // 1) Append 一段移動 tween
                    loopSequence.Append(enemyMove.transform
                        .DOMove(path[i], moveDuration)
                        .SetEase(Ease.Linear)
                        //.OnComplete(() => { Debug.Log($"[loopSequence] 已到達節點 index = {currentNodeIndex}"); })
                        .OnStart(() =>
                        {
                            moveStatus = MoveStatus.Loop;
                            loopPause = false;
                            //Debug.Log($"[loopSequence] 循環次數: {loopTime}，循環類型: {loopType}");
                            //Debug.Log($"[loopSequence] 開始移動到節點 index = {currentNodeIndex}" + $"MoveStatus: {moveStatus}");
                        })
                    );
                    // 2) Append Interval 等待
                    loopSequence.AppendInterval(waitTime);
                }
                switch (loopType)
                {
                    case loopType.Loop:
                        if (loopTime == 0)
                            return;
                        //Debug.Log($"[loopSequence] 設定為 Loop 循環，循環次數: {loopTime}");
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
        //
        // end Sequence
        //
        for (int i = loopEndIndex+1; i <= path.Length - 1; i++)
        {
            int currentNodeIndex = i;
            // 1) Append 一段移動 tween
            endSequence.Append(enemyMove.transform
                .DOMove(path[currentNodeIndex], moveDuration)
                .SetEase(Ease.Linear)
                .OnStart(() =>
                {
                    moveStatus = MoveStatus.End;
                    endPause = false;
                    //Debug.Log($"[endSequence] 開始移動到節點 index = {currentNodeIndex} Index Data: {path[currentNodeIndex]} MoveStatus: {moveStatus}");
                })
            );
            // 2) Append Interval 等待
            endSequence.AppendInterval(waitTime);
        }
        // 全部走完後呼叫離場
        endSequence.AppendCallback(() => enemyMove.CallLeave());

        // 啟動 Sequence
        startSequence.Play();
    }
    public bool CheckMoveStatus()
    {
        if (/*startPause||loopPause||endPause||*/startSequence.IsPlaying()|| loopSequence.IsPlaying()|| endSequence.IsPlaying())
        {
            Debug.Log($"MoveTypeC: MoveStatus is playing. Current status: {loopSequence.IsPlaying()}");
            return true;
        }
        else
        {
            Debug.Log($"MoveTypeC: MoveStatus is not playing. Current status: {loopSequence.IsPlaying()}");
            return false;
        }
    }
    public void StopMove()
    {
        if (startSequence.IsActive())
            startSequence.Kill();
        if(loopSequence.IsActive())
            loopSequence.Kill();
        if (endSequence.IsActive())
            endSequence.Kill();
    }
    public void ParalyzePause()
    {
        if (startSequence.IsPlaying())
        {
            startSequence.Pause();
            startPause = true;
            return;
        }
        if (loopSequence.IsPlaying())
        {
            Debug.Log($"MoveTypeC: LoopSequence is paused.");
            loopSequence.Pause();
            loopPause = true;
            return;
        }
        if (endSequence.IsPlaying())
        {
            endSequence.Pause();
            endPause = true;
            return;
        }

    }
    public void ParalyzeRecover()
    {
        if(startPause)
        {
            startSequence.Play();
            startPause = false;
            return;
        }
        if (loopPause)
        {
            loopSequence.Play();
            loopPause = false;
            return;
        }
        if (endPause)
        {
            endSequence.Play();
            endPause = false;
            return;
        }
    }
}

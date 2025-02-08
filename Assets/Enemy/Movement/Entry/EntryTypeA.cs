using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EntryTypeA : IEntryBehaviour
{

    public void Enter(EnemyMove enemyMove)
    {
        Debug.Log("Enter Type A");
        Tweener moveTweener = enemyMove.transform.DOPath(enemyMove.path, enemyMove.enterTime).SetEase(Ease.InOutSine);
        moveTweener.OnComplete(() =>{enemyMove.isMove = true;});
    }
}


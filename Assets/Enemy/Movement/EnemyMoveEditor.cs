using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(EnemyMoveA))]
public class EnemyMoveEditor : Editor
{
    //EnemyMoveA enemyMoveA_target;
    //EnemyMove enemyMove_target;

    //public override void OnInspectorGUI()
    //{
    //    //DrawDefaultInspector();
    //    enemyMoveA_target = (EnemyMoveA)target;
    //    enemyMove_target = (EnemyMove)target;
    //    EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((EnemyMoveA)target), typeof(EnemyMoveA), false);
    //    // 陪ボ EnterType 闽把计
    //    enemyMoveA_target.enterType = (EnemyMoveA.EnterType)EditorGUILayout.EnumPopup("Enter Type", enemyMoveA_target.enterType);
    //    switch (enemyMoveA_target.enterType)
    //    {
    //        case EnemyMoveA.EnterType.FromCamera:
    //            enemyMoveA_target.moveSpeed = EditorGUILayout.FloatField("Speed", enemyMoveA_target.moveSpeed);
    //            //enemyMove_target.rotationSpeed = EditorGUILayout.FloatField("Rotation Speed", enemyMove_target.rotationSpeed);
    //            // 陪ボ FromCamera 闽把计
    //            break;
    //        case EnemyMoveA.EnterType.AroundScreen:
    //            // 陪ボ AroundScreen 闽把计
    //            //enemyMoveA_target.moveSpeed = EditorGUILayout.FloatField("Speed", enemyMoveA_target.moveSpeed);
    //            break;
    //        case EnemyMoveA.EnterType.FarToPlayer:
    //            // 陪ボ FarToPlayer 闽把计
    //            enemyMoveA_target.moveSpeed = EditorGUILayout.FloatField("Speed", enemyMoveA_target.moveSpeed);
    //            break;
    //    }

    //    // 陪ボ MoveType 闽把计
    //    enemyMoveA_target.moveType = (EnemyMoveA.MoveType)EditorGUILayout.EnumPopup("Move Type", enemyMoveA_target.moveType);
    //    switch (enemyMoveA_target.moveType)
    //    {
    //        case EnemyMoveA.MoveType.RandomPath:
    //            // 陪ボ RandomPath 闽把计
    //            break;
    //        case EnemyMoveA.MoveType.RandomSmall:
    //            // 陪ボ RandomSmall 闽把计
    //            break;
    //        case EnemyMoveA.MoveType.RandonBig:
    //            // 陪ボ RandonBig 闽把计
    //            break;
    //        case EnemyMoveA.MoveType.Path:
    //            // 陪ボ Path 闽把计
    //            break;
    //    }

    //    // 陪ボ LeaveType 闽把计
    //    enemyMoveA_target.leaveType = (EnemyMoveA.LeaveType)EditorGUILayout.EnumPopup("Leave Type", enemyMoveA_target.leaveType);
    //    switch (enemyMoveA_target.leaveType)
    //    {
    //        case EnemyMoveA.LeaveType.AroundScreen:
    //            // 陪ボ AroundScreen 闽把计
    //            break;
    //        case EnemyMoveA.LeaveType.FlyToPlayer:
    //            // 陪ボ FlyToPlayer 闽把计
    //            break;
    //    }

    //    // 絋玂э计沮砆玂
    //    if (GUI.changed)
    //    {
    //        EditorUtility.SetDirty(enemyMoveA_target);
    //    }
    //}
}

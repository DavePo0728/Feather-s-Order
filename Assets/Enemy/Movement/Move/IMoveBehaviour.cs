using System.Collections;
using System.Collections.Generic;

public interface IMoveBehaviour 
{
    void Move(EnemyMove enemyMove);
    void StopMove();
    bool CheckMoveStatus();
    void ParalyzePause();
    void ParalyzeRecover();
}

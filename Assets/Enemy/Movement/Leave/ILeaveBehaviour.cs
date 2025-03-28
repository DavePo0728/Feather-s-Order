using System.Collections;
using System.Collections.Generic;

public interface ILeaveBehaviour
{
    void Leave(EnemyMove enemyMove);
    void StopLeave();
    bool CheckLeaveStatus();
    void ParalyzePause();
    void ParalyzeRecover();
}

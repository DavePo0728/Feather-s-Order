using System.Collections;
using System.Collections.Generic;
public interface IEntryBehaviour
{
    void Enter(EnemyMove enemyMove);
    void StopEnter();
    bool CheckEntryStatus();
    void ParalyzePause();
    void ParalyzeRecover();
}

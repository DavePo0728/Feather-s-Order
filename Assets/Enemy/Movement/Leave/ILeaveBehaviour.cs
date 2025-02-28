using System.Collections;
using System.Collections.Generic;

public interface ILeaveBehaviour
{
    void Leave(EnemyMove enemyMove);
    void StopLeave();
}

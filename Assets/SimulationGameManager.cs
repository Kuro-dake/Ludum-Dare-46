using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationGameManager : GameManager
{

    public void CombatSimulation()
    {
        GM.characters.NewTurn();
        int watchdog = 0;
        GM.characters.NextCharacterTurn(true);
        while (GM.characters.any_enemies_alive)
        {
            GM.characters.NextCharacterTurn();

            if(watchdog++ > 1000)
            {
                break;
            }
        }

    }

}

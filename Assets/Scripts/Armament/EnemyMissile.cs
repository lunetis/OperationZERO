using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMissile : Missile
{
    protected override void AdjustValuesByDifficulty()
    {
        smartTrackingRate = MissionData.GetFloatFromDifficultyXML("enemyMissileTrackingRate", smartTrackingRate);
        boresightAngle = MissionData.GetFloatFromDifficultyXML("enemyMissileBoresightAngle", boresightAngle);
        turningForce = MissionData.GetFloatFromDifficultyXML("enemyMissileTurningForce", turningForce);

        Debug.Log("Adjuested missile values");
    }
}

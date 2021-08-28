using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMissile : Missile
{
    protected override void AdjustValuesByDifficulty()
    {
        smartTrackingRate = MissionData.GetFloatFromDifficultyXML("enemyMissileTrackingRate");
        boresightAngle = MissionData.GetFloatFromDifficultyXML("enemyMissileBoresightAngle");
        turningForce = MissionData.GetFloatFromDifficultyXML("enemyMissileTurningForce");

        Debug.Log("Adjuested missile values");
    }
}

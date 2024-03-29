﻿namespace Core.CoreEnums
{
    public enum ServerToClientId : ushort
    {
        playerSpawned = 1,
        playerMovement,
        characterStatsInit,
        sync,
        eventTriggered,
        weaponChange,
        combatData,
        idAction,
        healthChange,
    }

    public enum ClientToServerId : ushort
    {
        name = 1,
        input,
        changeWeapon,
    }
}
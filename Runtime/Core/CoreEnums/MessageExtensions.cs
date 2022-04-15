﻿namespace Core.CoreEnums
{
    public enum ServerToClientId : ushort
    {
        playerSpawned = 1,
        playerMovement,
        characterStatsInit,
        sync
    }

    public enum ClientToServerId : ushort
    {
        name = 1,
        input
    }
}
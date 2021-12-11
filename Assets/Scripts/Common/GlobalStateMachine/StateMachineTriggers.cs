namespace ShootCommon.GlobalStateMachine
{
    public enum StateMachineTriggers 
    {
        Start,
        InitCachingService,
        SelectConfigState,
        ReadCachingConfigState,
        ReadCachingUserInfoState,
        GetAlternativeConfigsState,
        CreatePreloadersState,
        
        LobbyState,
        LobbyMainState,
        LobbySelectLocationState,
        ShopState,
        PVPState,
        OpenChestState,
        CollectionsState,
        
        GamePlayPreloaderState,
        #region Game Play Preloader Substates
        StartSetupGameSubstate,
        CreateInteractiveItemsSubstate,
        InteractiveItemsSetupSubstate,
        #endregion
        
        MainGamePlayState,
        #region Main Game Play Substates
        MainGameSubstate,
        PauseGameSubstate,
        AddTimeGameSubstate,
        #endregion

        #region Rooms
        StartRoomsSubstate,
        CreateRoomInteractiveItemsSubstate,
        PlayStartRoomsAnimationSubstate,
        RoomInteractiveItemsSetupSubstate,
        RoomMainGamePlaySubstate,
        NextRoomPlaySubstate,
        #endregion
        
        LoseGameState,
        WinGameState,
        EndGameState,
        
        GameplayTestLevelState
    }
}
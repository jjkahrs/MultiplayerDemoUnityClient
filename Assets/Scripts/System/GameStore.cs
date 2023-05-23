using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStore {

    // Note to move this to something configurable eventually.
    public static float STATE_UPDATE_INTERVAL = (1f / 10f); // 10 ticks per second

    private const int MAX_SNAPSHOT_SIZE = 10;
    private const int MAX_PLAYER_INPUT_SIZE = 100;

    public long gameTick;

    public string localSessionId;
    public Actor localActor;
    public List<Actor> remoteActors = new List<Actor>();

    private List<FrameSnapshot> snapshots = new List<FrameSnapshot>();
    
    public List<PlayerInputFrame> playerInputFrames = new List<PlayerInputFrame>();

    public FrameSnapshot lastFrameFromServer = null;

    public GameStore() {    
        gameTick = 0;    
    }

    public FrameSnapshot GetCurrentFrameSnapshot()
    {
        if( snapshots.Count == 0 )
            return null;
        
        return snapshots[0];
    }

    public FrameSnapshot GetPreviousFrameSnapshot()
    {
        if( snapshots.Count < 2 )
            return null;
        
        return snapshots[1];
    }

    public FrameSnapshot GetFrameSnapshot( int index )
    {
        if( index >= snapshots.Count )
            return null;

        return snapshots[ index ];
    }

    public void AddFrameSnapshot( FrameSnapshot inSnapshot )
    {
        inSnapshot.transitTimeMillis = ( Utils.CurrentTimeMillis() - inSnapshot.timestamp);
        snapshots.Insert( 0, inSnapshot );

        if( snapshots.Count > MAX_SNAPSHOT_SIZE )
            snapshots.RemoveAt( snapshots.Count - 1 );
    }


    public PlayerInputFrame GetCurrentPlayerInputFrame()
    {
        if( playerInputFrames.Count == 0 )
            return null;
        
        return playerInputFrames[0];
    }

    public PlayerInputFrame GetPreviousPlayerInputFrame()
    {
        if( playerInputFrames.Count < 2 )
            return null;
        
        return playerInputFrames[1];
    }

    public PlayerInputFrame GetPlayerInputFrame( int index )
    {
        if( index >= playerInputFrames.Count || index < 0 )
            return null;

        return playerInputFrames[ index ];
    }

    public PlayerInputFrame AddPlayerInputFrame( PlayerInputFrame inFrame )
    {
        inFrame.inputTimestamp = Utils.CurrentTimeMillis();
        playerInputFrames.Insert( 0, inFrame );

        if( playerInputFrames.Count > MAX_PLAYER_INPUT_SIZE )
            playerInputFrames.RemoveAt( playerInputFrames.Count - 1 );

        return inFrame;
    }

}
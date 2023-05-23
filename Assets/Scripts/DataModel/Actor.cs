using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Actor
{
    public string id;
    public string name;
    public Vector3 position;
    public Vector3 headingNormal;
    public float speed;

    public Actor( string id )
    {
        this.id = id;
        this.name = "";
        this.position = Vector3.zero;
        this.headingNormal = Vector3.zero;
        this.speed = 0f;
    }

    public static Actor Parse( string segment )
    {
        string[] keyValPairs = segment.Split(",");

        Vector3 pos = Vector3.zero;
        Vector3 heading = Vector3.zero;
        float speed = 0f;
        string sessionId = "";

        foreach( string keyValPair in keyValPairs )
        {
            string[] parts = keyValPair.Split(":");
            if( parts.Length != 2 )
                continue;

            string key = parts[0];
            string value = parts[1];

            switch( key )
            {
                case "sessionId":
                    sessionId = value;
                    break;
                case "posX":
                    pos.x = Convert.ToSingle( value );
                    break;
                case "posY":
                    pos.y = Convert.ToSingle( value );
                    break;
                case "posZ":
                    pos.z = Convert.ToSingle( value );
                    break;
                case "headingX":
                    heading.x = Convert.ToSingle( value );
                    break;
                case "headingY":
                    heading.y = Convert.ToSingle( value );
                    break;
                case "headingZ":
                    heading.z = Convert.ToSingle( value );
                    break;
                case "speed":
                    speed = float.Parse( value );
                    break;
                default:
                    break;
            }

        }

        Actor actor = new Actor( sessionId );
        actor.position = pos;
        actor.headingNormal = heading;
        actor.speed = speed;

        return actor;
    }
}
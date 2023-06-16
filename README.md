# MultiplayerDemoUnityClient
Unity Version 2021.3.6f1

## Overview
This is the Unity client for a simple multiplayer movement demo used with different implementations of the backend server. The client doesn't use a textbook methodology to handle network latency because one of the servers is written in Javascript/Typescript. JS is single-threaded, so when exactly setInterval() is executed is non-deterministic. So the client can't set itself to a constant tick rate. It has to adjust based on the timestamp intervals between each server update messages.

All communication is done via TCP so this demo isn't an example for a low-tolerance game like an FPS.

## Server Implementations
The available server implementations can be found here:
- [NodeJS](https://github.com/jjkahrs/multiplayer-demo-server-ts)

## Message Format
The client uses a custom message format for what is probably a very tiny performance improvement. To date, I have not run metrics against the use of JSON to compare. Messages are divided into segments that are pipe "|" delimited. The first segment is the Unix timestamp of the message's creation date (not the arrival date). The second segment is the message type. Other segments are optional. The end of the message is terminated with ";".

Object data is comma "," deliminated with key/value pairs being separated by a colon ":". Float values are limited to 3 decimal places. 

### Connection Handshake
Once the TCP connection is made, the client immediately sends their session id. In the larger multiplayer architecture, this would be the authentication token the client recieved from the login system. The format is:

`Token:sessionId`

When the server has finished authenticating the token and initializing the session, it will notify the client with the message:

`SessionReady`

At this point the server will start sending tick updates to the client and the client is free to start sending messages to update the server state or request specific data.

### Client to Server Messages

#### ReqPlayerState
Client request for the current state of the player attached to this connection. Segments:
1. Unix Timestamp
2. ReqPlayerState

#### ReqWorldState
Client request for the current state of the world. Segments:
1. Unix Timestamp
2. ReqWorldState

#### PlayerInput
Player's position change request for this current frame. Segments:
1. Segment 1: Unix Timestamp
2. Segment 2: PlayerInput
3. Segment 3: The player's submitted movement data in formated object data key/value pairs.
- **tick**: (long) Client's current frame number
- **headingX** (float) X component of the entity's normalized heading vector
- **headingY** (float) Y component of the entity's normalized heading vector
- **headingZ** (float) Z component of the entity's normalized heading vector
- **speed** (float) Speed for this frame.
- **duration** (float) The number of milliseconds this speed is maintained during this frame (might be less than tick duraction).
- **facingX** (float) X component of the entity's current facing
- **facingY** (float) Y component of the entity's current facing
- **facingZ** (float) Z component of the entity's current facing

Example:
`1686933133197|PlayerInput|tick:0,speed:2,duration:99,headingX:0,headingY:0,headingZ:0,facingX:0,facingY:0,facingZ:1`

### Server to Client Messages

#### WorldDelta
Server world state for the current frame tick. Segments:
1. Unix Timestamp
2. WorldState
3. Frame tick duration in milliseconds
4. Server frame tick number
5. Sub-segments of session state data deliminated by carat "^". The session data is formated object data key/value pairs.
- **SessionId** (string) Entity ID for the tracked object
- **posX** (float) X position on the map
- **posY** (float) Y position on the map
- **posZ** (float) Z position on the map
- **headingX** (float) X component of the entity's normalized heading vector
- **headingY** (float) Y component of the entity's normalized heading vector
- **headingZ** (float) Z component of the entity's normalized heading vector
- **speed** (float) Current speed
- **facingX** (float) X component of the entity's current facing
- **facingY** (float) Y component of the entity's current facing
- **facingZ** (float) Z component of the entity's current facing

Example: 
`1686933132878|WorldDelta|0.108|667732|sessionId:162,posX:2.000,posY:0.000,posZ:2.000,headingX:0.000,headingY:0.000,headingZ:0.000,speed:2.000,facingX:0.000,facingY:0.000,facingZ:0.000`

#### PlayerState
Server response for the "ReqPlayerState" data request. Segments:
1. Unix Timestamp
2. PlayerState
3. The player's  data in formated object data key/value pairs.
- **SessionId** (string) Entity ID for the tracked object
- **posX** (float) X position on the map
- **posY** (float) Y position on the map
- **posZ** (float) Z position on the map
- **headingX** (float) X component of the entity's normalized heading vector
- **headingY** (float) Y component of the entity's normalized heading vector
- **headingZ** (float) Z component of the entity's normalized heading vector
- **speed** (float) Current speed
- **facingX** (float) X component of the entity's current facing
- **facingY** (float) Y component of the entity's current facing
- **facingZ** (float) Z component of the entity's current facing

Example:
`1686933132.986|PlayerState|sessionId:162,posX:2.000,posY:0.000,posZ:2.000,headingX:0.000,headingY:0.000,headingZ:0.000,speed:2.000,facingX:0.000,facingY:0.000,facingZ:0.000`

#### WorldState
Server response for the "ReqWorldState" data request. Segments:
1. Unix Timestamp
2. WorldState
3. Sub-segments of session state data deliminated by carat "^". The session data is formated object data key/value pairs.
- **SessionId** (string) Entity ID for the tracked object
- **posX** (float) X position on the map
- **posY** (float) Y position on the map
- **posZ** (float) Z position on the map
- **headingX** (float) X component of the entity's normalized heading vector
- **headingY** (float) Y component of the entity's normalized heading vector
- **headingZ** (float) Z component of the entity's normalized heading vector
- **speed** (float) Current speed
- **facingX** (float) X component of the entity's current facing
- **facingY** (float) Y component of the entity's current facing
- **facingZ** (float) Z component of the entity's current facing

Example:
`1686933132.986|WorldState|sessionId:162,posX:2.000,posY:0.000,posZ:2.000,headingX:0.000,headingY:0.000,headingZ:0.000,speed:2.000,facingX:0.000,facingY:0.000,facingZ:0.000`


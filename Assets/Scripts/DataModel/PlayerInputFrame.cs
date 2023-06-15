using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

public class PlayerInputFrame
{
    public const string SEND_MESSGE_TYPE = "PlayerInput";

    public long inputTimestamp;
    public long inputTick;
    public Vector3 inputHeading;
    public float inputSpeed;
    public Vector3 inputPosition;
    public long inputDuration;
    public Vector3 inputFacing;

    public string BuildSendMessage()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(inputTimestamp).Append("|");
        sb.Append(SEND_MESSGE_TYPE).Append("|");
        sb.Append("tick:").Append(inputTick).Append(",");
        sb.Append("speed:").Append(inputSpeed).Append(",");
        sb.Append("duration:").Append(inputDuration).Append(",");
        sb.Append("headingX:").Append(inputHeading.x).Append(",");
        sb.Append("headingY:").Append(inputHeading.y).Append(",");
        sb.Append("headingZ:").Append(inputHeading.z).Append(",");

        sb.Append("facingX:").Append(inputFacing.x).Append(",");
        sb.Append("facingY:").Append(inputFacing.y).Append(",");
        sb.Append("facingZ:").Append(inputFacing.z);

        return sb.ToString();
    }
}
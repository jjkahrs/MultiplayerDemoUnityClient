using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Utils
{
    public static long CurrentTimeMillis()
    {
        DateTimeOffset dto = new DateTimeOffset(DateTime.UtcNow);
        return dto.ToUnixTimeMilliseconds();
    }
}
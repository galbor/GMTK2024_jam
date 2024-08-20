using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KeyKeeper
{
    private static int keys = 0;
    
    public static void GetKey()
    {
        keys++;
    }
    public static void RemoveKey()
    {
        keys--;
    }

    public static int KeysAmt()
    {
        return keys;
    }

    public static void Reset()
    {
        keys = 0;
    }
}

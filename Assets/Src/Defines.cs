using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Defines
{

    public class LevelDefines
    {
        // Determines how much "room space" there is in a normal level of 20x80.
        public readonly static int s_MAX_CAPACITY = 350;

        // Determines the maximal percentage of doors per room 
        public readonly static int s_MAX_DOOR_PRC = 20;

        // Determines how large does the envelop need to be to contain a large room (12 or 8).
        public readonly static int s_LARGE_ROOM_THRESHOLD = 28;

        // For all max dimensions, actual max dimensions is one lower.
        public readonly static int s_LARGE_ROOM_MAX_WIDTH = 15;
        public readonly static int s_ROOM_MAX_WIDTH = 11;
        public readonly static int s_ROOM_MAX_HEIGHT = 7;
        public readonly static int s_ROOM_MIN_WIDTH_OR_HEIGHT = 2;

        public readonly static int s_ROOM_MAX_AREA = 50;
    }
}
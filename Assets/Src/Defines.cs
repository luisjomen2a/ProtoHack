using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Defines
{

    public class LevelDefines
    {
        // Determines the cases around the level where nothing will be generated.
        public readonly static int s_Y_BORDER_SIZE = 3;
        public readonly static int s_X_BORDER_SIZE = 4;

        // Determines the maximal percentage of doors per room 
        public readonly static int s_MAX_DOOR_PRC = 20;

        public readonly static int s_MAX_ENVELOP_COUNT = 20;

        // Determines how large does the envelop need to be to contain a large room (12 or 8).
        public readonly static int s_LARGE_ROOM_THRESHOLD = 28;

        // For all max dimensions, actual max dimensions is one lower.
        public readonly static int s_LARGE_ROOM_MAX_WIDTH = 14;
        public readonly static int s_ROOM_MAX_WIDTH = 10;
        public readonly static int s_ROOM_MAX_HEIGHT = 6;
        public readonly static int s_ROOM_MIN_WIDTH = 3;
        public readonly static int s_ROOM_MIN_HEIGHT = 2;

        public readonly static int s_ROOM_MAX_AREA = 50;
    }
}
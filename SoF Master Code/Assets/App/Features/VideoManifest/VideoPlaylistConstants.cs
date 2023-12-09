using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VideoPlaylistConstants {
    static SimpleCache _cache;
    public static SimpleCache Cache {
        get {
            if (_cache == null) {
                _cache = SimpleCache.Load("/video");
            }
            return _cache;
        }
    }
}

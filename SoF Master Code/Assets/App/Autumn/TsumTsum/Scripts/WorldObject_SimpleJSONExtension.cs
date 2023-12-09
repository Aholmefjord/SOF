using System;
using AutumnInteractive.SimpleJSON;

public static class WorldObject_SimpleJSONExtension {
    /// <summary>
    /// Extract the field from json and return the result. If the json or the field cannot be found, return default value
    /// Typename T can only be int, float, bool and string
    /// </summary>
    public static T GetEntry<T> (this JSONNode json, string key, T defaultValue = default (T)) where T : IConvertible {
        if (json == null) {
            return defaultValue;
        }
        if (json[key] == null) {
            return defaultValue;
        }

        var type = typeof (T);
        if (type == typeof (int)) {
            return (T) Convert.ChangeType (json[key].AsInt, typeof (T));
        } else if (type == typeof (float)) {
            return (T) Convert.ChangeType (json[key].AsFloat, typeof (T));
        } else if (type == typeof (bool)) {
            return (T) Convert.ChangeType (json[key].AsBool, typeof (T));
        } else if (type == typeof (string)) {
            return (T) Convert.ChangeType (json[key].ToProperString (), typeof (T));
        } else {
            return defaultValue;
        }
    }

    /// <summary>
    /// Extract the field from json and return the result. If the json or the field cannot be found, return default value
    /// Typename T can only be JsonNode instance
    /// </summary>
    public static T GetJson<T> (this JSONNode json, string key, T defaultValue = default (T)) where T : JSONNode {
        if (json == null) {
            return defaultValue;
        }
        if (json[key] == null) {
            return defaultValue;
        }
        return (T) json[key];
    }
}
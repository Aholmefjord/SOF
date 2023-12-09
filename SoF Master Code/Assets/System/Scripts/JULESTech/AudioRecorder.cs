using UnityEngine;

public class AudioRecorder {

    #region Data
    private static AudioClip aud_clip;  // cache
    // these to be replaced with data class?
    private const string deviceName = "Built-in Microphone";
    private const int sampleRate = 44100;
    private const int recordLengthInSecs = 60;
    #endregion

    #region accessors;
    /// <summary>
    /// the last recorded clip
    /// </summary>
    public static AudioClip LastCachedAudioClip
    {
        get {
            return aud_clip;
        }
    }

    public static bool IsRecording()
    {
        return Microphone.IsRecording(deviceName);
    }

    public static int MaxRecordingDuration { get { return recordLengthInSecs; } }
    #endregion

    public static bool StartRecording()
    {
        bool result = false;

        if (!IsRecording())
        {
            aud_clip = Microphone.Start(deviceName, true, recordLengthInSecs, sampleRate);
            if(aud_clip!=null)
                result = true;
        }
        return result;
    }

    public static AudioClip StopRecording()
    {
        // return nothing if microphone is not recording
        if (!IsRecording())
            return null;
        //Debug.Log("[StopRecording] Success Start");
        //Capture the current clip data
        int position = Microphone.GetPosition(deviceName);
        float[] soundData = new float[aud_clip.samples * aud_clip.channels];
        aud_clip.GetData(soundData, 0);

        //Create shortened array for the data that was used for recording
        var newData = new float[position * aud_clip.channels];

        //Copy the used samples to a new array
        for (int i = 0; i < newData.Length; i++) {
            newData[i] = soundData[i];
        }
        //One does not simply shorten an AudioClip,
        //    so we make a new one with the appropriate length
        AudioClip newAudioClip = AudioClip.Create(aud_clip.name,
            position,
            aud_clip.channels,
            aud_clip.frequency,
            false);
        //newAudioClip = SavWav.TrimSilence(newAudioClip, .1f);
        newAudioClip.SetData(newData, 0);        //Give it the data from the old clip

        //Debug.Log("[StopRecording] new audioclip made");
        // Remove old clip, getting ready for new clip
        AudioClip.Destroy(aud_clip);
        aud_clip = newAudioClip;

        // Data grabbed, Stop recording!
        Microphone.End(deviceName);
        Debug.Log("[AudioRecorder::StopRecording] recording ended");
        return aud_clip;
    }

    // cancel recording session
    public static void CancelRecording()
    {
        // if the microphone is not recording, no need to cancel
        if (!IsRecording())
            return;
        Microphone.End(deviceName);
        aud_clip = null;
        Debug.Log("[AudioRecorder::CancelRecording] recording session canceled.");
    }
    #region data conversion, static functions
    public static string ConvertAudioClip2Base64String(AudioClip clip)
    {
        byte[] currData = WavUtility.FromAudioClip(clip);
        return System.Convert.ToBase64String(currData);
    }
    public static AudioClip ConvertBase64String2AudioClip(string base64data)
    {
        //Debug.Log("[AudioRecorder::ConvertBase64String2AudioClip]: " + base64data);
        byte[] data = System.Convert.FromBase64String(base64data);
        return WavUtility.ToAudioClip(data); ;
    }
    #endregion
}
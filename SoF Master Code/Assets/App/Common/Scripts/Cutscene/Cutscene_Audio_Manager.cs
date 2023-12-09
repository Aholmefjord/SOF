using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class Cutscene_Audio_Manager : MonoBehaviour
{
    // Constants
    [SerializeField]
    float AUDIO_FADE_SPEED                = 0.99f;
    [SerializeField]
    float AUDIO_FADE_REMOVAL_THRESHOLD    = 0.05f;
    [SerializeField]
    float AUDIO_FADE_SPEED_TICK           = 0.02f;

    // String of XML file to be read
    [SerializeField]
    string XMLFileName;

    // Serialize the CutsceneManager to extract the Time Elapsed
    [SerializeField]
    Cutscene_Manager CSMGR;

    // If this is enabled, audio will follow the timings instead of transiting frame from frame
    // Default to true
    [SerializeField]
    bool Time_Based_Transition = true;

    // Prefix path for the audio location
    [SerializeField]
    string Audio_File_Prefix_Path;

    // Counter to track the current 
    private int p_Track_Counter = 0;

    // Track the size of the container
    private int p_Track_Limit;

    // List of to be faded Audio
    private List<string> p_Fading_Audio = new List<string>();

    // Fade Speed Tracker
    private float p_Fade_Ticks = 0.0f;

    // Data container
    private Cutscene_Serializer Container;

	// String to find the GameObject to be destroyed
	private string ToBeDestroyed = "";

	// Temporary variable to store the TimeElapsed from CSMGR
	private float tmpTimeElapsed;

	// Store the AudioSources temporarily
	private AudioSource[] tmpArray;

	// Store the names of the tmpArray
	private string[] tmpStringArray;

	// Use this for initialization
	void Start()
    {
		//Container = Cutscene_Serializer.Load(Path.Combine(Application.dataPath, XMLFileName));
		Container = Cutscene_Serializer.Load(Path.Combine("data/", XMLFileName));

		p_Track_Limit = Container.TotalSizeAudio();

        Container.DebugPrintAllAudio();
    }

    // Update is called once per frame
    void Update()
    {
        if (!CheckLimit())
        {
            // Play the audio that is on right now
            PlayAudio();

            // Audio Transition
            TransitAudio();
        }

        // Audio Fades
		if (CSMGR.interval - CSMGR.TimeToFade > CSMGR.TimeElapsed)
			FadeAudio();

        // Force destroy expired loop SFX
        DestroyLoopedSFX();
    }

    // Transit audio function, public function to be called from other objects to transit the audio
    public void Transit_Audio_Frame()
    {
        ++p_Track_Counter;
    }

    // Play Audio Helper function, play the designated audio
    private void PlayAudio()
    {
        Cutscene_Audio tmp = Container.AudioList[p_Track_Counter];

        // Play the audio, determine if it loops or not
        if (!tmp.Played)
        {
            // If it is a time based transition, exit function if it is not time to play yet
            if (Time_Based_Transition && tmp.Time > CSMGR.TimeElapsed)
            {
                return;
            }

            // Play the audio
            AudioSystem.PlaySFX(Audio_File_Prefix_Path + tmp.Audio_File_Name, tmp.Loop, tmp.Volume);
            // Set to false to indicate it has been executed
            Container.AudioList[p_Track_Counter].Played = true;
        }
    }
    
    private void TransitAudio()
    {
        // Time based transition
        if (Time_Based_Transition)
        {
            // Will retrieve the current time elapsed from CSMGR
            // Centralize the time elapsed to CSMGR for accuracy
            if (CSMGR.TimeElapsed > Container.AudioList[p_Track_Counter].Time)
            {
                // Will tick up the tracking counter if the time determines so
                ++p_Track_Counter;
            }
        }
    }

    private void FadeAudio()
    {
        tmpTimeElapsed = CSMGR.TimeElapsed;

        // Loop through the array of audio to determine which audio is to be faded out
		// For Version
		for (int i = 0; i < Container.TotalSizeAudio(); i++)
		{
			// Check if it is time to fade
			if (Container.AudioList[i].FadeTime < tmpTimeElapsed)
			{
				// Add the name of the file to the list of audio to be faded
				// Flagged audio list
				p_Fading_Audio.Add(Container.AudioList[i].Audio_File_Name);
			}
		}

        // Fade Audio that are flagged
        tmpArray = FindObjectsOfType<AudioSource>() as AudioSource[];
		tmpStringArray = new string[tmpArray.Length];

		// For Version
		for (int i = 0; i < tmpArray.Length; i++)
		{
			tmpStringArray[i] = tmpArray[i].name;

			for (int j = 0; j < p_Fading_Audio.Count; j++)
			{
				// Fade audio volume if it matches
				if (p_Fading_Audio[j] == tmpStringArray[i])
				{
					// Track the fade tick timer
					p_Fade_Ticks += Time.deltaTime;

					if (p_Fade_Ticks > AUDIO_FADE_SPEED_TICK)
					{
						p_Fade_Ticks = 0.0f;

						tmpArray[i].volume = tmpArray[i].volume * AUDIO_FADE_SPEED;

						// Check if the volume is low enough to be destroyed
						if (tmpArray[i].volume < AUDIO_FADE_REMOVAL_THRESHOLD)
						{
							tmpArray[i].Stop();
							Destroy(tmpArray[i].gameObject, 0.1f);
						}

						break;
					}
				}
			}
		}
	}

    // Will run the program only if it is not over or at the limit
    private bool CheckLimit()
    {
        if (p_Track_Counter >= p_Track_Limit)
        {
            return true;
        }
        return false;
    }

    public static void FadeSFX()
    {
        // Stop all SFX
        AudioSource[] tmpArray;
        tmpArray = FindObjectsOfType<AudioSource>() as AudioSource[];

		// For Version
		for (int i = 0; i < tmpArray.Length; i++)
		{
			tmpArray[i].volume *= 0.8f;
		}
    }

    private void DestroyLoopedSFX()
    {
		// For Version
		for (int i = 0; i < Container.TotalSizeAudio(); i++)
		{
			if (Container.AudioList[i].Loop && (Container.AudioList[i].FadeTime + CSMGR.TimeToFade) < CSMGR.TimeElapsed)
			{
				ToBeDestroyed = Container.AudioList[i].Audio_File_Name;
			}
		}

		GameObject tmpObj = GameObject.Find(ToBeDestroyed);

        if (tmpObj)
            Destroy(tmpObj);
    }
}

using System.Collections;
using System.Collections.Generic;
using Core.Singleton;
using Melanchall.DryWetMidi.Core;
using UnityEngine;
using SO_Scripts;
using System.IO;
using UnityEngine.Networking;

namespace Managers
{
    public class SongManager : Singleton<SongManager>
    {
        [SerializeField] private MidiData midiData;
        public AudioSource audioSource; //audio source to play the song
        public static MidiFile MidiFile;//static ref to midi file, this is where it will load on run
        
        private void Start()
        {
            /*
            checking if the "StreamingAssets" path is a website or not, depending on the platform that loads the midi file
                    for example, windows, mac, linux = file location where as webgl = website
            if not look in local folder
            */ 
            if (Application.streamingAssetsPath.StartsWith("http://") || Application.streamingAssetsPath.StartsWith("https://")){
                StartCoroutine(ReadFromWebsite());//start coroutine to wait to load
            }else{
                ReadFromFile();
            }
        }

        /// <summary>
        /// Read Midi file from website, auto request through coroutine.
        /// </summary>
        private IEnumerator ReadFromWebsite()
        {
            //requesting unity web request the midi file
            using (UnityWebRequest www = UnityWebRequest.Get(Application.streamingAssetsPath + "/" + midiData.fileLocation)){
                yield return www.SendWebRequest();
                
                //checking to see if there's any network errors
    #pragma warning disable CS0618
                if(www.isNetworkError || www.isHttpError)
    #pragma warning restore CS0618
                {
                    Debug.LogError(www.error);
                }
                //if no error, load result from data
                //then send these results to memory stream
                //then load the stream onto midi file
                else
                {
                    byte[] results = www.downloadHandler.data;
                    using (var stream = new MemoryStream(results))
                    {
                        MidiFile = MidiFile.Read(stream);
                        LaneManager.Instance.CompileDataFromMidi(MidiFile);
                    }
                }
            }
        }
        
        /// <summary>
        /// Read Midi file from local file dir.
        /// </summary>
        private void ReadFromFile()
        {
            MidiFile = MidiFile.Read(Application.streamingAssetsPath + "/" + midiData.fileLocation);
            //convert midi data to necessary data for rhythm game, append them all to list
            LaneManager.Instance.CompileDataFromMidi(MidiFile);
            Invoke(nameof(StartSong), midiData.songDelayInSeconds);
        }
        
        public void StartSong()
        {
            audioSource.Play();
        }
    
        ///<summary>
        ///This is an utility function to return the audio source time
        ///Instead of using "AudioSource.time" we returning a double of playback pos in PCM sample divided by freq (Hz) for the accuracy 
        ///</summary>
        public static double GetAudioSourceTime()
        {
            return (double)Instance.audioSource.timeSamples / Instance.audioSource.clip.frequency;
        }

        /// <summary>
        /// Get Current AudioTime with inputDelay compensated
        /// </summary>
        /// <returns></returns>
        public double GetCurrentAudioTime()
        {
            return (double) (GetAudioSourceTime() - (midiData.inputDelayInMilliseconds / 1000.0));
        }
            
            
            
    }
}

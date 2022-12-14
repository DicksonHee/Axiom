//--------------------------------------------------------------------
//
// This is a Unity behaviour script that demonstrates how to use
// Programmer Sounds and the Audio Table in your game code. 
//
// Programmer sounds allows the game code to receive a callback at a
// sound-designer specified time and return a sound object to the be
// played within the event.
//
// The audio table is a group of audio files compressed in a Bank that
// are not associated with any event and can be accessed by a string key.
//
// Together these two features allow for an efficient implementation of
// dialogue systems where the sound designer can build a single template 
// event and different dialogue sounds can be played through it at runtime.
//
// This script will play one of three pieces of dialog through an event
// on a key press from the player.
//
// This document assumes familiarity with Unity scripting. See
// https://unity3d.com/learn/tutorials/topics/scripting for resources
// on learning Unity scripting. 
//
// For information on using FMOD example code in your own programs, visit
// https://www.fmod.com/legal
//
//--------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Yarn.Unity;
using FMODUnity;
using FMOD.Studio;
using FMOD;
using Axiom.Core;
using UnityEngine.SceneManagement;

namespace Axiom.Dialogue
{
    class ProgrammerSounds : MonoBehaviour
    {
        public static ProgrammerSounds current;

        EVENT_CALLBACK dialogueCallback;
        private PLAYBACK_STATE state;
        public EventInstance dialogueInstance;
        public FMODUnity.EventReference eventName;
        [SerializeField] static FMOD.Sound dialogueSound;
        public uint dialogueLength;
        private Camera mainCam;

#if UNITY_EDITOR
        void Reset()
        {
            eventName = FMODUnity.EventReference.Find("event:/Character/Radio/Command");
        }
#endif

        void Start()
        {
            if (current != null && current != this) Destroy(this);
            else current = this;

            // Explicitly create the delegate object and assign it to a member so it doesn't get freed
            // by the garbage collected while it's being used
            dialogueCallback = new EVENT_CALLBACK(DialogueEventCallback);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            StopDialog(true);
        }

        [YarnCommand("play")]
        public void PlayDialog(string key, bool _overrideAttentuation, float volume = 1f, Transform audiopos = null, float _minDistance = 0, float _maxDistance = 0)
        {
            UnityEngine.Debug.Log(key);
            dialogueInstance = RuntimeManager.CreateInstance(eventName);
            //possible fix to error
            dialogueInstance.set3DAttributes(RuntimeUtils.To3DAttributes(mainCam.transform));

            if (audiopos == null)
            {
                RuntimeManager.AttachInstanceToGameObject(dialogueInstance, mainCam.transform);
            }
            else
            {
                RuntimeManager.AttachInstanceToGameObject(dialogueInstance, audiopos);
            }

            dialogueInstance.setVolume(volume);

            //Attenuation stuff
            if(_overrideAttentuation)
            {
                dialogueInstance.setProperty(FMOD.Studio.EVENT_PROPERTY.MINIMUM_DISTANCE, _minDistance);
                dialogueInstance.setProperty(FMOD.Studio.EVENT_PROPERTY.MAXIMUM_DISTANCE, _maxDistance);
            }

            // Pin the key string in memory and pass a pointer through the user data
            GCHandle stringHandle = GCHandle.Alloc(key);
            dialogueInstance.setUserData(GCHandle.ToIntPtr(stringHandle));

            dialogueInstance.setCallback(dialogueCallback);

            // need to get length before playing audio, otherwise for loop will fire multiple times due to wait time being 0
            // Idk what im doing, but I know this is getting the length before playing the audio.
            // half of it is repeated code under DialogueEventCallback
            // not sure if this made some code redundant or will cause issues later on
            MODE soundMode = MODE.CREATESTREAM;
            FMOD.Studio.SOUND_INFO dialogueSoundInfo;
            var keyResult = FMODUnity.RuntimeManager.StudioSystem.getSoundInfo(key, out dialogueSoundInfo);
            //FMOD.Sound dialogueSound;

            //attempt to get length
            var soundResult = RuntimeManager.CoreSystem.createSound(dialogueSoundInfo.name_or_data,
                soundMode | dialogueSoundInfo.mode, ref dialogueSoundInfo.exinfo, out dialogueSound);
            FMOD.Sound subSound;
            dialogueSound.getSubSound(dialogueSoundInfo.subsoundindex, out subSound);
            // uint length = 0; 
            subSound.getLength(out dialogueLength, FMOD.TIMEUNIT.MS);
            //end attempt get sound length
            float delay = dialogueLength / 1000;

            dialogueInstance.start();
            //UnityEngine.Debug.Log("playD");
            //Invoke(nameof(RealseInstance), delay);
            dialogueInstance.release();
        }

        void RealseInstance()
        {
            dialogueInstance.release();
        }

        //for stopping static or dialog
        public void StopDialog(bool fadeOut)
        {
            if (!fadeOut)
            {
                dialogueInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                dialogueInstance.release();

            }
            else
            {
                dialogueInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                dialogueInstance.release();
            }
        }

        public void SetPauseState(bool pause)
        {
            dialogueInstance.setPaused(pause);
        }


        [AOT.MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
        static RESULT DialogueEventCallback(EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
        {
            EventInstance instance = new EventInstance(instancePtr);

            // Retrieve the user data
            IntPtr stringPtr;
            instance.getUserData(out stringPtr);

            // Get the string object
            GCHandle stringHandle = GCHandle.FromIntPtr(stringPtr);
            String key = stringHandle.Target as String;

            switch (type)
            {
                case EVENT_CALLBACK_TYPE.CREATE_PROGRAMMER_SOUND:
                {
                    //FMOD.MODE soundMode = FMOD.MODE.LOOP_NORMAL | FMOD.MODE.CREATECOMPRESSEDSAMPLE | FMOD.MODE.NONBLOCKING;

                    //Loading delay error possible fix
                    MODE soundMode = MODE.CREATESTREAM;
                    var parameter =
                        (PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr,
                            typeof(PROGRAMMER_SOUND_PROPERTIES));

                    if (key.Contains("."))
                    {
                        //FMOD.Sound dialogueSound;
                        var soundResult =
                            RuntimeManager.CoreSystem.createSound(Application.streamingAssetsPath + "/" + key,
                                soundMode, out dialogueSound);

                        if (soundResult == FMOD.RESULT.OK)
                        {
                            parameter.sound = dialogueSound.handle;
                            parameter.subsoundIndex = -1;
                            Marshal.StructureToPtr(parameter, parameterPtr, false);
                        }
                    }
                    else
                    {
                        FMOD.Studio.SOUND_INFO dialogueSoundInfo;
                        var keyResult = FMODUnity.RuntimeManager.StudioSystem.getSoundInfo(key, out dialogueSoundInfo);
                        if (keyResult != FMOD.RESULT.OK)
                        {
                            break;
                        }
                        //FMOD.Sound dialogueSound;

                        var soundResult = RuntimeManager.CoreSystem.createSound(dialogueSoundInfo.name_or_data,
                            soundMode | dialogueSoundInfo.mode, ref dialogueSoundInfo.exinfo, out dialogueSound);

                        if (soundResult == FMOD.RESULT.OK)
                        {
                            parameter.sound = dialogueSound.handle;
                            parameter.subsoundIndex = dialogueSoundInfo.subsoundindex;
                            Marshal.StructureToPtr(parameter, parameterPtr, false);
                        }
                    }

                    break;
                }
                case FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROY_PROGRAMMER_SOUND:
                {
                    var parameter =
                        (PROGRAMMER_SOUND_PROPERTIES)Marshal.PtrToStructure(parameterPtr,
                            typeof(PROGRAMMER_SOUND_PROPERTIES));
                    var sound = new Sound(parameter.sound);
                    sound.release();


                    break;
                }
                case FMOD.Studio.EVENT_CALLBACK_TYPE.DESTROYED:
                {
                    // Now the event has been destroyed, unpin the string memory so it can be garbage collected
                    stringHandle.Free();

                    break;
                }
            }

            return FMOD.RESULT.OK;
        }
    }
}
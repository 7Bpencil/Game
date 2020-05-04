using System;
using System.Collections.Generic;
using App.Engine.Physics;
using FMOD;
using FMOD.Studio;

namespace App.Engine.Audio
{
    public static class AudioEngine
    {
        private static FMOD.Studio.System system;
        public static bool Ready;
        private static ATTRIBUTES_3D attributes3d = new ATTRIBUTES_3D();
        private static FMOD.Studio.Bank masterBank;
        private static Dictionary<string, EventDescription> cachedEventDescriptions;


        /// <summary>
        /// Creates and plays instance that has position in world
        /// </summary>
        /// <param name="path"></param>
        /// <param name="instancePosition"></param>
        /// <param name="cameraPosition"></param>
        public static void PlayNewInstance(string path, Vector instancePosition, Vector cameraPosition)
        {
            PlayInstance(CreateEventInstance(path, instancePosition, cameraPosition));
        }
        
        /// <summary>
        /// Creates and plays instance without position - position doesn't affect sound
        /// </summary>
        /// <param name="path"></param>
        public static void PlayNewInstance(string path)
        {
            PlayInstance(CreateEventInstance(path));
        }
        
        /// <summary>
        /// Creates instance that has position in world
        /// </summary>
        /// <param name="path"></param>
        /// <param name="instancePosition"></param>
        /// <param name="cameraPosition"></param>
        /// <returns></returns>
        public static FMOD.Studio.EventInstance CreateEventInstance(string path, Vector instancePosition, Vector cameraPosition)
        {
            var eventDescription = GetEventDescription(path);
            eventDescription.createInstance(out var instance);
            eventDescription.is3D(out var is3D);
            if (instancePosition != null && is3D) PositionEvent(instance, instancePosition, cameraPosition);
            return instance;
        }
        
        /// <summary>
        /// Creates instance without position - position doesn't affect sound
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static FMOD.Studio.EventInstance CreateEventInstance(string path)
        {
            var eventDescription = GetEventDescription(path);
            eventDescription.createInstance(out var instance);
            return instance;
        }

        public static void PlayInstance(FMOD.Studio.EventInstance instance)
        {
            instance.start();
            instance.release();
        }
        
        public static void PositionEvent(EventInstance instance, Vector position, Vector cameraPosition)
        {
            
        }
        
        public static FMOD.Studio.EventDescription GetEventDescription(string path)
        {
            if (cachedEventDescriptions.ContainsKey(path))
                return cachedEventDescriptions[path];
            var result = system.getEvent(path, out var description);
            if (result == RESULT.OK) description.loadSampleData();
            else throw new Exception("FMOD getEvent failed: " + result);
            
            cachedEventDescriptions.Add(path, description);
            return description;
        }

        public static void Initialize()
        {
            CheckResult(FMOD.Studio.System.create(out system));
            CheckResult(system.initialize(512, FMOD.Studio.INITFLAGS.NORMAL, FMOD.INITFLAGS.NORMAL, IntPtr.Zero));

            cachedEventDescriptions = new Dictionary<string, EventDescription>();
            system.loadBankFile(@"Assets\Audio\Desktop\Master.bank", LOAD_BANK_FLAGS.NORMAL, out masterBank);
            system.loadBankFile(@"Assets\Audio\Desktop\Master.strings.bank", LOAD_BANK_FLAGS.NORMAL, out var masterBankStrings);
            
            while (true)
            {
                masterBank.getLoadingState(out var loadingState);
                masterBankStrings.getLoadingState(out var stringsState);
                if (loadingState == LOADING_STATE.LOADED && stringsState == LOADING_STATE.LOADED)
                {
                    masterBank.loadSampleData();
                    break;
                }
            }

            Ready = true;
        }

        private static void CheckResult(RESULT result)
        {
            if (result != RESULT.OK)
            {
                throw new Exception("FMOD Failed: " + result);
            }
        }

        public static void Update()
        {
            system.update();
        }

        public static void Release()
        {
            system.release();
        }
    }
}
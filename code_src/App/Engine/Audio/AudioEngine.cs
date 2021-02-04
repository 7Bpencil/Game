using System;
using System.Collections.Generic;
using App.Engine.Physics;
using FMOD;
using FMOD.Studio;
using INITFLAGS = FMOD.INITFLAGS;

namespace App.Engine.Audio
{
    public static class AudioEngine
    {
        private static FMOD.Studio.System system;
        private static FMOD.System fmodSystem;
        private static ATTRIBUTES_3D attributes3d;
        private static Bank masterBank;
        private static Dictionary<string, EventDescription> cachedEventDescriptions;


        /// <summary>
        /// Creates and plays instance that has position in world
        /// </summary>
        /// <param name="path"></param>
        /// <param name="instancePosition"></param>
        public static void PlayNewInstance(string path, Vector instancePosition)
        {
            PlayInstance(CreateEventInstance(path, instancePosition));
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
        /// <returns></returns>
        public static EventInstance CreateEventInstance(string path, Vector instancePosition)
        {
            var eventDescription = GetEventDescription(path);
            eventDescription.createInstance(out var instance);
            eventDescription.is3D(out var is3D);
            if (is3D && instancePosition != null)
                PositionEvent(instance, instancePosition);
            else
            {
                Console.WriteLine(path + " is not 3D");
                throw new ArgumentException();
            }
            return instance;
        }
        
        /// <summary>
        /// Creates instance without position - position doesn't affect sound
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static EventInstance CreateEventInstance(string path)
        {
            var eventDescription = GetEventDescription(path);
            eventDescription.createInstance(out var instance);
            eventDescription.is3D(out var is3D);
            if (is3D)
            {
                Console.WriteLine(path + " is not 2D");
                throw new ArgumentException();
            }
            return instance;
        }

        public static void PlayInstance(EventInstance instance)
        {
            instance.start();
            instance.release();
        }
        
        public static void PositionEvent(EventInstance instance, Vector position)
        {
            var vector = new VECTOR
            {
                x = position.X,
                y = position.Y,
                z = 0,
            };
            attributes3d.position = vector;
            instance.set3DAttributes(attributes3d);
        }

        public static void UpdateListenerPosition(Vector newPosition)
        {
            var vector = new VECTOR
            {
                x = newPosition.X,
                y = newPosition.Y,
                z = 0,
            };
            
            attributes3d.position = vector;
            system.setListenerAttributes(0, attributes3d);
        }

        private static EventDescription GetEventDescription(string path)
        {
            if (cachedEventDescriptions.ContainsKey(path))
                return cachedEventDescriptions[path];
            CheckResult(system.getEvent(path, out var description));
            description.loadSampleData();
            cachedEventDescriptions.Add(path, description);
            return description;
        }

        public static void Initialize()
        {
            CheckResult(Factory.System_Create(out fmodSystem));
            fmodSystem.init(512, INITFLAGS.NORMAL, IntPtr.Zero);
            fmodSystem.set3DSettings(1, 64, 1);
            CheckResult(FMOD.Studio.System.create(out system));
            CheckResult(system.initialize(512, FMOD.Studio.INITFLAGS.NORMAL, FMOD.INITFLAGS.NORMAL, IntPtr.Zero));
            SetAttributes3D();


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

        private static void SetAttributes3D()
        {
            attributes3d = new ATTRIBUTES_3D();
            
            var vector = new VECTOR 
            {
                x = 0f,
                y = 0f,
                z = 1f
            };
            attributes3d.forward = vector;
            
            vector = new VECTOR 
            {
                x = 0f,
                y = 1f,
                z = 0f
            };
            attributes3d.up = vector;
        }
    }
}
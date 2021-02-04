using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using App.Engine.Physics;
using FMOD;
using FMOD.Studio;

namespace App.Engine.Audio
{
    public static class AudioEngine
    {
        private static FMOD.Studio.System system;
        private static Dictionary<string, EventDescription> cachedEventDescriptions;
        private static ATTRIBUTES_3D attributes3d;

        public static void Initialize()
        {
            CheckResult(FMOD.Studio.System.create(out system));
            CheckResult(system.initialize(512, FMOD.Studio.INITFLAGS.NORMAL, FMOD.INITFLAGS.NORMAL, IntPtr.Zero));
            CheckResult(system.loadBankFile(@"assets\Audio\Desktop\Master.bank", LOAD_BANK_FLAGS.NORMAL, out _));
            CheckResult(system.loadBankFile(@"assets\Audio\Desktop\Master.strings.bank", LOAD_BANK_FLAGS.NORMAL, out _));

            cachedEventDescriptions = new Dictionary<string, EventDescription>();
            attributes3d = new ATTRIBUTES_3D
            {
                forward = new VECTOR {x = 0f, y = 0f, z = 1f},
                up = new VECTOR {x = 0f, y = 1f, z = 0f}
            };
        }

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
        private static EventInstance CreateEventInstance(string path, Vector instancePosition)
        {
            var eventDescription = GetEventDescription(path);
            eventDescription.createInstance(out var instance);
            eventDescription.is3D(out var is3D);
            if (is3D) PositionEvent(ref instance, instancePosition);
            else
            {
                Logger.Log($"{path} is not 3D", MessageClass.ERROR);
                throw new ArgumentException();
            }
            return instance;
        }

        /// <summary>
        /// Creates instance without position - position doesn't affect sound
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static EventInstance CreateEventInstance(string path)
        {
            var eventDescription = GetEventDescription(path);
            eventDescription.createInstance(out var instance);
            eventDescription.is3D(out var is3D);
            if (is3D)
            {
                Logger.Log($"{path} is not 2D", MessageClass.ERROR);
                throw new ArgumentException();
            }
            return instance;
        }

        private static void PlayInstance(EventInstance instance)
        {
            instance.start();
            instance.release();
        }

        private static void PositionEvent(ref EventInstance instance, Vector position)
        {
            attributes3d.position = Convert(position);
            instance.set3DAttributes(attributes3d);
        }

        public static void UpdateListenerPosition(Vector newPosition)
        {
            attributes3d.position = Convert(newPosition);
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

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private static void CheckResult(RESULT result)
        {
            if (result != RESULT.OK)
            {
                Logger.Log($"FMOD Failed: {result}: {Error.String(result)}", MessageClass.ERROR);
                throw new Exception();
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

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        private static VECTOR Convert(Vector vector)
        {
            VECTOR v = default;
            v.x = vector.X;
            v.y = vector.Y;
            return v;
        }

    }
}

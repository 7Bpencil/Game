using System;
using System.Collections.Generic;
using App.Engine;
using App.Engine.Physics;
using App.Engine.Physics.RigidBody;

namespace App.Model
{
    public class Player
    {
        public RigidCircle Shape;
        public Sprite Torso;
        public Sprite Legs;
        
        public Vector Center => Shape.Center;
        public float Radius => Shape.Radius;
        
        private List<Weapon> weapons;
        private int currentWeaponIndex;
        public Weapon CurrentWeapon => weapons[currentWeaponIndex];
        
        public Player(RigidCircle shape, Sprite torso, Sprite legs, List<Weapon> startWeapons)
        {
            Shape = shape;
            Torso = torso;
            Legs = legs;
            currentWeaponIndex = 0;
            weapons = startWeapons;
        }

        public void MoveBy(Vector delta)
        {
            Shape.MoveBy(delta);
        }

        public void AddWeapon(Weapon newWeapon)
        {
            foreach (var weapon in weapons)
            {
                if (newWeapon.Name != weapon.Name) continue;
                weapon.AddAmmo(newWeapon.AmmoAmount);
                return;
            }
            weapons.Add(newWeapon);
        }
        
        public void RemoveWeapon(string weaponName)
        {
            throw new NotImplementedException();
        }

        public void MoveNextWeapon()
        {
            currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Count;
        }
        
        public void MovePreviousWeapon()
        {
            currentWeaponIndex = (weapons.Count + currentWeaponIndex - 1) % weapons.Count;
        }

        public void IncrementWeaponsTick()
        {
            foreach (var weapon in weapons)
                weapon.IncrementTick();
        }
    }
}
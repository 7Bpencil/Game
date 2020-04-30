using System;
using System.Collections.Generic;
using App.Engine;
using App.Engine.Physics;
using App.Engine.Physics.RigidShapes;

namespace App.Model.Entities
{
    public class Player
    {
        public RigidCircle Shape;
        public Vector Position => Shape.Center;
        public float Radius => Shape.Radius;
        
        public SpriteContainer TorsoContainer;
        public SpriteContainer LegsContainer;

        private List<Weapon> weapons;
        private int currentWeaponIndex;
        public Weapon CurrentWeapon => weapons[currentWeaponIndex];
        
        public Player(RigidCircle shape, Sprite torso, Sprite legs, List<Weapon> startWeapons)
        {
            Shape = shape;
            TorsoContainer = new SpriteContainer(torso);
            LegsContainer = new SpriteContainer(legs);
            weapons = startWeapons;
            currentWeaponIndex = 0;
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
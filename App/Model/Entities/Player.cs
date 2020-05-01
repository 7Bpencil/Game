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
        
        public readonly SpriteContainer TorsoContainer;
        public readonly SpriteContainer LegsContainer;
        private readonly Dictionary<string, Sprite> weaponSprites;

        private List<Weapon> weapons;
        private int currentWeaponIndex;
        public Weapon CurrentWeapon => weapons[currentWeaponIndex];
        
        
        public Player(Vector startPosition, float startAngle, Sprite legs,
            List<Weapon> startWeapons, Dictionary<string, Sprite> weaponSprites)
        {
            this.weaponSprites = weaponSprites;
            weapons = startWeapons;
            currentWeaponIndex = 0;
            
            Shape = new RigidCircle(startPosition, 32, false, true);
            LegsContainer = new SpriteContainer(legs, startPosition, startAngle);
            TorsoContainer = new SpriteContainer(weaponSprites[CurrentWeapon.Name], startPosition, startAngle);
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
            TorsoContainer.Content = weaponSprites[CurrentWeapon.Name];
        }
        
        public void MovePreviousWeapon()
        {
            currentWeaponIndex = (weapons.Count + currentWeaponIndex - 1) % weapons.Count;
            TorsoContainer.Content = weaponSprites[CurrentWeapon.Name];
        }

        public void IncrementWeaponsTick()
        {
            foreach (var weapon in weapons)
                weapon.IncrementTick();
        }
    }
}
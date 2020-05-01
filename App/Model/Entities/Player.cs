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
        
        
        public Player(RigidCircle shape, Sprite legs, List<Weapon> startWeapons,
            Dictionary<string, Sprite> weaponSprites)
        {
            this.weaponSprites = weaponSprites;
            weapons = startWeapons;
            currentWeaponIndex = 0;
            
            Shape = shape;
            LegsContainer = new SpriteContainer(legs);
            TorsoContainer = new SpriteContainer(weaponSprites[CurrentWeapon.Name]);
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
            CorrectTorso();
        }
        
        public void MovePreviousWeapon()
        {
            currentWeaponIndex = (weapons.Count + currentWeaponIndex - 1) % weapons.Count;
            CorrectTorso();
        }

        public void IncrementWeaponsTick()
        {
            foreach (var weapon in weapons)
                weapon.IncrementTick();
        }

        private void CorrectTorso()
        {
            var currentAngle = TorsoContainer.Content.Angle;
            TorsoContainer.Content = weaponSprites[CurrentWeapon.Name];
            TorsoContainer.Content.Angle = currentAngle;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using App.Engine;
using App.Engine.Physics;
using App.Engine.Physics.RigidShapes;
using App.Engine.Sprites;

namespace App.Model.Entities
{
    public class Player
    {
        public readonly RigidCircle Shape;
        public Vector Position => Shape.Center;
        public float Radius => Shape.Radius;
        
        public readonly SpriteContainer TorsoContainer;
        public readonly SpriteContainer LegsContainer;
        private readonly Dictionary<Type, Sprite> weaponSprites;
        private readonly MeleeWeaponSprite meleeWeaponSprite;

        private readonly List<Weapon> weapons;
        public readonly MeleeWeapon MeleeWeapon;
        private int currentWeaponIndex;
        public Weapon CurrentWeapon => weapons[currentWeaponIndex];

        public Player(Vector startPosition, float startAngle, Sprite legs,
            List<Weapon> startWeapons, Dictionary<Type, Sprite> weaponSprites)
        {
            MeleeWeapon = new MeleeWeapon(startPosition, startAngle);
            this.weaponSprites = weaponSprites;
            meleeWeaponSprite = 
                new MeleeWeaponSprite(new Bitmap(@"Assets\Sprites\Weapons\pinkHair_katana.png"),
                    1, 0, 5, new Size(170, 170));
            weapons = startWeapons;
            currentWeaponIndex = 0;
            
            Shape = new RigidCircle(startPosition, 32, false, true);
            LegsContainer = new SpriteContainer(legs, startPosition, startAngle);
            TorsoContainer = new SpriteContainer(weaponSprites[CurrentWeapon.GetType()], startPosition, startAngle);
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
            TorsoContainer.Content = weaponSprites[CurrentWeapon.GetType()];
        }
        
        public void MovePreviousWeapon()
        {
            currentWeaponIndex = (weapons.Count + currentWeaponIndex - 1) % weapons.Count;
            TorsoContainer.Content = weaponSprites[CurrentWeapon.GetType()];
        }

        public void TakeMeleeWeapon()
        {
            TorsoContainer.Content = meleeWeaponSprite;
            meleeWeaponSprite.inAction = true;
        }

        public void HideMeleeWeapon()
        {
            TorsoContainer.Content = weaponSprites[CurrentWeapon.GetType()];
        }

        public void IncrementWeaponsTick()
        {
            MeleeWeapon.IncrementTick();
            foreach (var weapon in weapons)
                weapon.IncrementTick();
        }
    }
}
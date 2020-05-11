using System;
using System.Collections.Generic;
using App.Engine;
using App.Engine.Physics.RigidShapes;
using App.Engine.Sprites;
using App.Model.Entities.Weapons;

namespace App.Model.Entities
{
    public class Player : LivingEntity
    {
        public float Radius => CollisionShape.Radius;
        private readonly Dictionary<Type, Sprite> weaponSprites;
        private readonly MeleeWeaponSprite meleeWeaponSprite;
        private readonly List<Weapon> weapons;
        public readonly MeleeWeapon MeleeWeapon;
        private int currentWeaponIndex;
        public Weapon CurrentWeapon => weapons[currentWeaponIndex];

        public Player(int health, int armor, RigidCircle collisionShape, 
            SpriteContainer legsContainer, SpriteContainer torsoContainer,
            List<Weapon> startWeapons, Dictionary<Type, Sprite> weaponSprites, 
            MeleeWeaponSprite meleeWeaponSprite, MeleeWeapon meleeWeapon, string deadBodyPath) 
            : base(health, armor, collisionShape, legsContainer, torsoContainer, deadBodyPath)
        {
            MeleeWeapon = meleeWeapon;
            this.meleeWeaponSprite = meleeWeaponSprite;
            weapons = startWeapons;
            this.weaponSprites = weaponSprites;
            currentWeaponIndex = 0;
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

        public void RaiseMeleeWeapon()
        {
            TorsoContainer.Content = meleeWeaponSprite;
            meleeWeaponSprite.InAction = true;
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

        public bool WasMeleeWeaponRaised => meleeWeaponSprite.WasRaised;
    }
}
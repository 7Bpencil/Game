using System;
using System.Collections.Generic;
using App.Engine;
using App.Engine.Physics;
using App.Engine.Physics.RigidShapes;
using App.Engine.Sprites;
using App.Model.Entities.Weapons;

namespace App.Model.Entities
{
    public class Player
    {
        public int Health;
        public int Armor;
        public bool IsDead;
        public readonly RigidCircle CollisionShape;
        public Vector Position => CollisionShape.Center;
        public float Radius => CollisionShape.Radius;
        
        public readonly SpriteContainer TorsoContainer;
        public readonly SpriteContainer LegsContainer;
        private readonly Dictionary<Type, Sprite> weaponSprites;
        private readonly MeleeWeaponSprite meleeWeaponSprite;
        private readonly List<Weapon> weapons;
        public readonly MeleeWeapon MeleeWeapon;
        private int currentWeaponIndex;
        public Weapon CurrentWeapon => weapons[currentWeaponIndex];
        public Vector Velocity;

        public Player(int health, int armor, Vector startPosition, float startAngle, Sprite legs, RigidCircle collisionShape,
            List<Weapon> startWeapons, Dictionary<Type, Sprite> weaponSprites, MeleeWeaponSprite meleeWeapon)
        {
            Health = health;
            Armor = armor;
            MeleeWeapon = new MeleeWeapon(startPosition, startAngle);
            this.weaponSprites = weaponSprites;
            meleeWeaponSprite = meleeWeapon;
            weapons = startWeapons;
            currentWeaponIndex = 0;
            CollisionShape = collisionShape;
            LegsContainer = new SpriteContainer(legs, startPosition, startAngle);
            TorsoContainer = new SpriteContainer(weaponSprites[CurrentWeapon.GetType()], startPosition, startAngle);
            Velocity = Vector.ZeroVector;
        }

        public void MoveBy(Vector delta)
        {
            CollisionShape.MoveBy(delta);
        }
        
        public void MoveTo(Vector newPosition)
        {
            CollisionShape.MoveTo(newPosition);
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
        
        public void TakeHit(int damage)
        {
            if (IsDead) return;
            Armor -= damage;
            if (Armor < 0)
            {
                Health += Armor;
                Armor = 0;
            }

            if (Health <= 0) IsDead = true;
        }
    }
}
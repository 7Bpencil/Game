using System;
using System.Collections.Generic;
using App.Engine;
using App.Engine.Physics;
using App.Engine.Physics.Collision;
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

        private const int RegularSpeed = 8;
        private const int DashSpeed = 24;

        private int dashCount;
        private const int DashPeriod = 2;
        private const int TicksBetweenTwoDashes = 5;
        private const int TicksBetweenDashes = 28;
        private int ticksFromLastDash;

        private bool IsInDash => ticksFromLastDash <= DashPeriod;
        private bool CanDash
            => dashCount == 1 && ticksFromLastDash > TicksBetweenTwoDashes
               || dashCount == 2 && ticksFromLastDash > TicksBetweenDashes;


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
            ticksFromLastDash = TicksBetweenDashes + 1;
            dashCount = 2;
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
            MeleeWeapon.ResetTick();
        }

        public void HideMeleeWeapon()
        {
            TorsoContainer.Content = weaponSprites[CurrentWeapon.GetType()];
        }

        public void IncrementTick()
        {
            MeleeWeapon.IncrementTick();
            foreach (var weapon in weapons)
                weapon.IncrementTick();

            ticksFromLastDash++;
        }

        public bool WasMeleeWeaponRaised => meleeWeaponSprite.WasRaised;

        public bool IsMeleeWeaponInAction => meleeWeaponSprite.InAction;

        public override Type GetWeaponType()
        {
            return CurrentWeapon.GetType();
        }

        public void UpdatePosition(Core.KeyStates keyState, List<RigidShape> sceneStaticShapes)
        {
            if (keyState.Shift && !IsInDash && CanDash)
                Dash(keyState, sceneStaticShapes);
            else if (!IsInDash)
                CreateVelocity(keyState, RegularSpeed);

            MoveBy(Velocity);
        }

        private void Dash(Core.KeyStates keyState, List<RigidShape> sceneStaticShapes)
        {
            CreateVelocity(keyState, DashSpeed);
            if (Equals(Velocity, Vector.ZeroVector)) return;

            var firstPosition = Position + Velocity.Normalize() * (Radius - 1);
            var closestPenetrationTime = float.PositiveInfinity;

            foreach (var shape in sceneStaticShapes)
            {
                var penetrationTimes = DynamicCollisionDetector.AreCollideWithStatic(firstPosition, Velocity, shape);
                if (penetrationTimes == null) continue;
                if (penetrationTimes[0] < 1 && penetrationTimes[0] < closestPenetrationTime && penetrationTimes[0] > 0)
                    closestPenetrationTime = penetrationTimes[0];
            }

            if (closestPenetrationTime < 1)
                Velocity *= closestPenetrationTime;

            ticksFromLastDash = 0;
            dashCount++;
            if (dashCount == 3) dashCount = 1;
        }

        private void CreateVelocity(Core.KeyStates keyState, int speed)
        {
            var delta = Vector.ZeroVector;

            if (keyState.W)
                delta.Y -= speed;
            if (keyState.S)
                delta.Y += speed;
            if (keyState.A)
                delta.X -= speed;
            if (keyState.D)
                delta.X += speed;

            Velocity = delta.Normalize() * speed;
        }

        public void UpdateSprites(Vector cursorPosition)
        {
            RotateLegs(Velocity);
            RotateTorso(cursorPosition);
        }

        private void RotateLegs(Vector delta)
        {
            if (delta.Equals(Vector.ZeroVector)) return;
            var dirAngle = Math.Atan2(-delta.Y, delta.X);
            var angle = 180 / Math.PI * dirAngle;
            LegsContainer.Angle = (float) angle;
        }

        private void RotateTorso(Vector cursorPosition)
        {
            var direction = cursorPosition - Position;
            var dirAngle = direction.Angle;
            var angle = (float) (180 / Math.PI * dirAngle);
            TorsoContainer.Angle = angle;
            MeleeWeapon.RotateRangeTo(angle, Position);
        }
    }
}

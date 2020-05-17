﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using App.Engine.Audio;
using App.Engine.Physics;
using App.Engine.Physics.Collision;
using App.Engine.Render;
using App.Model;
using App.Model.Entities;
using App.Model.Entities.Weapons;
using App.Model.Factories;
using App.Model.LevelData;
using App.View;

namespace App.Engine
{
    public class Core
    {
        private readonly Random r;
        private readonly ViewForm viewForm;
        private Size screenSize;

        private KeyStates keyState;
        private MouseState mouseState;
        private CustomCursor cursor;
        private Camera camera;
        
        private Level currentLevel;
        private Player player;
        private int livingBotsAmount;

        public class KeyStates
        {
            public bool W, S, A, D, I, R, P, Shift;
            public int pressesOnIAmount;
            public int pressesOnPAmount;
        }

        private class MouseState
        {
            public bool LMB, RMB;
        }
        
        public Core(ViewForm viewForm, Size screenSize)
        {
            r = new Random();
            this.viewForm = viewForm;
            this.screenSize = screenSize;
            InitializeSystems();
            currentLevel = LevelManager.MoveNextLevel();
            InitState();

            //AudioEngine.PlayNewInstance(@"event:/themes/THEME");
        }
        
        private void InitializeSystems()
        {
            RenderMachine.Initialize(viewForm, screenSize);
            AudioEngine.Initialize();
            LevelManager.Initialize();
            ParticleFactory.Initialize();
            AbstractWeaponFactory.Initialize();
            BotBank.Initialize();
        }
        
        private void InitState()
        {
            AStarSearch.SetMesh(currentLevel.NavMesh);
            player = currentLevel.Player;
            camera = new Camera(player.Position, player.Radius, screenSize);
            cursor = new CustomCursor(player.Position.Copy());
            currentLevel.Sprites.Add(cursor.SpriteContainer);
            keyState = new KeyStates();
            mouseState = new MouseState();
            livingBotsAmount = currentLevel.Bots.Count;
        }
        
        private void ResetState()
        {
            currentLevel.Reset();
            player = currentLevel.Player;
            cursor.MoveTo(player.Position);
            currentLevel.Sprites.Add(cursor.SpriteContainer);
            camera.Reset(player.Position);
            keyState = new KeyStates();
            mouseState = new MouseState();
        }
        
        public void GameLoop(object sender, EventArgs args)
        {
            if (currentLevel.WavesAmount == 0)
            {
                currentLevel.IsCompleted = true;
                currentLevel.Particles.Add(ParticleFactory.CreateExit(currentLevel.Exit.Center));
            }
            if (player.IsDead)
            {
                ResetState();
            }
            if (currentLevel.IsCompleted 
                && CollisionDetector.GetCollisionInfo(player.CollisionShape, currentLevel.Exit) != null)
            {
                currentLevel = LevelManager.MoveNextLevel();
                InitState();
            }

            UpdateState();

            var renderRaytracing = keyState.pressesOnPAmount % 2 == 1;
            var renderDebug = keyState.pressesOnIAmount % 2 == 1; 
            RenderPipeline.Render(currentLevel, camera, cursor.Position, renderRaytracing, renderDebug);

            AudioEngine.Update();
        }
        
        private void UpdateState()
        {
            UpdateEntities();
            
            camera.UpdateCamera(player.Position, player.Velocity, cursor.Position);
            viewForm.CursorReset();

            foreach (var spriteContainer in currentLevel.Sprites)
                if (!spriteContainer.IsEmpty) spriteContainer.Content.UpdateFrame();
            foreach (var unit in currentLevel.Particles)
                if (!unit.IsExpired) unit.UpdateFrame();
        }
        
        private void UpdateEntities()
        {
            UpdatePlayer();
            UpdateBullets();
            UpdateBots();
            UpdateCollectables();
        }

        private void UpdatePlayer()
        {
            var previousPosition = player.Position.Copy();
            
            player.UpdatePosition(keyState, currentLevel.StaticShapes);
            currentLevel.CollisionsInfo = CollisionSolver.ResolveCollisions(currentLevel.SceneShapes);
            AudioEngine.UpdateListenerPosition(player.Position);
            var realVelocity = player.Position - previousPosition;
            player.MeleeWeapon.MoveRangeBy(realVelocity);
            cursor.MoveBy(viewForm.GetCursorDiff() + realVelocity);
            player.UpdateSprites(cursor.Position);

            if (mouseState.RMB && player.MeleeWeapon.IsReady)
            {
                player.RaiseMeleeWeapon();
            }
            else if (mouseState.LMB && player.CurrentWeapon.IsReady && !player.IsMeleeWeaponInAction)
            {
                player.HideMeleeWeapon();
                var firedProjectiles = player.CurrentWeapon.Fire(player.Position, cursor);
                AudioEngine.PlayNewInstance("event:/gunfire/2D/misc/DROPPED_SHELL");
                currentLevel.Projectiles.AddRange(firedProjectiles);
                currentLevel.Particles.Add(ParticleFactory.CreateShell(player.Position, cursor.Position - player.Position, player.CurrentWeapon));
                foreach (var projectile in firedProjectiles)
                    if (projectile is Grenade grenade) currentLevel.Particles.Add(grenade.GetWarheadParticle());
            }
            
            player.IncrementTick();
        }

        private void UpdateCollectables()
        {
            var collectables = currentLevel.Collectables;
            foreach (var collectable in collectables)
            {
                if (collectable.IsPicked) continue;
                var collision = CollisionDetector.GetCollisionInfo(player.CollisionShape, collectable.CollisionShape);
                if (collision == null) continue;
                collectable.Pick(player);
            }
        }

        private void UpdateBullets()
        {
            var projectiles = currentLevel.Projectiles;
            var particles = currentLevel.Particles;
            foreach (var projectile in projectiles)
            {
                if (projectile.IsStuck) continue;
                if (projectile.StaticPenetrations.Count == 0)
                    CalculateStaticPenetrations(projectile);
                CalculateDynamicPenetrations(projectile);
                projectile.Update();
                if (projectile.ClosestPenetrationPoint != null)
                    HandleProjectileStaticStuck(projectile, particles);
            }
        }

        private void HandleProjectileStaticStuck(AbstractProjectile projectile, List<AbstractParticleUnit> levelParticles)
        {
            if (projectile is Grenade grenade) HandleGrenadeStaticStuck(grenade, levelParticles);
            else if (projectile is Bullet bullet) HandleBulletStaticStuck(bullet, levelParticles);
        }

        private void HandleBulletStaticStuck(Bullet bullet, List<AbstractParticleUnit> levelParticles)
        {
            levelParticles.Add(ParticleFactory.CreateWallDust(bullet.ClosestPenetrationPoint, bullet.Velocity));
            bullet.ClosestPenetrationPoint = null;
        }

        private void HandleGrenadeStaticStuck(Grenade grenade, List<AbstractParticleUnit> levelParticles)
        {
            HandleGrenadeExplosion(grenade, levelParticles, 0);
            grenade.ClosestPenetrationPoint = null;
        }

        private void CalculateStaticPenetrations(AbstractProjectile projectile)
        {
            var staticShapes = currentLevel.StaticShapes;
            foreach (var obstacle in staticShapes)
            {
                var penetrationTime = DynamicCollisionDetector.AreCollideWithStatic(projectile, obstacle);
                if (penetrationTime == null) continue;
                var distanceToPenetrations = new float[2];
                for (var i = 0; i < 2; i++)
                    distanceToPenetrations[i] = projectile.Speed * penetrationTime[i];
                projectile.StaticPenetrations.Add(distanceToPenetrations);
            }
            projectile.CalculateTrajectory();
        }

        private void CalculateDynamicPenetrations(AbstractProjectile projectile)
        {
            var bots = currentLevel.Bots;
            var particles = currentLevel.Particles;
            foreach (var bot in bots)
                if (!bot.IsDead) CalculateEntityRespond(bot, projectile, particles);

            CalculateEntityRespond(player, projectile, particles);
        }

        private void CalculateEntityRespond(LivingEntity entity, AbstractProjectile projectile, List<AbstractParticleUnit> levelParticles)
        {
            var penetrationTimes = 
                DynamicCollisionDetector.AreCollideWithDynamic(projectile, entity.CollisionShape, entity.Velocity);
            if (penetrationTimes == null) return;
            var penetrationPlace = projectile.Position + projectile.Velocity * penetrationTimes[0];
            var firstPenetrationTime = penetrationTimes[0];
            switch (projectile)
            {
                case Bullet bullet:
                    HandleBulletHit(entity, bullet, levelParticles, penetrationPlace, firstPenetrationTime);
                    break;
                case Grenade grenade:
                    HandleGrenadeExplosion(grenade, levelParticles, firstPenetrationTime);
                    break;
            }
        }

        private void HandleBulletHit(
            LivingEntity entity, Bullet bullet, List<AbstractParticleUnit> levelParticles, Vector penetrationPlace, float firstPenetrationTime)
        {
            HandleHit(entity, bullet, levelParticles, firstPenetrationTime);
            if (entity.Armor > 50)
            {
                bullet.IsStuck = true;
                levelParticles.Add(ParticleFactory.CreateSmallBloodSplash(penetrationPlace));
            }
            else
            {
                bullet.SlowDown();
                levelParticles.Add(ParticleFactory.CreateBloodSplash(penetrationPlace));
                levelParticles.Add(ParticleFactory.CreateBloodSplash(penetrationPlace));
            }
        }

        private void HandleGrenadeExplosion(
            Grenade grenade, List<AbstractParticleUnit> levelParticles, float firstPenetrationTime)
        {
            foreach (var bot in currentLevel.Bots)
            {
                if (bot.IsDead) continue;
                var v = bot.Position - grenade.Position;
                if (Vector.ScalarProduct(v, v) < grenade.DamageRadius * grenade.DamageRadius)
                {
                    HandleHit(bot, grenade, levelParticles, firstPenetrationTime);
                    levelParticles.Add(ParticleFactory.CreateBigBloodSplash(bot.Position));
                    levelParticles.Add(ParticleFactory.CreateBigBloodSplash(bot.Position));
                    levelParticles.Add(ParticleFactory.CreateBigBloodSplash(bot.Position));
                    levelParticles.Add(ParticleFactory.CreateBigBloodSplash(bot.Position));
                }
            }
            
            var x = player.Position - grenade.Position;
            if (Vector.ScalarProduct(x, x) < grenade.DamageRadius)
            {
                HandleHit(player, grenade, levelParticles, firstPenetrationTime);
                levelParticles.Add(ParticleFactory.CreateBigBloodSplash(player.Position));
                levelParticles.Add(ParticleFactory.CreateBigBloodSplash(player.Position));
                levelParticles.Add(ParticleFactory.CreateBigBloodSplash(player.Position));
                levelParticles.Add(ParticleFactory.CreateBigBloodSplash(player.Position));
            }
            
            levelParticles.Add(ParticleFactory.CreateExplosion(grenade.Position));
            levelParticles.Add(ParticleFactory.CreateExplosionFunnel(grenade.Position));
            AudioEngine.PlayNewInstance(@"event:/gunfire/3D/misc/GL_EXPLOSION_3D", grenade.Position);

            grenade.IsStuck = true;
            grenade.GetWarheadParticle().IsExpired = true;
        }

        private void HandleHit(
            LivingEntity entity, AbstractProjectile projectile, List<AbstractParticleUnit> levelParticles, float firstPenetrationTime)
        {
            entity.TakeHit(projectile.Damage);
            if (entity.IsDead && !entity.Velocity.Equals(Vector.ZeroVector))
            {
                entity.MoveTo(entity.CollisionShape.Center + entity.Velocity * firstPenetrationTime);
                HandleKill(entity, entity.Position - projectile.Position, levelParticles);
            }
        }
        
        private void HandleKill(LivingEntity deadEntity, Vector bodyDirection, List<AbstractParticleUnit> sceneParticles)
        {
            deadEntity.LegsContainer.ClearContent();
            deadEntity.TorsoContainer.ClearContent();
            deadEntity.CollisionShape.CanCollide = false;
            sceneParticles.Add(ParticleFactory.CreateDeadMenBody(EntityFactory.CreateDeadBody(deadEntity.DeadBodyPath),deadEntity.Position, bodyDirection));
            var wPosition = deadEntity.Position + new Vector(r.Next(48, 64), r.Next(48, 64));
            var collectableWeapon = AbstractWeaponFactory.CreateRuntimeCollectable(wPosition, deadEntity.GetWeaponType());
            currentLevel.Collectables.Add(collectableWeapon);
            currentLevel.Sprites.Add(collectableWeapon.SpriteContainer);
        }
        
        private void UpdateBots()
        {
            if (livingBotsAmount == 0 && currentLevel.WavesAmount != 0)
            {
                currentLevel.TryOptimize();
                LevelDynamicEntitiesFactory.SpawnBots(
                    currentLevel.BotSpawnPoints, player.Position, currentLevel.Bots, currentLevel.Sprites, currentLevel.DynamicShapes, currentLevel.BotPatrolPoints);
                livingBotsAmount += currentLevel.BotSpawnPoints.Count;
                currentLevel.WavesAmount--;
            }
            var regions = new List<Raytracing.VisibilityRegion>();
            //regions.Add(new Raytracing.VisibilityRegion(player.Position, currentLevel.RaytracingEdges, 1000));
            var paths = new List<List<Vector>> {Capacity = 10};
            var bots = currentLevel.Bots;
            var particles = currentLevel.Particles;
            livingBotsAmount = 0;
            foreach (var bot in bots)
            {
                if (bot.IsDead) continue;
                if (player.WasMeleeWeaponRaised && player.MeleeWeapon.IsInRange(bot))
                {
                    HandleMeleeHit(bot, particles);
                }
                bot.Update(
                    player.Position, player.Velocity, 
                    currentLevel.Projectiles, currentLevel.Particles, 
                    currentLevel.SceneShapes, paths, currentLevel.RaytracingEdges);
                if (!bot.IsDead) livingBotsAmount++;
            }

            currentLevel.VisibilityRegions = regions;
            currentLevel.Paths = paths;
        }

        private void HandleMeleeHit(LivingEntity bot, List<AbstractParticleUnit> sceneParticles)
        {
            bot.TakeHit(player.MeleeWeapon.Damage);
            if (bot.IsDead) HandleKill(bot, bot.Position - player.Position, sceneParticles);
            var particlePosition = player.Position + (bot.Position - player.Position).Normalize() * player.Radius * 3;
            sceneParticles.Add(ParticleFactory.CreateBigBloodSplash(particlePosition));
            sceneParticles.Add(ParticleFactory.CreateBigBloodSplash(particlePosition));
        }

        public void OnKeyDown(Keys keyPressed)
        {
            switch (keyPressed)
            {
                case Keys.W:
                    keyState.W = true;
                    break;

                case Keys.S:
                    keyState.S = true;
                    break;

                case Keys.A:
                    keyState.A = true;
                    break;

                case Keys.D:
                    keyState.D = true;
                    break;
                
                case Keys.I:
                    keyState.I = true;
                    keyState.pressesOnIAmount++;
                    break;
                
                case Keys.R:
                    keyState.R = true;
                    break;
                
                case Keys.P:
                    keyState.P = true;
                    keyState.pressesOnPAmount++;
                    break;
                
                case Keys.ShiftKey:
                    keyState.Shift = true;
                    break;
            }
        }

        public void OnKeyUp(Keys keyPressed)
        {
            switch (keyPressed)
            {
                case Keys.Escape:
                    AudioEngine.Release();
                    Application.Exit();
                    break;
                
                case Keys.W:
                    keyState.W = false;
                    break;

                case Keys.S:
                    keyState.S = false;
                    break;

                case Keys.A:
                    keyState.A = false;
                    break;

                case Keys.D:
                    keyState.D = false;
                    break;
                
                case Keys.I:
                    keyState.I = false;
                    break;
                
                case Keys.R:
                    keyState.R = false;
                    break;
                
                case Keys.P:
                    keyState.P = false;
                    break;
                
                case Keys.ShiftKey:
                    keyState.Shift = false;
                    break;
            }
        }
        
        public void OnMouseWheel(int wheelDelta)
        {
            if (wheelDelta > 0) player.MoveNextWeapon();
            else player.MovePreviousWeapon();
        }

        public void OnMouseDown(MouseButtons buttons)
        {
            switch (buttons)
            {
                case MouseButtons.Left:
                    mouseState.LMB = true;
                    break;
                
                case MouseButtons.Right:
                    mouseState.RMB = true;
                    break;
            }
        }

        public void OnMouseUp(MouseButtons buttons)
        {
            switch (buttons)
            {
                case MouseButtons.Left:
                    mouseState.LMB = false;
                    break;
                
                case MouseButtons.Right:
                    mouseState.RMB = false;
                    break;
            }
        }
    }
}
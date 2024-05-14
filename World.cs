﻿using Gala.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Gala
{
    internal class World
    {
        private EntityManager entityManager;
        private InputSystem inputSystem;
        private MoveSystem moveSystem;

        private int playerEntityId;

        // Input stuff
        private Vector2 moveDirection;

        private readonly float moveSpeed = 300; // pixels per second
        
        public FollowCamera Camera { get; private set; }

        public World(string filepath)
        {
            entityManager = new EntityManager();
            inputSystem = new InputSystem(filepath);
            moveSystem = new MoveSystem();
        }

        public void GetInput()
        {
            moveDirection = inputSystem.GetMovement();
        }

        public void CreatePlayer(ContentManager content)
        {
            // Creating the player entity
            playerEntityId = entityManager.CreateEntity();
            entityManager.AddComponent(playerEntityId, ComponentFlag.InputComponent, new InputComponent());
            entityManager.AddComponent(playerEntityId, ComponentFlag.GraphicComponent, new GraphicComponent
            {
                offset = new Vector2(-64, -64),
                texture = content.Load<Texture2D>("ship")
            });
            entityManager.AddComponent(playerEntityId, ComponentFlag.TransformComponent, new TransformComponent
            {
                position = new Vector2(400, 400)
            });
        }

        public void Update(GameTime gameTime)
        {
            // Maybe move this outside the update call?
            List<Entity> moveEntites = entityManager.getMoveEntities();

            foreach (Entity entity in moveEntites)
            {
                moveSystem.ApplyMovement(ref entityManager.GetTransformComponent(entity.Id), moveDirection * moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            Camera.Update();
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Maybe move this outside the update call?
            List<Entity> drawEntites = entityManager.getDrawEntities();

            foreach (Entity entity in drawEntites)
            {
                var transform = entityManager.GetTransformComponent(entity.Id);
                var graphic = entityManager.GetGraphicComponent(entity.Id);

                spriteBatch.Draw(graphic.texture, transform.position, Color.White);
            }
        }

        public void AddCamera(Viewport viewport)
        {
            Camera = new FollowCamera(viewport, playerEntityId, entityManager);
        }
    }
}

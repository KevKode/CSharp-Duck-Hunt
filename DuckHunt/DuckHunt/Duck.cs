using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;


namespace DuckHunt
{
    class Duck
    {
        public Rectangle duckRect;
        public Boolean isFlying;
        public Boolean[] isDead;
        public int lives;
        public Boolean gameOver;


        float timer;
        float interval;
        public int currentFrame;
        int frameCount;
        int spriteWidth;
        int spriteHeight;
        public Rectangle sourceRect;
        public SpriteEffects s;
        int flyTime;
        int leftRight;

        int fallTime;

        Random rand;

        public int shot;

        public Duck()
        {
            isDead = new Boolean[20];

            rand = new Random();

            duckRect = new Rectangle(rand.Next(200, 801), 450, 74, 70);
            isFlying = true;

            timer = 0f;
            interval = 1000f / 20f;
            currentFrame = 0;
            frameCount = 8;
            spriteWidth = 34;
            spriteHeight = 31;

            shot = 3;
        }

        public void Update(float deltaTime, SoundEffect gunCock)
        {
            timer += deltaTime;

            if (timer > interval)
            {
                if (isFlying)
                {                    
                    //START SPRITE
                    currentFrame++;
                    if (currentFrame > frameCount - 6)
                        currentFrame = 0;
                    timer = 0f;
                    //END SPRITE

                    //START MOVEMENT
                    if (flyTime == 0)
                    {
                        leftRight = rand.Next(2);
                        flyTime = 10;
                    }

                    if (duckRect.X >= 1026)
                    {
                        leftRight = 0;
                        flyTime = 10;
                    }

                    if (duckRect.X <= 0)
                    {
                        leftRight = 1;
                        flyTime = 10;
                    }

                    if (leftRight == 0)
                    {
                        duckRect.X -= 15;
                        duckRect.Y -= 10;
                        s = SpriteEffects.FlipHorizontally;
                    }
                    else
                    {
                        duckRect.X += 15;
                        duckRect.Y -= 10;
                        s = SpriteEffects.None;
                    }

                    flyTime--;

                    if (duckRect.Y <= -71)
                    {
                        duckRect.X = rand.Next(200, 801);
                        duckRect.Y = 450;
                        lives++;
                        if (lives == 20)
                        {
                            gameOver = true;
                            isFlying = false; 
                        }
                        if (shot != 3 && !gameOver)
                        {
                            gunCock.Play();
                            shot = 3;
                        }
                        
                    }
                }

                else
                {
                    if (fallTime <= 50)
                    {
                        currentFrame = 6;
                    }

                    else if (duckRect.Y <= 450)
                    {
                        currentFrame = 7;
                        duckRect.Y += 5;
                    }
                    else
                    {
                        isFlying = true;
                        duckRect.X = rand.Next(200, 801);
                        duckRect.Y = 450;
                        fallTime = 0;
                        if (lives == 20)
                        {
                            gameOver = true;
                            isFlying = false;
                        }
                        if (!gameOver)
                        {
                            gunCock.Play();
                            shot = 3;
                        }
                    }

                    fallTime++;
                }
                //END MOVEMENT

                sourceRect = new Rectangle(currentFrame * spriteWidth, 0, spriteWidth, spriteHeight);
            }
        }
    }
}




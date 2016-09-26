using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace DuckHunt
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D directions;
        Rectangle directRect;

        GamePadState state;
        GamePadState prevState;
        int isShooting;

        Texture2D dog;
        Rectangle dogRect;
        float timer;
        float interval;
        float deltaTime;
        float jumpTime;
        int currentFrame;
        int frameCount;
        int spriteWidth;
        int spriteHeight;
        Rectangle sourceRect;
        SoundEffect sniff;

        Texture2D screen;
        Texture2D flora;
        Rectangle screenRect;
        Rectangle floraRect;

        Texture2D reticle;
        Texture2D explosion;
        Rectangle retRect;
        Rectangle exRect;
        SoundEffect gunshot;
        SoundEffect gunCock;
        SoundEffect gunDry;
        Point center;

        Texture2D duck;
        Duck quack;
        SoundEffect quackSound;

        Texture2D shot0;
        Texture2D shot1;
        Texture2D shot2;
        Texture2D shot3;
        Rectangle shotRect;

        SpriteFont quartz;
        Vector2 scorePos;
        Vector2 duckScorePos;
        Vector2 posScorePos;
        int score;
        int posScore;

        Vector2 gameOverPos;
        Vector2 finalScorePos;
        Vector2 finalNumScorePos;
        Texture2D endDog;
        Rectangle endDogRect;

        Texture2D pointDuck;
        Rectangle[] pointDuckRect;

        Texture2D cloud;
        Rectangle[] cloudRect;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1100;
            graphics.PreferredBackBufferHeight = 700;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            directRect = new Rectangle(225, 75, 650, 325);

            dogRect = new Rectangle(-190, 410, 190, 138);
            timer = 0f;
            jumpTime = 0f;
            interval = 1000f / 7f;
            currentFrame = 0;
            frameCount = 8;
            spriteWidth = 55;
            spriteHeight = 47;

            screenRect = new Rectangle(0, 0, 1100, 700);
            floraRect = new Rectangle(0, 38, 1100, 500);

            retRect = new Rectangle(525, 325, 65, 65);
            exRect = new Rectangle(535, 335, 45, 45);
            center = new Point(557, 557);

            quack = new Duck();

            shotRect = new Rectangle(113, 607, 80, 45);

            duckScorePos = new Vector2(275, 592);
            scorePos = new Vector2(825, 623);
            posScorePos = new Vector2(825, 555);

            gameOverPos = new Vector2(400, 150);
            finalScorePos = new Vector2(475, 200);
            finalNumScorePos = new Vector2(490, 250);
            endDogRect = new Rectangle(485, 425, 150, 109);

            pointDuckRect = new Rectangle[20];
            for (int x = 0; x < 20; x++)
            {
                if (x < 10)
                    pointDuckRect[x] = new Rectangle(x * 30 + 415, 607, 20, 20);
                else
                    pointDuckRect[x] = new Rectangle(pointDuckRect[x - 10].X, 632, 20, 20);
            }

            cloudRect = new Rectangle[4];
            cloudRect[0] = new Rectangle(-140, 0, 350, 100);
            cloudRect[1] = new Rectangle(140, 110, 350, 100);
            cloudRect[2] = new Rectangle(420, 0, 350, 100);
            cloudRect[3] = new Rectangle(700, 110, 350, 100);
                base.Initialize();
        }

        protected override void LoadContent()
        {
            directions=Content.Load<Texture2D>("directions");

            dog = Content.Load<Texture2D>("dogWalk");
            sniff = Content.Load<SoundEffect>("sniff");
            sniff.Play();

            screen = Content.Load<Texture2D>("screen");
            flora = Content.Load<Texture2D>("flora");

            reticle = Content.Load<Texture2D>("reticle");
            explosion = Content.Load<Texture2D>("explosion");
            gunshot = Content.Load<SoundEffect>("gunshot");
            gunCock = Content.Load<SoundEffect>("gunCock");
            gunDry = Content.Load<SoundEffect>("gunDry");

            duck = Content.Load<Texture2D>("duck");
            quackSound = Content.Load<SoundEffect>("quackSound");

            shot0 = Content.Load<Texture2D>("shot0");
            shot1 = Content.Load<Texture2D>("shot1");
            shot2 = Content.Load<Texture2D>("shot2");
            shot3 = Content.Load<Texture2D>("shot3");

            quartz = Content.Load<SpriteFont>("Quartz");

            pointDuck = Content.Load<Texture2D>("pointDuck");

            endDog = Content.Load<Texture2D>("endDog");

            cloud = Content.Load<Texture2D>("cloud");

            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            //START DOG
            deltaTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            timer += deltaTime;

            if (timer > interval)
            {
                currentFrame++;

                if (!(currentFrame == 0 || currentFrame == 1 || currentFrame == 5) && dogRect.X <= 450)
                    dogRect.X += 10;
                if (currentFrame > frameCount - 4 && dogRect.X <= 450)
                    currentFrame = 0;
                if (dogRect.X >= 460 && jumpTime <= 50f)
                {
                    currentFrame = 5;
                    jumpTime += deltaTime;
                }
                if (jumpTime > 50f)
                {
                    currentFrame = 6;
                    dogRect.Y = 345;
                    jumpTime += deltaTime;
                }
                if (jumpTime > 70f)
                {
                    dogRect.Y = 360;
                    currentFrame = 7;
                }
                if (jumpTime > 100f)
                    dog = null;

                timer = 0f;
            }
            sourceRect = new Rectangle(currentFrame * spriteWidth, 0, spriteWidth, spriteHeight);
            //END DOG

            //START CLOUD
                for (int x=0; x<4; x++)
                {
                    cloudRect[x].X -= 1;

                    if (cloudRect[x].X + 350 <= 0)
                        cloudRect[x].X = 1100;
                }
            //END CLOUD

            if (dog == null && !quack.gameOver )
            {
                //START RETICLE
                state = GamePad.GetState(PlayerIndex.One);
                retRect.X += (int)(state.ThumbSticks.Left.X * 15);
                retRect.Y -= (int)(state.ThumbSticks.Left.Y * 15);
                exRect.X += (int)(state.ThumbSticks.Left.X * 15);
                exRect.Y -= (int)(state.ThumbSticks.Left.Y * 15);
                center.X = retRect.X + 32;
                center.Y = retRect.Y + 32;

                if (retRect.X <= -32)
                {
                    retRect.X = -32;
                    exRect.X = -22;
                }
                if (retRect.Y <= -32)
                {
                    retRect.Y = -32;
                    exRect.Y = -22;
                }
                if (retRect.X >= 1068)
                {
                    retRect.X = 1068;
                    exRect.X = 1078;
                }
                if (retRect.Y >= 500)
                {
                    retRect.Y = 500;
                    exRect.Y = 510;
                }
                //END RETICLE

                //START DUCK
                quack.Update(deltaTime, gunCock);
                //END DUCK

                //START SHOOT
                if (quack.isFlying)
                    if (quack.shot == 3)
                        posScore = (quack.duckRect.Y + 70) * (quack.shot);
                    else
                        posScore = (quack.duckRect.Y + 70) * (quack.shot + 1);

                if (posScore <= 0)
                    posScore = 0;

                if (state.IsButtonDown(Buttons.A) && prevState.IsButtonUp(Buttons.A) && quack.shot == 0)
                    gunDry.Play();

                if (state.IsButtonDown(Buttons.A) && prevState.IsButtonUp(Buttons.A) && quack.shot != 0)
                {
                    isShooting = 5;
                    gunshot.Play();
                    quack.shot--;

                    if ((center.X >= quack.duckRect.X && center.X <= quack.duckRect.X + 65) && (center.Y >= quack.duckRect.Y && center.Y <= quack.duckRect.Y + 71) && (quack.isFlying))
                    {
                        quack.isFlying = false;
                        quackSound.Play();
                        score += posScore;
                        quack.isDead[quack.lives] = true;
                        quack.lives++;
                    }

                }
                else
                    isShooting--;

                prevState = state;
                //END SHOOT

            }

            if (dog == null && quack.gameOver)
            {
                if (endDogRect.Y > 350)
                    endDogRect.Y -= 2;
            }
           

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            spriteBatch.Begin();
            spriteBatch.Draw(screen, screenRect, Color.White);

            if (quack.shot == 3)
                spriteBatch.Draw(shot3, shotRect, Color.White);
            else if (quack.shot == 2)
                spriteBatch.Draw(shot2, shotRect, Color.White);
            else if (quack.shot == 1)
                spriteBatch.Draw(shot1, shotRect, Color.White);
            else
                spriteBatch.Draw(shot0, shotRect, Color.White);

            spriteBatch.DrawString(quartz, "HIT:", duckScorePos, new Color(64, 191, 255));
            spriteBatch.DrawString(quartz, score + "", scorePos, new Color(64, 191, 255));
            spriteBatch.DrawString(quartz, posScore + "", posScorePos, new Color(64, 191, 255));

            for (int x = 0; x < 20; x++)
            {
                
                if (quack.isDead[x])
                    spriteBatch.Draw(pointDuck, pointDuckRect[x], Color.Red);
                else if(x == quack.lives)
                    spriteBatch.Draw(pointDuck, pointDuckRect[x], Color.LightSkyBlue);
                else
                    spriteBatch.Draw(pointDuck, pointDuckRect[x], Color.White);
            }
            
            if (dog != null)
            {
                foreach (Rectangle r in cloudRect)
                    spriteBatch.Draw(cloud, r, Color.White);
                spriteBatch.Draw(flora, floraRect, Color.White);
                spriteBatch.Draw(dog, dogRect, sourceRect, Color.White);
                spriteBatch.Draw(directions, directRect, Color.White);
            }
            else
            {
                spriteBatch.Draw(duck, quack.duckRect, quack.sourceRect, Color.White, 0, Vector2.Zero, quack.s, 0);
                if (quack.gameOver)
                    spriteBatch.Draw(endDog, endDogRect, Color.White);
                foreach (Rectangle r in cloudRect)
                    spriteBatch.Draw(cloud, r, Color.White);
                spriteBatch.Draw(flora, floraRect, Color.White);
                if (isShooting > 0)
                    spriteBatch.Draw(explosion, exRect, Color.White);
                if (!quack.gameOver)
                    spriteBatch.Draw(reticle, retRect, Color.White);
            }

            if (quack.gameOver)
            {
                spriteBatch.DrawString(quartz, "Game Over!", gameOverPos, Color.Black);
                spriteBatch.DrawString(quartz, "Score:", finalScorePos, Color.Black);
                spriteBatch.DrawString(quartz, score + "", finalNumScorePos, Color.Black);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

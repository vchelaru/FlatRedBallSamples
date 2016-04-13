#region Usings

using System;
using System.Collections.Generic;
using System.Text;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Graphics.Particle;

using FlatRedBall.Math.Geometry;
using FlatRedBall.Math.Splines;
using BitmapFont = FlatRedBall.Graphics.BitmapFont;
using Cursor = FlatRedBall.Gui.Cursor;
using GuiManager = FlatRedBall.Gui.GuiManager;
using FlatRedBall.Math;

#if FRB_XNA || SILVERLIGHT
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using Texture2D = Microsoft.Xna.Framework.Graphics.Texture2D;

#endif
#endregion

namespace StarBlaster.Entities
{
	public partial class Explosion
	{
        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
		private void CustomInitialize()
		{


		}

		private void CustomActivity()
		{

		}

        public void PlayExplosion()
        {
            const int bigExplosions = 12;
            const float delay = .007f;
            CreateExplosion(120);


            for (int i = 1; i < bigExplosions; i++)
            {
                this.Call(() => CreateExplosion(90)).After(i * delay);
            }
        }

        private void CreateExplosion(float endRadius)
        {
            

            var sprite = new Sprite();
            sprite.Texture = ExplosionCropped;
            sprite.Position = this.Position;
            var pointInCircle = MathFunctions.GetPointInCircle(40);
            sprite.Position.X  += (float)pointInCircle.X;
            sprite.Position.Y  += (float)pointInCircle.Y;
            sprite.BlendOperation = FlatRedBall.Graphics.BlendOperation.Add;
            const float life = .3f;
            const float starting = 20;
            sprite.AlphaRate = -1 / life;
            sprite.ScaleX = starting;
            sprite.ScaleY = starting;

            sprite.ScaleXVelocity = (endRadius - starting) / life;
            sprite.ScaleYVelocity = (endRadius - starting) / life;

            SpriteManager.AddSprite(sprite);
            this.SpriteList.Add(sprite);
        }

        private void CustomDestroy()
		{


		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}
}

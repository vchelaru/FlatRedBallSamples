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
using Microsoft.Xna.Framework;
using FlatRedBall.Math;

#if FRB_XNA || SILVERLIGHT
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using Texture2D = Microsoft.Xna.Framework.Graphics.Texture2D;

#endif
#endregion

namespace StarBlaster.Entities
{
	public partial class EnemyShip
	{

        int health;

        float distanceMoved;

        public Vector3 SplineOffset;

        public Spline SplineFollowing
        {
            get;
            set;
        }

        public int? SplinePointToLoopTo
        {
            get;
            set;
        }

        bool IsAtEndOfSpline
        {
            get
            {
                return distanceMoved >= SplineFollowing.Length;
            }
        }


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
            MovementActivity();

            if(IsAtEndOfSpline)
            {
                PerformEndOfSplineActivity();
            }

		}

        private void PerformEndOfSplineActivity()
        {
            if (SplinePointToLoopTo == null)
            {
                this.Destroy();
            }
            else
            {
                var point = SplineFollowing[SplinePointToLoopTo.Value];

                this.distanceMoved = (float)SplineFollowing.GetLengthAtTime(point.Time);
            }
        }

        public void TakeDamage(int amountOfDamage)
        {
            health -= amountOfDamage;

            if (health <= 0)
            {
                var explosion = new Explosion();
                explosion.Position = this.Position;
                explosion.PlayExplosion();
                explosion.Call(explosion.Destroy).After(3);
                this.Destroy();
            }
            else
            {
                ReactToTakingDamage();
            }
        }

        private void ReactToTakingDamage()
        {
            CurrentFlashingCategoryState = FlashingCategory.FlashOn;
            this.Set(nameof(CurrentFlashingCategoryState))
                .To(FlashingCategory.FlashOff)
                .After(DamageFlashDuration);
        }

        private void MovementActivity()
        {
            var oldPosition = this.Position;

            var newPosition = GetNewPosition();

            this.Position = newPosition;

            // This biases movement in the past and doesn't consider
            // movement that will happen, so the angle will lag behind
            // slightly - but it shouldn't be noticeable:
            var directionMoved = oldPosition - newPosition;

            if (EnemyInfo.AlwaysFaceUp == false)
            {
                FaceDirectionMoving(directionMoved);
            }
        }

        private void FaceDirectionMoving(Vector3 directionMoved)
        {
            if (directionMoved.LengthSquared() != 0)
            {
                var angle = (float)Math.Atan2(directionMoved.Y, directionMoved.X);

                // Up is an angle of 0, so we need to offset by pi/2
                this.RotationZ = angle + MathHelper.PiOver2;
            }
        }

        private Vector3 GetNewPosition()
        {
            distanceMoved += TimeManager.SecondDifference * EnemyInfo.MovementSpeed;

            if(SplineFollowing != null)
            {
                return SplineFollowing.GetPositionAtLengthAlongSpline(distanceMoved) + SplineOffset;

            }
            else
            {
                return this.Position;
            }
        }

        private void CustomDestroy()
		{


		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}
}

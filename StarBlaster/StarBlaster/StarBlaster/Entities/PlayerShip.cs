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
using StarBlaster.Factories;

#if FRB_XNA || SILVERLIGHT
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using Texture2D = Microsoft.Xna.Framework.Graphics.Texture2D;

#endif
#endregion

namespace StarBlaster.Entities
{
	public partial class PlayerShip
	{
        I2DInput movementInput;
        IPressableInput shootInput;

        int gunLevel = 1;

        public event Action<Bullet> BulletCreated;

        public float CollisionRadius
        {
            get
            {
                return this.CircleInstance.Radius;
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
            ApplyInput();


		}

        private void ApplyInput()
        {
            float desiredXVelocity = movementInput.X * MaxMovementSpeed;
            float desiredYVelocity = movementInput.Y * MaxMovementSpeed;

            float xDifference = desiredXVelocity - this.XVelocity;
            float yDifference = desiredYVelocity - this.YVelocity;

            const float threshold = 1;

            if (System.Math.Abs(xDifference) < threshold)
            {
                this.XAcceleration = 0;
                this.XVelocity = desiredXVelocity;
            }
            else
            {
                float xAccelerationSign = Math.Sign(xDifference);
                var oldXAccelerationSign = Math.Sign(this.XAcceleration);
                this.XAcceleration = xAccelerationSign * (MaxMovementSpeed / AccelerationTime);
            }

            if(System.Math.Abs(yDifference) < threshold)
            {
                this.YAcceleration = 0;
                this.YVelocity = desiredYVelocity;
            }
            else
            {
                float yAccelerationSign = Math.Sign(desiredYVelocity - this.YVelocity);
                var oldYAccelerationSign = Math.Sign(this.YAcceleration);
                this.YAcceleration = yAccelerationSign * (MaxMovementSpeed / AccelerationTime);
            }


            if (this.shootInput.WasJustPressed)
            {
                ShootBullet();
            }
        }

        private void ShootBullet()
        {
            foreach (var offset in GetBulletOffsets())
            {

                var bullet = BulletFactory.CreateNew();
                bullet.Position = this.Position + offset;
                bullet.YVelocity = BulletVelocity;

                BulletCreated?.Invoke(bullet);
            }
        }

        private IEnumerable<Vector3> GetBulletOffsets()
        {
            switch(gunLevel)
            {
                case 0:
                    yield return new Vector3(0, 20, 0);
                    break;
                case 1:
                    yield return new Vector3(10, 10, 0);
                    yield return new Vector3(-10, 10, 0);
                    break;
            }
        }

        private void CustomDestroy()
		{


		}

        internal void AddKeyboardControls()
        {
            movementInput = InputManager.Keyboard.Get2DInput(Keys.A, Keys.D, Keys.W, Keys.S);

            shootInput = InputManager.Keyboard.GetKey(Keys.Space);
        }

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}
}

using System;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Specialized;
using FlatRedBall.Audio;
using FlatRedBall.Screens;
using StarBlaster.Entities;
using StarBlaster.Screens;
namespace StarBlaster.Entities
{
	public partial class PlayerShip
	{
        void OnAfterSideGunsVisibleSet (object sender, EventArgs e)
        {
            this.LeftGun.Visible = this.SideGunsVisible;
            this.RightGun.Visible = this.SideGunsVisible;
            
        }
		
	}
}

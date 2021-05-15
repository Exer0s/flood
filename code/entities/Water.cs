using System;

namespace Sandbox
{
    [Library( "water" )]

	// Sam: I won't make any underwater physics logic right now
	public partial class Water : AnimEntity
	{
		public WaterController WaterController = new WaterController();

		public Water()
		{
			WaterController.WaterEntity = this;

			EnableTouch = true;
			EnableTouchPersists = true;
		}

		public override void Touch( Entity other )
		{
			base.Touch( other );

			WaterController.Touch( other );
		}

		public override void EndTouch( Entity other )
		{
			base.EndTouch( other );

			WaterController.EndTouch( other );
		}

		public override void StartTouch( Entity other )
		{
			base.StartTouch( other );

			WaterController.StartTouch( other );
		}
	}    
}

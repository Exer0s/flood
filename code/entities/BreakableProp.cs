using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

[Library( "prop_flood" )]
public partial class BreakableProp : Prop
{
	[Net] public float Health { get; set; }

	protected override void UpdatePropData(Model model)
	{
		var propInfo = model.GetPropData();
		Health = (int)propInfo.Health;
	}

	public override void TakeDamage(DamageInfo info)
	{
		Health -= info.Damage;

		if (Health <= 0)
		{
			OnKilled();
		}

		base.TakeDamage(info);
	}

	public override void OnKilled()
	{
		base.OnKilled();
	}

}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

[Library( "prop_flood" )]
public partial class BreakableProp : Prop
{
	[Net] public float PropHealth { get; set; }
	[Net] public List<Entity> holdingPlayers { get; set; } = new List<Entity>();

	protected override void UpdatePropData(Model model)
	{
		var propInfo = model.GetPropData();
		PropHealth = propInfo.Health;
	}

	public override void TakeDamage(DamageInfo info)
	{
		PropHealth -= info.Damage;

		if (PropHealth <= 0)
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


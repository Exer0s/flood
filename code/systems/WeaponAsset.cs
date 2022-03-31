using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

[Library( "weapon" ), AutoGenerate]
public partial class WeaponAsset : Asset
{

	public string Title { get; set; }
	public string Weapon { get; set; }

	[ResourceType( "png" )]
	public string Icon { get; set; }
	public int Cost { get; set; }
	public static IReadOnlyList<WeaponAsset> All => _all;
	internal static List<WeaponAsset> _all = new();

	protected override void PostLoad()
	{
		base.PostLoad();
		if ( !_all.Contains( this ) )
			_all.Add( this );
	}
}

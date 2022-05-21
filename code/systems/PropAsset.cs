using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

[GameResource( "props", "fprop", "Flood Prop" )]
public partial class PropAsset : GameResource
{

	public string Title { get; set; }

	[ResourceType( "vmdl" )]
	public string Model { get; set; }
	[ResourceType( "png" )]
	public string Icon { get; set; }
	public int Cost { get; set; }
	public float DestroyPayout { get; set; }
	public float Health { get; set; }
	public static IReadOnlyList<PropAsset> All => _all;
	internal static List<PropAsset> _all = new();

	protected override void PostLoad()
	{
		base.PostLoad();
		if ( !_all.Contains( this ) )
			_all.Add( this );
	}
}

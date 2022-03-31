using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

[Library( "props" ), AutoGenerate]
public partial class PropAsset : Asset
{

	public string Title { get; set; }

	[ResourceType( "vmdl" )]
	public string Model { get; set; }
	public string Icon { get; set; }
	public int Cost { get; set; }
	public static IReadOnlyList<PropAsset> All => _all;
	internal static List<PropAsset> _all = new();

	protected override void PostLoad()
	{
		base.PostLoad();

		if ( !_all.Contains( this ) )
			_all.Add( this );
	}
}

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class FloodCrossPanel : Panel
{

	public Panel CrosshairCenter;
	public Panel CrosshairCircle;
	public Panel Hitmarker;
	public Panel PlayerHitmarker;

	TimeSince hit;
	TimeSince playerhit;

	public float CircleSpread { get; set; } = 1f;

	public static FloodCrossPanel Instance;

	public FloodCrossPanel()
	{
		Instance = this;
		StyleSheet.Load( "ui/FloodCrossPanel.scss" );
		Add.Panel( "crosshaircenter" );

		var circlepanel = Add.Panel( "crosscirclepanel" );
		CrosshairCircle = circlepanel.Add.Panel( "crosshaircircle" );


		//Prop Hitmarker
		var hitp = Add.Panel( "hitmarkerpanel" );
		Hitmarker = hitp.Add.Panel( "hitmarker" );
		Hitmarker.Style.BackgroundImage = Texture.Load( FileSystem.Mounted, "ui/hud/hit.png" );

		//Player Hitmarker
		var phitp = Add.Panel( "phitmarkerpanel" );
		PlayerHitmarker = phitp.Add.Panel( "phitmarker" );
		PlayerHitmarker.Style.BackgroundImage = Texture.Load( FileSystem.Mounted, "ui/hud/hit2.png" );
	}

	public void OnHit( float dmg )
	{
		hit = 0;
		Hitmarker.SetClass( "active", true );
	}

	public void OnHitPlayer()
	{
		playerhit = 0;
		PlayerHitmarker.SetClass( "active", true );
	}

	public override void Tick()
	{
		if ( hit > 0.1f ) Hitmarker.SetClass( "active", false );
		if ( playerhit > 0.1f ) PlayerHitmarker.SetClass( "active", false );

		if ( CircleSpread > 1 ) CircleSpread -= Time.Delta * 5;
		else CircleSpread = 1;
		CrosshairCircle.Style.Width = Length.Pixels(3 * CircleSpread);
		CrosshairCircle.Style.Height = Length.Pixels(3 * CircleSpread);
		Log.Info( CircleSpread );
		base.Tick();
	}

}

public class HitPoint : Panel
{
	public HitPoint( float amount, Vector3 pos, Panel parent )
	{
		StyleSheet.Load( "ui/DamageNumber.scss" );
		Parent = parent;
		_ = Lifetime();
		Add.Label( amount.ToString(), "number" );
	}

	async Task Lifetime()
	{
		await Task.Delay( 200 );
		Delete();
	}
}

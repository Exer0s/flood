using Sandbox;
//Part of the sandbox gamemode, used in tools
public partial class Carriable : BaseCarriable, IUse
{
	public override void CreateViewModel()
	{
		Host.AssertClient();

		if ( string.IsNullOrEmpty( ViewModelPath ) )
			return;

		ViewModelEntity = new DmViewModel
		{
			WorldPos = WorldPos,
			Owner = Owner,
			EnableViewmodelRendering = true
		};

		ViewModelEntity.SetModel( ViewModelPath );
	}

	public bool OnUse( Entity user )
	{
		return false;
	}

	public virtual bool IsUsable( Entity user )
	{
		return Owner == null;
	}
}

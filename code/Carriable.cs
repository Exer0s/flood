using Sandbox;
//Part of the sandbox gamemode, used in tools
public partial class Carriable : BaseCarriable, IUse
{
	
	public virtual int Bucket => 1;
	public virtual int BucketWeight => 100;
	
	public override void CreateViewModel()
	{
		Host.AssertClient();

		if ( string.IsNullOrEmpty( ViewModelPath ) )
			return;

		ViewModelEntity = new FloodViewModel
		{
			Position = Position,
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

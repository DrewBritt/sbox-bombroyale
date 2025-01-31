﻿using Editor;
using Sandbox;
using System.Linq;

namespace Facepunch.BombRoyale;

[HammerEntity]
[Title( "Bombable Entity")]
[Description( "A player blocker that can be destroyed by a bomb." )]
[SupportsSolid]
[Model]
public partial class BombableEntity : ModelEntity, IResettable
{
	[Net] public bool IsHidden { get; private set; }

	public void Reset()
	{
		Show();
	}

	public bool IsSpaceOccupied()
	{
		if ( !IsHidden ) return true;

		var trace = Trace.Ray( WorldSpaceBounds.Center, WorldSpaceBounds.Center )
			.EntitiesOnly()
			.Radius( 8f )
			.WithAnyTags( "solid", "pickup", "player", "bomb" )
			.Run();

		return trace.Hit;
	}

	public override void Spawn()
	{
		EnableAllCollisions = true;
		EnableShadowCasting = true;
		Transmit = TransmitType.Always;

		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );

		base.Spawn();
	}

	public void TrySpawnPickup()
	{
		if ( Game.Random.Float() < 0.35f )
		{
			var p = Pickup.CreateRandom();
			p.Position = WorldSpaceBounds.Center;
			return;
		}
	}

	public void Hide()
	{
		EnableAllCollisions = false;
		IsHidden = true;
	}

	public void Show()
	{
		EnableAllCollisions = true;
		IsHidden = false;
	}

	[Event.Tick.Client]
	private void ClientTick()
	{
		EnableDrawing = !IsHidden;
	}
}

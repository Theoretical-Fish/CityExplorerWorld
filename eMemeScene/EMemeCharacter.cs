using Godot;
using System;

public partial class EMemeCharacter : CharacterBody3D
{
	public const float Speed = 0.5f;
	public float tick = 0.00f;
	public const float limit = 200.00f;

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		Vector3 direction = (Transform.Basis * new Vector3(0,10,0)).Normalized();
		float movement = direction.Y * Speed;
		velocity.Y = movement;
		Velocity = velocity;
		MoveAndSlide();

		tick += movement;

		if (tick >= limit)
		{
			QueueFree();
		}

	}
}

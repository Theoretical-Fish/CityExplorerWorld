using Godot;
using System;
using System.IO;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Resolvers;

public partial class FpsController : CharacterBody3D
{

	[Export] public float Speed = 5.00f;
	[Export] public float JumpVelocity = 4.50f;
	[Export] public float MOUSE_SENSITIVITY = 0.50f;
	[Export] public float TILT_LOWER_LIMIT = Single.DegreesToRadians(-90);
	[Export] public float TILT_UPPER_LIMIT = Single.DegreesToRadians(90);
	[Export] public Node3D CAMERA_CONTROLLER;

	private Sprite2D _markiplier;

	public PackedScene _markimark;

	public Marker3D shoot_from;
	public Godot.Vector3 shootOrigin;

	public bool _mouse_input = false;
	public float _rotation_input;
	public float _tilt_input;
	public Godot.Vector3 _mouse_rotation;
	public Godot.Vector3 _camera_rotation;
	public Godot.Vector3 _player_rotation;

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;



		GD.Print("Test");
	}

	public override void _Input(InputEvent @event)
	{
			// Exit button
		if (Input.IsActionJustPressed("exit"))
		{
			GetTree().Quit();
		}

		if (Input.IsActionJustPressed("input"))
		{

			shoot_from = GetNode<Marker3D>("SpawnPoint/SpawnMarker");
			shootOrigin = shoot_from.GlobalTransform.Origin;
			_markimark = ResourceLoader.Load<PackedScene>("res://eMemeScene/eScene.tscn");
			Node tempEMeme = _markimark.Instantiate();
			AddChild(tempEMeme);
			CharacterBody3D charbody3d = (CharacterBody3D)tempEMeme.GetChild(0);
			charbody3d.GlobalPosition = shootOrigin;
			charbody3d.Rotate(new Godot.Vector3(0,1,0),Single.DegreesToRadians(GlobalRotationDegrees.Y));

		}
		
			// Mouse Movement
		if (@event is InputEventMouseMotion motionEvent && Input.MouseMode == Input.MouseModeEnum.Captured )
		{
			_rotation_input = -(motionEvent.Relative.X) * MOUSE_SENSITIVITY;
			_tilt_input = -(motionEvent.Relative.Y) * MOUSE_SENSITIVITY;
			
			//GD.Print(_rotation_input," | ",_tilt_input);
		}
	}

	public void Update_Camera(float delta)
	{

		_mouse_rotation.X += _tilt_input * delta;
		_mouse_rotation.X = Math.Clamp(_mouse_rotation.X, TILT_LOWER_LIMIT, TILT_UPPER_LIMIT);
		_mouse_rotation.Y += _rotation_input * delta;

		_player_rotation = new(0.0f,_mouse_rotation.Y,0.00f);
		_camera_rotation = new(_mouse_rotation.X,0.0f, 0.0f);

		CAMERA_CONTROLLER.Basis = Basis.FromEuler(_camera_rotation);
		CAMERA_CONTROLLER.RotateZ(0);

		Transform3D temp_transform = Transform;
		temp_transform.Basis = Basis.FromEuler(_player_rotation);
		Transform = temp_transform;

		_rotation_input = 0;
		_tilt_input = 0;

	}
	public override void _PhysicsProcess(double delta)
		{

			float fdelta = (float)delta;

			Godot.Vector3 velocity = Velocity;
			// Add the gravity.
			if (!IsOnFloor())
			{
				velocity += GetGravity() * fdelta;
			}

			// Handle Jump.
			if (Input.IsActionJustPressed("movement_jump") && IsOnFloor())
			{
				velocity.Y = JumpVelocity;
			}

			Update_Camera(fdelta);

			// Get the input direction and handle the movement/deceleration.
			// As good practice, you should replace UI actions with custom gameplay actions.
			Godot.Vector2 inputDir = Input.GetVector("movement_left", "movement_right", "movement_forward", "movement_back");
			Godot.Vector3 direction = (Transform.Basis * new Godot.Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
			if (direction != Godot.Vector3.Zero)
			{
				velocity.X = direction.X * Speed;
				velocity.Z = direction.Z * Speed;
			}
			else
			{
				velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
				velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
			}

			Velocity = velocity;
			MoveAndSlide();
		}
}

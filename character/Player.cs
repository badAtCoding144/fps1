using Godot;
using System;

public partial class Player : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	public float mouse_sensitivity = 0.002f;

	// Reference to the PlayerCamera
	public Camera3D PlayerCamera;

	// Rotation variables
	private float _pitch = 0.0f;
	private float _yaw = 0.0f;

	public override void _Ready()
	{
		// Hide the mouse cursor and capture it
		Input.MouseMode = Input.MouseModeEnum.Captured;

		// Assuming PlayerCamera is a child of the Player
		PlayerCamera = GetNode<Camera3D>("PlayerCamera");
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseMotionEvent)
		{
			// Capture mouse movement for player and camera rotation
			_yaw -= mouseMotionEvent.Relative.X * mouse_sensitivity;
			_pitch -= mouseMotionEvent.Relative.Y * mouse_sensitivity;

			// Clamp the pitch to prevent flipping over
			_pitch = Mathf.Clamp(_pitch, -Mathf.Pi / 2.0f, Mathf.Pi / 2.0f);

			// Apply the rotation
			Rotation = new Vector3(0, _yaw, 0); // Rotate player on the Y-axis
			PlayerCamera.Rotation = new Vector3(_pitch, _yaw, 0); // Rotate camera on both axes
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle movement relative to the camera.
		Vector2 inputDir = Input.GetVector("left", "right", "forward", "backward");
		Vector3 direction = (PlayerCamera.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		if (direction != Vector3.Zero)
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

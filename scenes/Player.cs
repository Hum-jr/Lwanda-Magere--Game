using Godot;

namespace LwandaMagere.scenes;

public partial class Player : CharacterBody3D
{
    public const float Speed = 5.0f;
    public const float JumpVelocity = 4.5f;
    public const float Sensitivity = 5.0f;
    private AnimationPlayer _animationPlayer;



    public override void _Ready()
    {
        base._Ready();
        Input.MouseMode = Input.MouseModeEnum.Captured;
        _animationPlayer = GetNode<AnimationPlayer>("Visuals/Player/AnimationPlayer");
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = Velocity;

        // Add the gravity.
        if (!IsOnFloor())
        {
            velocity += GetGravity() * (float)delta;
        }

        // Handle Jump.
        if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
        {
            velocity.Y = JumpVelocity;
        }

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 inputDir = Input.GetVector("Left", "Right", "Foward", "Back");
        Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * Speed;
            velocity.Z = direction.Z * Speed;
			
            _animationPlayer.Play("mixamo_com");

			
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
            _animationPlayer.Play("Take 001");
        }

        Velocity = velocity;
        MoveAndSlide();
    }
	
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if(@event is InputEventMouseMotion)
        {
            InputEventMouseMotion motion = @event as InputEventMouseMotion;
            Rotation = new Vector3(Rotation.X, Rotation.Y - motion.Relative.X / 1000 * Sensitivity, Rotation.Z);
			
            Node3D camera = GetNode<Node3D>("CameraMount");
			

            camera.Rotation = new Vector3(camera.Rotation.X - motion.Relative.Y / 1000 * Sensitivity, camera.Rotation.Y,
                camera.Rotation.Z);
        }
    }
}
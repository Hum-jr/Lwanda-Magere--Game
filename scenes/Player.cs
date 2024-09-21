using Godot;
using System;

namespace LwandaMagere.scenes;

public partial class Player : CharacterBody3D
{
    public const float Speed = 5.0f;
    public const float JumpVelocity = 4.5f;
    public const float Sensitivity = 5.0f;
    private AnimationPlayer _animationPlayer;
    private const float VerticalClampMin = -Mathf.Pi / 4; // -45 degrees
    private const float VerticalClampMax = Mathf.Pi / 4; // 45 degrees

    public override void _Ready()
    {
        base._Ready();
        Input.MouseMode = Input.MouseModeEnum.Confined;
        _animationPlayer = GetNode<AnimationPlayer>("Visuals/player/AnimationPlayer");
    }


    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = Velocity;

        if (!IsOnFloor())
        {
            velocity += GetGravity() * (float)delta;
        }

        if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
        {
            velocity.Y = JumpVelocity;
            _animationPlayer.Play("jump");
        }

        Vector2 inputDir = Input.GetVector("Right", "Left", "Foward", "Back");
        Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * Speed;
            velocity.Z = direction.Z * Speed;

        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
            _animationPlayer.Play("Idle");
        }

        Velocity = velocity;
        MoveAndSlide();

        // Play walk animation when in motion
        if (velocity.LengthSquared() > 0.1f)
        {
            if (Input.IsActionPressed("run"))
                _animationPlayer.Play("Run");
            else
            {
                _animationPlayer.Play("walk");
            }
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event is InputEventMouseMotion motion)
        {
            HandleCameraMovement(motion.Relative.X, motion.Relative.Y);
        }
        else if (@event is InputEventJoypadMotion joypadMotion)
        {
            if (joypadMotion.Axis == JoyAxis.RightX || joypadMotion.Axis == JoyAxis.RightY)
            {
                float sensitivity = 10.0f; // Adjust this value to change gamepad sensitivity
                float deadzone = 0.1f; // Adjust this value to set the deadzone for the analog stick

                Vector2 analogInput = new Vector2(
                    Input.GetJoyAxis(0, JoyAxis.RightX),
                    Input.GetJoyAxis(0, JoyAxis.RightY)
                );

                if (analogInput.Length() > deadzone)
                {
                    HandleCameraMovement(analogInput.X * sensitivity, analogInput.Y * sensitivity);
                }
            }
        }
    }

    private void HandleCameraMovement(float deltaX, float deltaY)
    {
        RotateY(-deltaX / 1000 * Sensitivity);
        Node3D camera = GetNode<Node3D>("CameraMount");
        float newRotationX = camera.Rotation.X - deltaY / 1000 * Sensitivity;
        newRotationX = Mathf.Clamp(newRotationX, VerticalClampMin, VerticalClampMax);
        camera.Rotation = new Vector3(newRotationX, camera.Rotation.Y, camera.Rotation.Z);
    }

}
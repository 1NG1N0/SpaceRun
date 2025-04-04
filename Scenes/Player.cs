using Godot;
using System;
public partial class Player : Area2D
{
    [Export]
    public int Speed = 300;
    private Vector2 direction = Vector2.Zero;
    private CollisionShape2D collisionrect;
    private float startBound = 0.0f;
    private float endBound = 0.0f;
    private float boundingSizeX = 0.0f;
    private AnimationPlayer animationPlayer;
    [Signal]
    public delegate void PlayerDestroyedEventHandler();

    public override void _Ready()
    {
       CollisionShape2D collisionrect = GetNode<CollisionShape2D>("CollisionShape2D");
       animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer"); 
       animationPlayer.AnimationFinished += OnAnimationFinished;
       var viewportRect = GetViewportRect();
       var camera = GetViewport().GetCamera2D();
       var camera_position = camera.GlobalPosition;
       startBound = (camera_position.X - viewportRect.Size.X / 2);
        endBound = (camera_position.X + viewportRect.Size.X / 2);
       boundingSizeX = collisionrect.Shape.GetRect().Size.X;
    }

    public override void _Process(double delta)
    {
        float input = (Input.IsActionPressed("esquerda") ? -1.0f : 0.0f) + (Input.IsActionPressed("direita") ? 1.0f : 0.0f);
        //GD.Print($"Input: {input}");
        if (input > 0)
        {
            direction = Vector2.Right;
        }
        else if (input < 0)
        {
            direction = Vector2.Left;
        }
        else
        {
            direction = Vector2.Zero;
        }
        //GD.Print($"Direction: {direction}");
        float delta_movement = Speed * (float)delta * direction.X;
        //GD.Print($"Delta Movement: {delta_movement}");
        float newPositionX = Position.X + delta_movement;
        if (newPositionX < startBound + boundingSizeX * Scale.X || newPositionX > endBound - boundingSizeX * Scale.X)
        {
            return;
        }

        // Aplicando o movimento
        Position = new Vector2(newPositionX, Position.Y);
    }

    public void On_player_destroyed()
    {
        Speed = 0;
        animationPlayer.Play("destroy");
    }    

    public async void OnAnimationFinished(StringName animName)
    {
        GD.Print($"Animation finished: {animName}");
        if (animName == "destroy")
        {
            var timer = GetTree().CreateTimer(1);
            await ToSignal(timer, "timeout");
            EmitSignal(nameof(PlayerDestroyed));
            QueueFree();
        }
    }

}





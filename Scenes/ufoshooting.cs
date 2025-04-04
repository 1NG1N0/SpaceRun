using Godot;
using System;

public partial class ufoshooting : Node2D
{
    private SpawnTimer Spawn_Timer;
    private readonly PackedScene projectile = GD.Load<PackedScene>("res://Scenes/invader_shoot.tscn");

    public override void _Ready()
    {
        GD.Randomize();
        Spawn_Timer = GetNode<SpawnTimer>("SpawnTimer");
        Spawn_Timer.setup_timer();
        Spawn_Timer.Timeout += SpawnTimeout;
    }
    public void SpawnTimeout()
    {
        var projectile_instance = projectile.Instantiate<InvaderShoot>();
        GD.Print("chutei");
        projectile_instance.GlobalPosition = GlobalPosition;
        var spriteInvader = projectile_instance.GetNode<Sprite2D>("Sprite2D");
        spriteInvader.Modulate = new Color(0.67f, 0.2f, 0.2f, 1f); // Red color
        GetTree().Root.AddChild(projectile_instance);
        Spawn_Timer.setup_timer();
    }
}

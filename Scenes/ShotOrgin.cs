using Godot;
using System;

public partial class ShotOrgin : Node2D
{
    [Export]
    public PackedScene LaserScene;
    public bool isShooting = true;
    private Timer _shootimer;

    public override void _Ready()
    {
        _shootimer = new Timer();
        _shootimer.WaitTime = 0.5f;
        _shootimer.OneShot = true;
        _shootimer.Timeout += () => isShooting = true;
        AddChild(_shootimer);
    }
    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionPressed("tiro") && isShooting)
        {
            isShooting = false;
            var laser = LaserScene.Instantiate<Area2D>();
            laser.GlobalPosition = ((Area2D)GetParent()).GlobalPosition - new Vector2(0, 20);
            GetTree().Root.GetNode("Node2D").AddChild(laser);
            _shootimer.Start();
        }
    }

}

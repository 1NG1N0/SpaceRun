using Godot;
using System;


public partial class Laser : Area2D
{
    [Export]
    public int Speed = 300;
    public override void _Process(double delta)
    {
        Position = new Vector2(Position.X, Position.Y - Speed * (float)delta);
    }
}

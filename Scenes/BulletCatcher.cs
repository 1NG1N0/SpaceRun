using Godot;
using System;

public partial class BulletCatcher : Area2D
{
    
    public override void _Ready()
    {
        // Connect the area entered signal to the OnAreaEntered method
       Connect("area_entered", new Callable(this, nameof(OnAreaEntered)));
    }
    private void OnAreaEntered(Area2D area)
    {
        if (area is Laser)
        {
            GD.Print("Laser hit the catcher!");
            area.QueueFree();
        }
    }
}

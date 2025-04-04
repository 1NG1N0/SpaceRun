using Godot;
using System;

public partial class invader_config : Resource
{
    [Export]
    public Texture2D Sprite_1 { get; set; }
    [Export]
    public Texture2D Sprite_2;
    [Export]
    public int width;
    [Export]
    public string animation_name { get; set; }
    [Export]
    public int points;
}

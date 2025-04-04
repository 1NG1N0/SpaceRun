using Godot;
using System;

public partial class StartScreen : CanvasLayer
{
    private TextureRect Textura_1;
    private Label Label1;
    private TextureRect Texture2;
    private Label Label2;
    private TextureRect TextureRect;
    private Label Label3;
    private Godot.Collections.Array Con_array = new Godot.Collections.Array();
    private Timer timer;
    public override void _Ready()
    {
        Textura_1 = GetNode<TextureRect>("%Textura1");
        Texture2 = GetNode<TextureRect>("%Texture2");
        TextureRect = GetNode<TextureRect>("%TextureRect3"); 
        Label1 = GetNode<Label>("%Label1");
        Label2 = GetNode<Label>("%Label2");
        Label3 = GetNode<Label>("%Label3");
        Button StartButton = GetNode<Button>("MarginContainer/VBoxContainer/VBoxContainer2/Button");
        StartButton.Pressed += OnStartButtonPressed;

        Con_array.Add(Textura_1);
        Con_array.Add(Label1);
        Con_array.Add(Texture2);
        Con_array.Add(Label2);
        Con_array.Add(TextureRect);
        Con_array.Add(Label3);

        timer = GetNode<Timer>("Timer");
        timer.Timeout += _on_Timer_timeout;
        foreach (Node elemenr in Con_array)
        {
            if (elemenr is Control control)
            {
                control.Modulate = Colors.Transparent;
            }
        }
    }

    public void OnStartButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://Scenes/Main.tscn");
    }
    public void ShowNextControl()
    {
        
    }

    private void _on_Timer_timeout()
    {
        var node = Con_array.Count > 0 ? Con_array[0].As<Control>() : null;

        if (Con_array.Count > 0)
        {
            Con_array.RemoveAt(0);
        }

        if (node != null)
        {
            // Tornando o nó visível
            node.Modulate = Colors.White;
            node.Visible = true;
        }
        else
        {
            // Parando e liberando o timer
            timer.Stop();
            timer.QueueFree();
        }
    }
}

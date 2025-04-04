using Godot;
using System;
using System.Linq;

public partial class InvaderSpawn : Node2D
{
    //Spawner Configuration
    const int Rows = 5;
    const int Columns = 11;
    const int Horizontal_Spacing = 32;
    const int Vertical_Spacing = 32;
    const int invader_heigth = 24;
    const int Start_y_position = -250;
    const int Start_x_position_increment = 10;
    const int Start_y_position_increment = 20;
    
    public int movement_direction = 1;


    //NODE REFERENCES
    private readonly invader_config invader_1_res = GD.Load<invader_config>("res://Resources/invader_1.tres");
    private readonly invader_config invader_2_res = GD.Load<invader_config>("res://Resources/invader_2.tres");
    private readonly invader_config invader_3_res = GD.Load<invader_config>("res://Resources/invader_3.tres");
    private readonly PackedScene invader_scene = GD.Load<PackedScene>("res://Scenes/invader.tscn");

    private Area2D LeftWall;
    private Area2D RightWall;
    private Area2D BottomWall;
    private Timer movement_timer;
    private Timer shoot_timer;
    private readonly PackedScene invader_shoot_scene = GD.Load<PackedScene>("res://Scenes/invader_shoot.tscn");
    private int invaderDestroiyed_count = 0;
    private int invadertotal_counts = Rows * Columns;
    [Signal]
    public delegate void InvaderDestroyedEventHandler(int points);
    [Signal]
    public delegate void game_wonEventHandler();
    [Signal]
    public delegate void game_lostEventHandler();

    public override void _Ready()
    {
        movement_timer = GetNode<Timer>("MovementTimer");
        shoot_timer = GetNode<Timer>("ShootTimer");
        LeftWall = GetNode<Area2D>("../Walls/LeftWall");
        RightWall = GetNode<Area2D>("../Walls/RightWall");
        BottomWall = GetNode<Area2D>("../Walls/BottomWall");
        // GD.Print($"Colisão detectada com: {body.Name}");
        // GD.Print($"Colisão detectada com: {body.Name}");
        // GD.Print($"Colisão detectada com: {body.Name}");
        // GD.Print("Game Over!");
        // GD.Print("Game Won!");
        invader_config invaderconfiguration;
        movement_timer.Timeout += move_invader;
        shoot_timer.Timeout += invader_shooot;
        for (int row = 0; row < Rows; row++)
        {
            // Define a configuração do invasor com base na linha
            if (row == 0)
            {
                invaderconfiguration = invader_1_res;
            }
            else if (row == 1 || row == 2)
            {
                invaderconfiguration = invader_2_res;
            }
            else if (row == 3 || row == 4)
            {
                invaderconfiguration = invader_3_res;
            }
            else
            {
                continue;
            }

            float rowWidth = (Columns * invaderconfiguration.width * 3) + ((Columns - 1) * Horizontal_Spacing);

            // Calcula a posição inicial X para centralizar os invasores
            float startXPosition = (Position.X - rowWidth) / 2;

            for (int col = 0; col < Columns; col++)
            {
                // Calcula as posições X e Y com espaçamento
                float x = startXPosition + (col * invaderconfiguration.width * 3) + (col * Horizontal_Spacing);
                float y = Start_y_position + (row * invader_heigth) + (row * Vertical_Spacing);

                Vector2 spawnPosition = new Vector2(x, y);

                // Instancia o invasor
                SpawnInvader(invaderconfiguration, spawnPosition);
            }
        }

        if (LeftWall != null)
            LeftWall.BodyEntered += _on_left_wall_area_entered;
        if (RightWall != null)
            RightWall.BodyEntered += _on_rigth_wall_area_entered;
        if (BottomWall != null)
            BottomWall.BodyEntered += _on_bottom_wall_area_entered;
    }

    public void SpawnInvader(invader_config invaderConfig, Vector2 spawnPosition)
    {
        var invader = invader_scene.Instantiate<Invader>();
        if (invader == null)
        {
            //GD.PrintErr("Failed to instantiate Invader!");
            return;
        }

        AddChild(invader); // Adicione o invasor à árvore antes de configurar
        invader.GlobalPosition = spawnPosition;
        invader.Invaderdestruido += Invasordestr;
        invader.SetConfig(invaderConfig); // Configure o invasor
    }

    private void _on_left_wall_area_entered(Node body)
    {
        //GD.Print($"Colisão detectada com: {body.Name}");
        if (movement_direction == -1)
        {
            Position += new Vector2(0, Start_y_position_increment);
            movement_direction *= -1;
        }
    }
    private void _on_rigth_wall_area_entered(Node body)
    {
        //GD.Print($"Colisão detectada com: {body.Name}");
        if (movement_direction == 1)
        {
            Position += new Vector2(0, Start_y_position_increment);
            movement_direction *= -1;
        }
    }
    private void _on_bottom_wall_area_entered(Node body)
    {
        //GD.Print($"Colisão detectada com: {body.Name}");
        if (body is Invader)
        {
            //GD.Print("Game Over!");
            EmitSignal(nameof(game_lost));
            shoot_timer.Stop();
            movement_timer.Stop();
            movement_direction = 0;
        }
    }

    private void move_invader()
    {
        Position += new Vector2(Start_x_position_increment * movement_direction, 0);
    }

    private void invader_shooot()
    {
        var random_child_position = GetChildren().OfType<Invader>().Select(Invader => Invader.GlobalPosition).OrderBy(_ => GD.Randi()).FirstOrDefault();
        var invaderchute = invader_shoot_scene.Instantiate<InvaderShoot>();
        invaderchute.GlobalPosition = random_child_position;
        GetTree().Root.AddChild(invaderchute);
    }

    private void Invasordestr(int points)
    {
        EmitSignal(nameof(InvaderDestroyed), points);
        invaderDestroiyed_count++;
        if (invaderDestroiyed_count == invadertotal_counts)
        {
            //GD.Print("Game Won!");
            EmitSignal(nameof(game_won));
            shoot_timer.Stop();
            movement_timer.Stop();
        }
    }
}

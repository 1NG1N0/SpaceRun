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
    const int Start_x_position_increment = 2;
    const int Start_y_position_increment = 10;
    
    public int movement_direction = 1;

    public enum PhaseType
    {
        EliminateAllInvaders,       // Eliminar todos os invasores
        SurviveForTime,             // Sobreviver por um tempo determinado
        EliminateSpecificInvaders,  // Eliminar tipos específicos de invasores
        EliminateAllBeforeTimeout,  // Eliminar todos antes do tempo acabar
        ProtectBankers              // Proteger os bunkers
    }

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
    [Export]
    public PhaseType CurrentPhaseType { get; set; } = PhaseType.EliminateAllInvaders;
    private Bunker_park[] Bunkers;

    public override void _Ready()
    {
        invadertotal_counts = Rows * Columns;
        GD.Print($"Total de invasores esperados: {invadertotal_counts}");
        movement_timer = GetNode<Timer>("MovementTimer");
        shoot_timer = GetNode<Timer>("ShootTimer");
        LeftWall = GetNode<Area2D>("../Walls/LeftWall");
        RightWall = GetNode<Area2D>("../Walls/RightWall");
        BottomWall = GetNode<Area2D>("../Walls/BottomWall");

        // Configurar comportamento com base no tipo de fase
        switch (CurrentPhaseType)
        {
            case PhaseType.EliminateAllInvaders:
                GD.Print("Fase: Eliminar todos os invasores");
                break;

            case PhaseType.SurviveForTime:
                GD.Print("Fase: Sobreviver por tempo");
                shoot_timer.WaitTime = 0.8f; // Ajustar velocidade de tiro, por exemplo
                StartSurvivalTimer(300); // 300 segundos = 5 minutos
                break;

            case PhaseType.EliminateSpecificInvaders:
                GD.Print("Fase: Eliminar tipos específicos de invasores");
                break;

            case PhaseType.EliminateAllBeforeTimeout:
                GD.Print("Fase: Eliminar todos antes do tempo acabar");
                StartTimeoutTimer(300); // 300 segundos = 5 minutos
                break;

            case PhaseType.ProtectBankers:
                GD.Print("Fase: Proteger os bunkers");
                break;
        }

        movement_timer.Timeout += move_invader;
        shoot_timer.Timeout += invader_shooot;

        // Configurar os invasores
        for (int row = 0; row < Rows; row++)
        {
            invader_config invaderconfiguration = GetInvaderConfigForRow(row);

            float rowWidth = (Columns * invaderconfiguration.width * 3) + ((Columns - 1) * Horizontal_Spacing);
            float startXPosition = (Position.X - rowWidth) / 2;

            for (int col = 0; col < Columns; col++)
            {
                float x = startXPosition + (col * invaderconfiguration.width * 3) + (col * Horizontal_Spacing);
                float y = Start_y_position + (row * invader_heigth) + (row * Vertical_Spacing);

                Vector2 spawnPosition = new Vector2(x, y);
                SpawnInvader(invaderconfiguration, spawnPosition);
            }
        }

        if (LeftWall != null)
            LeftWall.BodyEntered += _on_left_wall_area_entered;
        if (RightWall != null)
            RightWall.BodyEntered += _on_rigth_wall_area_entered;
        if (BottomWall != null)
            BottomWall.BodyEntered += _on_bottom_wall_area_entered;

        // Inicializar os bunkers
        Bunkers = GetTree().GetNodesInGroup("Bunkers").OfType<Bunker_park>().ToArray();

        foreach (var bunker in Bunkers)
        {
            bunker.Connect(nameof(Bunker_park.BunkerDestroyed), new Callable(this, nameof(OnBunkerDestroyed)));
        }
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
        if (body is Invader invader)
        {
            //GD.Print("Game Over!");
            invader.DestroyInvader(0);
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
        invaderDestroiyed_count++;
        EmitSignal(nameof(InvaderDestroyed), points);

        switch (CurrentPhaseType)
        {
            case PhaseType.EliminateAllInvaders:
                if (invaderDestroiyed_count == invadertotal_counts)
                {
                    GD.Print("Todos os invasores destruídos. Vitória!");
                    EmitSignal(nameof(game_won));
                    shoot_timer.Stop();
                    movement_timer.Stop();
                }
                break;

            case PhaseType.EliminateSpecificInvaders:
                // Adicione lógica para verificar tipos específicos de invasores
                break;

            case PhaseType.EliminateAllBeforeTimeout:
                if (invaderDestroiyed_count == invadertotal_counts)
                {
                    GD.Print("Todos os invasores destruídos antes do tempo. Vitória!");
                    EmitSignal(nameof(game_won));
                    shoot_timer.Stop();
                    movement_timer.Stop();
                }
                break;
        }
    }

    private void StartSurvivalTimer(float duration)
    {
        var survivalTimer = new Timer();
        AddChild(survivalTimer);
        survivalTimer.WaitTime = duration;
        survivalTimer.OneShot = true;
        survivalTimer.Timeout += () =>
        {
            //GD.Print("Survival time ended!");
            EmitSignal(nameof(game_lost));
            shoot_timer.Stop();
            movement_timer.Stop();
        };
        survivalTimer.Start();
    }
    private void StartTimeoutTimer(float duration)
    {
        var timeoutTimer = new Timer();
        AddChild(timeoutTimer);
        timeoutTimer.WaitTime = duration;
        timeoutTimer.OneShot = true;
        timeoutTimer.Timeout += () =>
        {
            //GD.Print("Timeout time ended!");
            EmitSignal(nameof(game_lost));
            shoot_timer.Stop();
            movement_timer.Stop();
        };
        timeoutTimer.Start();
    }
    private invader_config GetInvaderConfigForRow(int row)
    {
        switch (row)
        {
            case 0:
                return invader_1_res;
            case 1:
                return invader_2_res;
            case 2:
                return invader_3_res;
            default:
                return invader_1_res; // Default to the first type if row is out of range
        }
    }
    private void OnBunkerDestroyed()
    {
        GD.Print("Sinal BunkerDestroyed recebido!");
        if (CurrentPhaseType == PhaseType.ProtectBankers)
        {
            GD.Print("Um bunker foi destruído! Fim da partida.");
            EmitSignal(nameof(game_lost)); // Emite o sinal de derrota
            shoot_timer.Stop();
            movement_timer.Stop();
        }
    }
}

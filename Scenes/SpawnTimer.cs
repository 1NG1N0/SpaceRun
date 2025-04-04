using Godot;
using System;
// Removed [Godot.ClassName("SpawnTimer")] as it caused a compile error

[Tool]
public partial class SpawnTimer : Timer
{
    [Export]
    public int min_time = 5;
    [Export]
    public int max_time = 10;
    private bool isGameOver = false;

    public override void _Ready()
    {
        setup_timer();
        Timeout += setup_timer;
    }

    public void setup_timer()
    {
        if (isGameOver)
        {
            Stop(); // Não reinicia o timer se o jogo acabou
            return;
        }
        var random_time = (float)GD.RandRange(min_time, max_time);
        //GD.Print($"Novo tempo gerado para SpawnTimer: {random_time} segundos");
        WaitTime = (float)random_time;
        
        Stop();
        Start();
    }

    public void OnTimeout()
    {
        // Aqui você pode adicionar a lógica para spawnar UFOs ou outros objetos
        //GD.Print("Timeout do SpawnTimer atingido!");

        // Reinicia o timer se o jogo ainda estiver ativo
        setup_timer();
    }
    public void OnGameOver()
    {
        //GD.Print("Jogo terminou! Pausando SpawnTimer.");
        isGameOver = true;
        Stop();
    }
    public void OnRestartGame()
    {
        //GD.Print("Jogo reiniciado! Resetando SpawnTimer.");
        isGameOver = false;
        setup_timer();
    }
}


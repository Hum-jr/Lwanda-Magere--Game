using Godot;
using System;

public partial class Menu : Control
{
    private Button NewGame;
    private Button Exit;

    public override void _Ready()
    {
        NewGame = GetNode<Button>("New");
        Exit = GetNode<Button>("Exit");

        
        NewGame.Pressed += OnNewGamePressed;
        Exit.Pressed += OnExitPressed;
    }



    private void OnNewGamePressed()
    {
        GetTree().ChangeSceneToFile("res://scenes/test_scene.tscn");
    }

    private void OnExitPressed()
    {
        GetTree().Quit();
    }
}
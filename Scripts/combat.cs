using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public partial class combat : Node
{
	[Export]
	public int max_health;

	[Export]
	public int attack;

	public int health;

	[Export]
	public Node engine { get; set; }

	private card_engine cardEngine;

	[Export]
	public RichTextLabel attackDisp { get; set; }

	[Export]
	public RichTextLabel healthDisp { get; set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		cardEngine = engine as card_engine;
		health = max_health;
		UpdateUI();
	}

	private void UpdateUI()
	{
		healthDisp.Text = $"{health}";
		attackDisp.Text = $"{attack}";
	}

	public void ChangeHealth(Func<int, int> change)
	{
		health = change(health);
		if (health > max_health)
		{
			health = max_health;
		}
		UpdateUI();
	}

	public bool Damage(Func<int, int> change)
	{
		var damaged = change(health);
		bool dealt = damaged < health;
		if (dealt)
		{
			health = damaged;
		}
		Debug.Print($"new health: {health}");
		UpdateUI();
		return dealt;
	}

	public void ChangeAttack(Func<int, int> change)
	{
		attack = change(attack);
		UpdateUI();
	}

	public void ChangeMaxHealh(Func<int, int> change) 
	{
		max_health = change(max_health);
		UpdateUI();
	}

	//make sure something can't be killed twice
	public bool Died()
	{
		return health <= 0;
	}
}

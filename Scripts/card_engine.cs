using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class card_engine : Node
{
	private Stack<Action> ActionsToAdd;

	private Stack<Action> Blocks;

	private bool processing;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ActionsToAdd= new Stack<Action>();
		Blocks = new Stack<Action>();
		processing = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void ProcessActions()
	{
		if (processing)
		{
			return;
		}
		processing = true;
		while (ActionsToAdd.TryPop(out Action action))
		{
			Blocks.Push(action);
		}
		ActionsToAdd.Clear();
		while (Blocks.TryPop(out Action action))
		{
			action.Invoke();
			if (ActionsToAdd.Count > 0)
			{
				Stack<Action> stack = new Stack<Action>();
				while (ActionsToAdd.TryPop(out Action nextAction))
				{
					stack.Push(nextAction);
				}
				ActionsToAdd.Clear();
				Blocks.Push(() =>
				{
					while (stack.TryPop(out Action nextAction))
					{
						nextAction.Invoke();
					}
				});
			}
		}
		Blocks.Clear();
		processing = false;
	}

	public void AddAction(Action action)
	{
		ActionsToAdd.Push(action);
	}

	public void AddInteraction(card_interaction targeter, card_interaction target)
	{
		// will have to make a variety of cases depending on targeter and target

		// minion attack minion case
		combat attacker = targeter.getCombat();
		combat defender = target.getCombat();
		AddAction(() => { Debug.Print("OnCardAttack"); EmitSignal(SignalName.OnCardAttack, targeter, target); });
		AddAction(() => { Debug.Print("OnCardAttacked"); EmitSignal(SignalName.OnCardAttacked, targeter, target); });
		AddAction(() => 
		{
			Debug.Print("Defender damage");
			// this should be a responsibility of combat class
			bool dDamaged = defender.Damage((int x) => { return x - attacker.attack; });
			if (dDamaged)
			{
				EmitSignal(SignalName.OnCardDamaged, targeter, target);
			}
		});
		AddAction(() =>
		{
			Debug.Print("Attacker damage");
			// this should be a responsibility of combat class
			bool aDamaged = attacker.Damage((int x) => { return x - defender.attack; });
			if (aDamaged)
			{
				EmitSignal(SignalName.OnCardDamaged, target, targeter);
			}
		});
		AddAction(() => { Debug.Print("AfterCardAttack"); EmitSignal(SignalName.AfterCardAttack, targeter, target);  });
		AddAction(() => { Debug.Print("AfterCardAttacked"); EmitSignal(SignalName.AfterCardAttacked, targeter, target); });

		AddAction(() =>
		{
			Debug.Print("Defender died?");
			if (defender.Died())
			{
				Debug.Print("Defender died ):");
				EmitSignal(SignalName.OnCardDied, targeter, target);
			}
		});
		AddAction(() =>
		{
			Debug.Print("Attacker died?");
			if (attacker.Died())
			{
				Debug.Print("Attacker died ):");
				EmitSignal(SignalName.OnCardDied, target, targeter);
			}
		});
	}


	[Signal]
	public delegate void OnCardAttackEventHandler(card_interaction attacker, card_interaction defender);

	[Signal]
	public delegate void OnCardAttackedEventHandler(card_interaction attacker, card_interaction defender);

	[Signal]
	public delegate void OnCardDamagedEventHandler(card_interaction damager, card_interaction damaged);

	[Signal]
	public delegate void OnCardDiedEventHandler(card_interaction killer, card_interaction killed);

	[Signal]
	public delegate void AfterCardAttackEventHandler(card_interaction attacker, card_interaction defender);

	[Signal]
	public delegate void AfterCardAttackedEventHandler(card_interaction attacker, card_interaction defender);
}

using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

public partial class game_manager : Node2D
{
	private Area2D selected_card;
	private Area2D hover_card;
	private List<Area2D> cards;
	private card_engine CardEngine;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CardEngine = GetNode<Node>("Card_Engine") as card_engine;
		selected_card = null;
		hover_card = null;
		cards = new List<Area2D>();
		cards.Add(GetNode<Area2D>("Card_Engine/Card"));
		cards.Add(GetNode<Area2D>("Card_Engine/Card2"));
		foreach (var card in cards)
		{
			card.MouseEntered += () => OnCardEnter(card);
			card.MouseExited += () => OnCardExit(card);
			card.Connect("CardSelected", new Callable(this, MethodName.OnCardSelect));
			card.Connect("CardDeselected", new Callable(this, MethodName.OnCardDeselect));
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (hover_card != null && selected_card != null && hover_card == selected_card)
		{
			Debug.Print("ruh roh");
		}
	}

	private void OnCardEnter(Area2D card)
	{
		if (selected_card == null || !selected_card.Equals(card))
		{
			hover_card = card;
			card.Scale = new Vector2(1.2f, 1.2f);
		}
	}

	private void OnCardExit(Area2D card)
	{
		if (hover_card == null || hover_card.Equals(card))
		{
			card.Scale = new Vector2(1, 1);
			hover_card = null;
		}
	}

	private void OnCardSelect(Area2D card)
	{
		if (hover_card == null || hover_card.Equals(card))
		{
			hover_card = null;
		}
		card.Scale = new Vector2(1.5f, 1.5f);
		selected_card = card;
	}

	private void OnCardDeselect(Area2D card)
	{
		if (hover_card != null && selected_card != null && card == selected_card)
		{
			card_interaction targeter = selected_card as card_interaction;
			card_interaction target = hover_card as card_interaction;
			CardEngine.AddInteraction(targeter, target);
			CardEngine.ProcessActions();
		}
		card.Scale = new Vector2(1, 1);
		selected_card = null;
	}
}

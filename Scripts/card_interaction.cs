using Godot;
using System;
using System.Diagnostics;

public partial class card_interaction : Area2D
{
	private bool dragging;
	private Vector2 origin;
	private int default_z;
	private combat Combat;

	[Export]
	public Sprite2D sprite { get; set; }

	[Export]
	public Node combatNode { get; set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		dragging = false;
		origin = this.Position;
		default_z = sprite.ZIndex;
		Combat = combatNode as combat;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (dragging)
		{
			this.Position = GetGlobalMousePosition();
		}
	}

	public combat getCombat()
	{
		return Combat;
	}

	[Signal]
	public delegate void CardSelectedEventHandler(Area2D card);

	[Signal]
	public delegate void CardDeselectedEventHandler(Area2D card);

	public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx)
	{
		if (@event is InputEventMouseButton)
		{
			InputEventMouseButton click = (@event as InputEventMouseButton);
			if (click.ButtonIndex == MouseButton.Left && click.Pressed) 
			{ 
				origin = this.Position;
				dragging = true;
				sprite.ZIndex = 100;
				EmitSignal(SignalName.CardSelected, this);
			}
			else if (click.ButtonIndex == MouseButton.Left && !click.Pressed && dragging)
			{
				dragging = false;
				this.Position = origin;
				sprite.ZIndex = default_z;
				EmitSignal(SignalName.CardDeselected, this);
			}
		}
	}
}

using UnityEngine;
using System.Collections;

public class FastMovementDecorator : PlayerControllerDecorator {
	public FastMovementDecorator(IPlayerController playerController) : base(playerController) { }

	public override float speedScalar{
		get {
			return 4;
		}

		set {
			base.speedScalar = value;
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAgent : BaseAgent {

	public Vector2 turnOffsetRange = new Vector2(-.1f, .1f);
	public Vector2 forwardOffsetRange = new Vector2(-.1f, .1f);

	[SerializeField] private BasicEngine engine = null;

	private void Start() {
		if(engine == null) {
			engine = ship.GetSystem<BasicEngine>();
		}
	}

	private void Update() {
		engine.turnThrottle = Mathf.Clamp(engine.turnThrottle + Random.Range(turnOffsetRange.x, turnOffsetRange.y) * Time.deltaTime, -1f, 1f);
		engine.forwardThrottle = Mathf.Clamp(engine.forwardThrottle + Random.Range(forwardOffsetRange.x, forwardOffsetRange.y) * Time.deltaTime, 0f, 1f);
	}

}

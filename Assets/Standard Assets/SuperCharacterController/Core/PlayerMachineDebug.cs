using UnityEngine;

public class PlayerMachineDebug : MonoBehaviour
{
    [SerializeField]
    private PlayerMachine playerMachine;

    private float timeScale = 1.0f;

    void Awake()
    {
        if (playerMachine == null)
        {
            playerMachine = GetComponent<PlayerMachine>();
        }
    }

	void OnGUI()
	{
	    GUI.Box(new Rect(10, 10, 200, 100), "Player Machine"); // x, y, x max, y max

        GUI.TextField(new Rect(20, 40, 180, 20), string.Format("State: {0}", playerMachine.currentState));
        timeScale = GUI.HorizontalSlider(new Rect(20, 70, 180, 20), timeScale, 0.0f, 1.0f);
		GUI.TextField (new Rect(20,80,180,20), string.Format("Look Direction: {0}", playerMachine.moveDirection));
        Time.timeScale = timeScale;
		GUI.TextField(new Rect(20, 120, 180, 20), string.Format("{0}", Time.deltaTime));
		GUI.TextField(new Rect(20, 160, 180, 20), string.Format("{0}", playerMachine.inputDecay));
		GUI.TextField(new Rect(20, 180, 180, 20), string.Format("{0}", playerMachine.Gravity));
	}
}

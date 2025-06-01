using UnityEngine;

public class PlayerMachineDebug : MonoBehaviour
{
    [SerializeField]
    private PlayerMachine playerMachine;

    private float timeScale = 1.0f;

    void Awake()
    {
        if(playerMachine == null)
        {
            playerMachine = GetComponent<PlayerMachine>();
        }
    }

	void OnGUI()
	{
	    GUI.Box(new Rect(10, 10, 200, 100), "Player Machine");

        GUI.TextField(new Rect(20, 40, 180, 20), string.Format("State: {0}", playerMachine.currentState));
        timeScale = GUI.HorizontalSlider(new Rect(20, 70, 180, 20), timeScale, 0.0f, 1.0f);

        Time.timeScale = timeScale;
	}
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlagueIntroSceneLoader : MonoBehaviour {
	private void Start() {
		SceneManager.GetSceneByName("PlagueIntro_Riot");
		if (false)
			SceneManager.LoadScene("PlagueIntro_Riot", LoadSceneMode.Additive);
		SceneManager.GetSceneByName("PlagueIntro_FlamethrowerTest");
		if (false)
			SceneManager.LoadScene("PlagueIntro_FlamethrowerTest", LoadSceneMode.Additive);
		SceneManager.GetSceneByName("PlagueIntro_Rats");
		if (false)
			SceneManager.LoadScene("PlagueIntro_Rats", LoadSceneMode.Additive);
		SceneManager.GetSceneByName("PlagueIntro_Flamethrowers");
		if (true)
			return;
		SceneManager.LoadScene("PlagueIntro_Flamethrowers", LoadSceneMode.Additive);
	}
}
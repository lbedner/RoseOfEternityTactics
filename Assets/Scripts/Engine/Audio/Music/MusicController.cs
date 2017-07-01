using UnityEngine;
using System.Collections;

public class MusicController : MonoBehaviour {

	public AudioSource musicCalm;
	public AudioSource musicFire;

	private bool isMusicCalm = false;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start() {
		Initialize ();
	}

	/// <summary>
	/// Initialize the music for the level.
	/// </summary>
	public void Initialize() {
		
		musicCalm.Play();
		isMusicCalm = true;
		if (musicFire != null) {
			musicFire.Play ();
			musicFire.volume = 0.0f;
		}
	}

	/// <summary>
	/// Stops all music.
	/// </summary>
	public void StopAllMusic() {
		musicCalm.Stop ();
		musicFire.Stop ();
	}
		
	/// <summary>
	/// Transitions the music from calm to fire (or vice versa).
	/// If there is no fire version of the song, bail out.
	/// <param name="IsTransitionCalm">Determines what type of track to transition to.</param>
	/// </summary>
	public void TransitionMusic(bool isTransitionCalm) {

		// Bail out if there is nothing to transition to or already on that type of music
		if (musicFire == null  || (isTransitionCalm && isMusicCalm) || (!isTransitionCalm && !isMusicCalm))
			return;
		
		AudioSource audioSourceStop;
		AudioSource audioSourceStart;

		if (isMusicCalm) {
			audioSourceStop = musicCalm;
			audioSourceStart = musicFire;
			isMusicCalm = false;
		}
		else {
			audioSourceStop = musicFire;
			audioSourceStart = musicCalm;
			isMusicCalm = true;
		}
		StartCoroutine (FadeOutMusic (audioSourceStop));
		StartCoroutine (FadeInMusic (audioSourceStart));
	}

	/// <summary>
	/// Lowers the combat music.
	/// </summary>
	public IEnumerator LowerCombatMusic() {
		AudioSource audioSource;
		if (isMusicCalm)
			audioSource = musicCalm;
		else
			audioSource = musicFire;
		yield return StartCoroutine (LowerMusic (audioSource));
	}

	/// <summary>
	/// Raises the combat music.
	/// </summary>
	public IEnumerator RaiseCombatMusic() {
		AudioSource audioSource;
		if (isMusicCalm)
			audioSource = musicCalm;
		else
			audioSource = musicFire;
		yield return StartCoroutine (RaiseMusic (audioSource));
	}

	/// <summary>
	/// Lowers the music.
	/// </summary>
	/// <returns>The music.</returns>
	/// <param name="audioSource">Audio source.</param>
	/// <param name="targetVolume">Target volume.</param>
	private IEnumerator LowerMusic(AudioSource audioSource, float targetVolume = 0.5f) {
		float currentVolume = 1.0f;
		while (currentVolume > targetVolume) {
			currentVolume -= Time.deltaTime;
			audioSource.volume = currentVolume;
			yield return null;
		}				
	}

	/// <summary>
	/// Raises the music.
	/// </summary>
	/// <returns>The music.</returns>
	/// <param name="audioSource">Audio source.</param>
	/// <param name="targetVolume">Target volume.</param>
	private IEnumerator RaiseMusic(AudioSource audioSource, float targetVolume = 1.0f) {
		float currentVolume = 0.5f;
		while (currentVolume < targetVolume) {
			currentVolume += Time.deltaTime;
			audioSource.volume = currentVolume;
			yield return null;
		}				
	}

	/// <summary>
	/// Fades in the music.
	/// </summary>
	/// <returns>The in music.</returns>
	/// <param name="audioSource">Audio source.</param>
	private IEnumerator FadeInMusic(AudioSource audioSource) {
		float volume = 0.0f;
		while (volume < 1.0f) {
			volume += Time.deltaTime;
			audioSource.volume = volume;
			yield return null;
		}		
	}

	/// <summary>
	/// Fades out the music.
	/// </summary>
	/// <returns>The out music.</returns>
	/// <param name="audioSource">Audio source.</param>
	private IEnumerator FadeOutMusic(AudioSource audioSource) {
		float volume = 1.0f;
		while (volume > 0.0f) {
			volume -= Time.deltaTime;
			audioSource.volume = volume;
			yield return null;
		}
	}
}
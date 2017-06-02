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
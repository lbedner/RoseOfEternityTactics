using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicController : MonoBehaviour {

	[SerializeField]
	private AudioSource _musicCalm;
	[SerializeField]
	private AudioSource _musicFire;

	[SerializeField]
	private List<AudioSource> _playlist;

	private bool _isMusicCalm;
	private float _raisedVolume;
	private float _loweredVolume;

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
		_musicCalm = GetRandomPlaylistTrack();
		_musicCalm.Play();
		_isMusicCalm = true;
		_raisedVolume = _musicCalm.volume;
		if (_musicFire != null) {
			_musicFire.Play ();
			_musicFire.volume = 0.0f;
		}
	}

	/// <summary>
	/// Stops all music.
	/// </summary>
	public void StopAllMusic() {
		_musicCalm.Stop ();
		if (_musicFire != null)
			_musicFire.Stop ();
	}
		
	/// <summary>
	/// Transitions the music from calm to fire (or vice versa).
	/// If there is no fire version of the song, bail out.
	/// <param name="IsTransitionCalm">Determines what type of track to transition to.</param>
	/// </summary>
	public void TransitionMusic(bool isTransitionCalm) {

		// Bail out if there is nothing to transition to or already on that type of music
		if (_musicFire == null  || (isTransitionCalm && _isMusicCalm) || (!isTransitionCalm && !_isMusicCalm))
			return;
		
		AudioSource audioSourceStop;
		AudioSource audioSourceStart;

		if (_isMusicCalm) {
			audioSourceStop = _musicCalm;
			audioSourceStart = _musicFire;
			_isMusicCalm = false;
		}
		else {
			audioSourceStop = _musicFire;
			audioSourceStart = _musicCalm;
			_isMusicCalm = true;
		}
		StartCoroutine (FadeOutMusic (audioSourceStop));
		StartCoroutine (FadeInMusic (audioSourceStart));
	}

	/// <summary>
	/// Lowers the combat music.
	/// </summary>
	public IEnumerator LowerCombatMusic() {
		yield return StartCoroutine (LowerMusic (GetAudioSource()));
	}

	/// <summary>
	/// Raises the combat music.
	/// </summary>
	public IEnumerator RaiseCombatMusic() {
		yield return StartCoroutine (RaiseMusic (GetAudioSource()));
	}

	/// <summary>
	/// Lowers the music.
	/// </summary>
	/// <returns>The music.</returns>
	/// <param name="audioSource">Audio source.</param>
	/// <param name="targetVolume">Target volume.</param>
	private IEnumerator LowerMusic(AudioSource audioSource, float targetVolumePercent = 0.5f) {
		float currentVolume = _raisedVolume;
		float targetVolume = currentVolume * targetVolumePercent;
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
	private IEnumerator RaiseMusic(AudioSource audioSource) {
		float currentVolume = _loweredVolume;
		while (currentVolume < _raisedVolume) {
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

	private AudioSource GetRandomPlaylistTrack() 
	{
		int index = Random.Range(0, _playlist.Count);
		return _playlist[index];
	}

	private AudioSource GetAudioSource() 
	{
		AudioSource audioSource;
		if (_isMusicCalm)
			audioSource = _musicCalm;
		else
			audioSource = _musicFire;
		return audioSource;
	}
}
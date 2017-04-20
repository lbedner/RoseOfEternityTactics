using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader {

	private bool _isFading = false;

	/// <summary>
	/// Fade type.
	/// </summary>
	public enum FadeType {
		FADE_IN,
		FADE_OUT,
	}

	/// <summary>
	/// Fades the screen.
	/// </summary>
	/// <returns>The screen.</returns>
	/// <param name="fadeImage">Fade image.</param>
	/// <param name="fadeType">Fade type.</param>
	/// <param name="duration">Duration.</param>
	public IEnumerator FadeScreen(RawImage fadeImage, FadeType fadeType, float duration) {

		_isFading = true;

		float currentAlpha = fadeImage.color.a;
		int endAlpha;

		switch (fadeType) {

		// Fade In
		case FadeType.FADE_IN:
			currentAlpha = 1;
			endAlpha = 0;
			while (currentAlpha >= endAlpha) {
				fadeImage.color = new Color (fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, currentAlpha);
				currentAlpha += Time.deltaTime * (1.0f / duration) * -1;
				yield return null;
			}
			break;

		// Fade Out
		case FadeType.FADE_OUT:
			currentAlpha = 0;
			endAlpha = 1;
			while (currentAlpha <= endAlpha) {
				fadeImage.color = new Color (fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, currentAlpha);
				currentAlpha += Time.deltaTime * (1.0f / duration) * 1;
				yield return null;
			}
			break;
		}

		_isFading = false;
	}

	/// <summary>
	/// Determines whether this instance is fading.
	/// </summary>
	/// <returns><c>true</c> if this instance is fading; otherwise, <c>false</c>.</returns>
	public bool IsFading() {
		return _isFading;
	}
}
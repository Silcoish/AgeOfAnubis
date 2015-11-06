using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SliderAudioSFX : Slider {

	public override void OnMove(UnityEngine.EventSystems.AxisEventData eventData)
	{
		base.OnMove(eventData);

		AudioManager.Inst.SetSFXVolume(value);
	}

}

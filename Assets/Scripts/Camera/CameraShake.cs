using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{	
    [SerializeField] private float _shakeDuration = 0.3f;
	[SerializeField] private float _shakeAmount = 0.7f;
    private Vector3 _originalPos;
    private float _counter = 0.0f;
    private bool _startShake = false;

    public void SetValues(float shakeAmount, Vector3 originalPosition)
    {
		_originalPos = originalPosition;
        _startShake = true;
        _shakeAmount = shakeAmount;
        _counter = 0.0f;
    }

	void Update()
	{
		if (_startShake)
		{
            if (_counter >= _shakeDuration)
            {
                _counter = 0.0f;
                _startShake = false;
                transform.localPosition = _originalPos;
            }
            else
            {
                transform.localPosition = _originalPos + Random.insideUnitSphere * _shakeAmount;
                _counter += Time.deltaTime;
            }
        }
	}
}
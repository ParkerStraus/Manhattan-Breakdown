using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirCamStuff : MonoBehaviour
{
    private CinemachineVirtualCamera vircam;
    private CinemachineBasicMultiChannelPerlin perlinNoise;

    [SerializeField] private float Freq_Default;
    [SerializeField] private float Amp_Default;
    // Start is called before the first frame update
    void Start()
    {
        vircam = GetComponent<CinemachineVirtualCamera>();
        perlinNoise = vircam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        perlinNoise.m_AmplitudeGain = 0;
        perlinNoise.m_FrequencyGain = 0;
    }

    public void Shake(float Freq, float Amp, float TimeDecay, float TimeShake)
    {
        StopAllCoroutines();
        StartCoroutine(ShakeProtocol(Freq, Amp, TimeDecay, TimeShake, 0));
    }
    public void Shake(float Freq, float Amp, float TimeDecay, float TimeShake, float ActivateDelay)
    {
        StopAllCoroutines();
        StartCoroutine(ShakeProtocol(Freq, Amp, TimeDecay, TimeShake, ActivateDelay));
    }

    IEnumerator ShakeProtocol(float Freq, float Amp, float TimeDecay, float TimeShake, float ActivateDelay)
    {
        yield return new WaitForSeconds(ActivateDelay);

        perlinNoise.m_FrequencyGain = Freq;
        perlinNoise.m_AmplitudeGain = Amp;

        yield return new WaitForSeconds(TimeShake);

        float CurrentTime = 0;
        while (true)
        {
            CurrentTime += Time.deltaTime;
            perlinNoise.m_FrequencyGain = (Freq - Freq_Default) * (TimeDecay - CurrentTime) / TimeDecay + Freq_Default;
            perlinNoise.m_AmplitudeGain = (Amp - Amp_Default) * (TimeDecay - CurrentTime) / TimeDecay + Amp_Default;

            if (CurrentTime >= TimeDecay) break;
            else yield return null;
        }
        perlinNoise.m_FrequencyGain = Freq_Default;
        perlinNoise.m_AmplitudeGain = Amp_Default;
    }

    public void SnapToFollow()
    {
        Transform snapobj = vircam.m_Follow;
        vircam.ForceCameraPosition(snapobj.position, Quaternion.identity);
    }

    public void SetNewObject(GameObject go)
    {
        vircam.m_Follow = go.transform;
    }
}

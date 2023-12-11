using Mirror;
using UnityEngine;

public class AudioManager : NetworkBehaviour
{
    public static AudioManager Instance = null;

    public AudioSource audioSource;
    public AudioClip attackSound;


    private void Awake()
    {
        // 싱글톤 인스턴스를 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //클라이언트에서 소리를 서버로
    public void SendSoundToServer(float soundValue)
    {
         CmdSendSoundToServer(soundValue);
    }

    //서버에서 모든 클라이언트로 소리를 전달
    [ClientRpc]
    void RpcReceiveSoundOnClients(float soundValue)
    {
        audioSource.PlayOneShot(audioSource.clip, soundValue);
    }

    //클라이언트에서 소리를 서버로
    [Command(requiresAuthority = false)]
    void CmdSendSoundToServer(float soundValue)
    {
        RpcReceiveSoundOnClients(soundValue);
    }
}
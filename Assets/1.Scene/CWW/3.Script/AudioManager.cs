using Mirror;
using UnityEngine;

public class AudioManager : NetworkBehaviour
{
    public static AudioManager Instance = null;

    public AudioSource audioSource;
    public AudioClip attackSound;


    private void Awake()
    {
        // �̱��� �ν��Ͻ��� ����
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //Ŭ���̾�Ʈ���� �Ҹ��� ������
    public void SendSoundToServer(float soundValue)
    {
         CmdSendSoundToServer(soundValue);
    }

    //�������� ��� Ŭ���̾�Ʈ�� �Ҹ��� ����
    [ClientRpc]
    void RpcReceiveSoundOnClients(float soundValue)
    {
        audioSource.PlayOneShot(audioSource.clip, soundValue);
    }

    //Ŭ���̾�Ʈ���� �Ҹ��� ������
    [Command(requiresAuthority = false)]
    void CmdSendSoundToServer(float soundValue)
    {
        RpcReceiveSoundOnClients(soundValue);
    }
}
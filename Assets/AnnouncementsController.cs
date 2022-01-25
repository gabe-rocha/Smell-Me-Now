using System.Runtime.InteropServices.WindowsRuntime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnnouncementsController : MonoBehaviour
{
    public static AnnouncementsController Instance;
    [SerializeField] private GameObject announcementsGO;
    [SerializeField] private TextMeshProUGUI textAnnouncement;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        textAnnouncement.text = "";
        announcementsGO.SetActive(false);
        announcementsGO.LeanScale(Vector3.zero, 0);
    }

    public void ShowAnnouncement(string message, float duration){
        textAnnouncement.text = message;
        StartCoroutine(DisplayAnnouncementForSecs(duration));
    }

    private IEnumerator DisplayAnnouncementForSecs(float duration){
        announcementsGO.SetActive(true);
        announcementsGO.LeanScale(Vector3.one, 0.5f).setEaseOutBack();
        yield return new WaitForSeconds(duration);
        announcementsGO.LeanScale(Vector3.zero, 0.5f).setEaseInBack();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text.RegularExpressions;

[RequireComponent(typeof(TMPDialogue))]
public class EnemyDialogManager : MonoBehaviour
{
    public TMPDialogue dialogs;
    [SerializeField] private float _timeRandomDialog = 5f;
    [SerializeField] private int _probablityDialog = 2;
    [SerializeField] private float _clockDialog = 0f;
    [SerializeField] private float _durationDialog = 4.5f;

    public bool triggerDialog = false;
    private string _dialogName;
    [SerializeField] private GameObject _dialogBoxReference;

    [SerializeField] private GameObject popUpPrefab;
    [SerializeField] private GameObject dialogPrefab;

    //Alert ou Spoted : 
    // coroutine activent les dialogues
    // trigger dans la coroutine activent la réponse si y a un enemy a proximité

    private void Start()
    {
        dialogs = GetComponent<TMPDialogue>();
        var entity = transform;
        var sprite = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        dialogs.SetUpTarget(entity, new Vector3(0, sprite.bounds.size.y / 1.4f, 0));

        SetDialog("Dialog ", dialogPrefab, false);
    }

    private void Update()
    {
        if (!triggerDialog)
            return;
        _clockDialog += Time.deltaTime;
        if (_clockDialog >= _timeRandomDialog) {
            _clockDialog = 0f;
            var rd = Random.Range(0, _probablityDialog);
            //Debug.Log("rd = " + rd);
            if (rd == 0) {
                StartCoroutine(PlayRandomDialog(_dialogName, _dialogBoxReference, _durationDialog));
            }
        }
    }

    //si on veut un dialog random c bien, mais moi je veux le popUp "(!)"
    private IEnumerator PlayRandomDialog(string dialogType, GameObject dialogBox, float lifeTimeDialog)
    {
        int nbDialog = 0;
        foreach(string dialogName in dialogs.GetDialogueNames()) {
            if (dialogName.Contains(dialogType)) {
                ++nbDialog;
            }
        }
        int indexDialog = Random.Range(0, nbDialog) + 1;
        string dialogueName = dialogType + indexDialog.ToString();
        dialogs.SetDialogBox(dialogBox);
        dialogs.StartDialogue(dialogueName);
        yield return new WaitForSeconds(lifeTimeDialog);
        dialogs.StopDialogue();
    }

    //play dans les trigger qui se font qu'une seule fois -> dans enemyDetectionManager du coup
    public void SetDialog(string dialogName, GameObject dialogBox, bool instantDialog)
    {
        if (instantDialog) {
            //Debug.Log("--Instant dialog");
            StartCoroutine(PlayThisDialog(dialogName, dialogBox, _durationDialog));
        } else {
            //Debug.Log("--Sheduled dialog");
            triggerDialog = true;
            _dialogName = dialogName;
            _dialogBoxReference = dialogBox;
        }
    }

    private IEnumerator PlayThisDialog(string dialogName, GameObject dialogBox, float lifeTimeDialog)
    {
        dialogName = ReplaceWhitespace(dialogName, "");
        dialogs.SetDialogBox(dialogBox);
        dialogs.StartDialogue(dialogName);
        yield return new WaitForSeconds(lifeTimeDialog);
        dialogs.StopDialogue();
    }


    //dans eventManager on set les sprites de EnemyDialogManager
    public void ChoosDialogType(DetectionState detectionState)
    {
        //Debug.Log("Change dialog type");
        ResetDialogVariables();

        if (detectionState == DetectionState.None) {
            SetDialog("Dialog ", dialogPrefab, false);
        } else if (detectionState == DetectionState.Alert) {
            SetDialog("Alerted ", popUpPrefab, true);
            //attendre que le popUp soit finit
            //quand meme set des dialogs pas instant après
        } else if (detectionState == DetectionState.Spoted) {
            SetDialog("Spoted ", popUpPrefab, true);
            //attendre que le popUp soit finit
            //quand meme set des dialogs pas instant après
        }
    }

    private void ResetDialogVariables()
    {
        triggerDialog = false;
        _clockDialog = 0f;
        //_dialogName = "";
        //_spriteSheet = null;
    }

    private readonly Regex sWhitespace = new Regex(@"\s+");
    private string ReplaceWhitespace(string input, string replacement) 
    {
        return sWhitespace.Replace(input, replacement);
    }
}

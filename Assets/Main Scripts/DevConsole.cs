using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class DevConsole : MonoBehaviour
{
    [SerializeField] WeaponData weaponData;
    private TMP_InputField console;

    public void Start()
    {
        gameObject.SetActive(false); //hide UI

        console = transform.GetComponent<TMP_InputField>(); //get the TMP component
    }

    public void DisconnectEvents(PlayerInput playerInput)
    {
        gameObject.SetActive(false);
        console.onEndEdit.RemoveAllListeners(); //remove the listener.
        console.onDeselect.RemoveAllListeners();
        playerInput.SwitchCurrentActionMap("Player");
    }

    public void Toggle(PlayerInput playerInput)
    {
        gameObject.SetActive(true);
        console.text = ""; //Reset the text to be nothing.

        EventSystem.current.SetSelectedGameObject(null); //Deselect UI.
        console.Select(); //Focus on the text.

        playerInput.SwitchCurrentActionMap("UI"); //so players won't move.

        console.onEndEdit.AddListener((x) => {
            foreach(var obj in weaponData.Weapons)
            {
                if (obj.name.ToLower().Contains(x.ToLower()))
                {
                    GameObject weapon = Instantiate(obj, playerInput.transform.position, Quaternion.identity);
                    break;
                }
            }
            DisconnectEvents(playerInput);
        });

        console.onDeselect.AddListener(x => {DisconnectEvents(playerInput);}); //If user deselect UI, just disconnect everything.
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMarkerController : MonoBehaviour
{
    public string levelNameReference;
    private SpriteRenderer selfSprite;

    [SerializeField]
    private StateClass neutralState;
    [SerializeField]
    private StateClass hoveringState;
    [SerializeField]
    private StateClass selectedState;

    private void Start() {
        selfSprite = GetComponent<SpriteRenderer>();
        ActivateNeutral();
    }
 
    public void ActivateNeutral() {
        neutralState.AlterMarker(selfSprite);
    }

    public void ActivateHover() {
        hoveringState.AlterMarker(selfSprite);
    }

    public void ActivateSelected() {
        selectedState.AlterMarker(selfSprite);
        FindObjectOfType<TeamChooserController>().SetLevelStringReference(levelNameReference);
    }

    [System.Serializable]
    private class StateClass {
        [SerializeField]
        private Color stateColor;
        [SerializeField]
        private Sprite stateSprite;

        public void AlterMarker(SpriteRenderer objectSpriteRenderer) {
            objectSpriteRenderer.color = stateColor;
            objectSpriteRenderer.sprite = stateSprite;
        }
    }
}

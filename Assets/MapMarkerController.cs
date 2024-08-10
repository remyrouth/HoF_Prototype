using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMarkerController : MonoBehaviour
{
    [SerializeField]
    private MapLevel maplevel;





    [SerializeField]
    private StateClass neutralState;
    [SerializeField]
    private StateClass hoveringState;
    [SerializeField]
    private StateClass selectedState;


    private SpriteRenderer selfSprite;

    private void Start() {
        selfSprite = GetComponent<SpriteRenderer>();
        ActivateNeutral();
    }

    public MapLevel GiveMarkerLevel() {
        return maplevel;
    }
 
    public void ActivateNeutral() {
        neutralState.AlterMarker(selfSprite);
    }

    public void ActivateHover() {
        hoveringState.AlterMarker(selfSprite);
    }

    public void ActivateSelected() {
        selectedState.AlterMarker(selfSprite);
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

    [System.Serializable]
    public class MapLevel {
        public string levelStringName;
        public int teamMemberMax = 1;
    }
}

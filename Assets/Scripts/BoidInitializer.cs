

using BoidSim;
using System;
using UnityEngine;

internal class BoidInitializer : MonoBehaviour {
    [SerializeField] private BoidSettings[] boidSettings;

    private KeyCode[] keyCodes;

    private void Start() {
        boidSettings[0].Activate();

        int settingsLength = boidSettings.Length;

        keyCodes = new KeyCode[settingsLength];
        for (int i=0; i<settingsLength; ++i) {
            keyCodes[i] = (KeyCode)Enum.Parse(typeof(KeyCode), $"Alpha{i + 1}");
        }
    }

    private void Update() {
        for (int i = 0, length = boidSettings.Length; i < length; ++i) {
            if (Input.GetKeyDown (keyCodes[i])) {
                boidSettings[i].Activate();
            }
        }
    }
}
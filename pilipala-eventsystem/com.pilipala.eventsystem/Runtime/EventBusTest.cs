using UnityEngine;
using EventSystem;

namespace EventSystem.Tests {
    /// <summary>
    /// Example script demonstrating how to use the EventBus system.
    /// This can be used as a reference for both developers and designers.
    /// </summary>
    public class EventBusTest : MonoBehaviour {
        private float timer = 0f;
        private float interval = 10f;
        private string[] testEventsGeneric = new string[] {
            "core.system.startup.generic",
            "editor.window.opened.generic",
            "gameplay.player.spawned.generic",
            "infrastructure.network.connected.generic",
            "input.keyboard.pressed.generic",
            "presentation.ui.shown.generic",
            "custom.test.event.generic"
        };
        private string[] testEventsNonGeneric = new string[] {
            "core.system.startup.nongeneric",
            "editor.window.opened.nongeneric",
            "gameplay.player.spawned.nongeneric",
            "infrastructure.network.connected.nongeneric",
            "input.keyboard.pressed.nongeneric",
            "presentation.ui.shown.nongeneric",
            "custom.test.event.nongeneric"
        };
        private EventDomain[] testDomains = new EventDomain[] {
            EventDomain.Core,
            EventDomain.Editor,
            EventDomain.Gameplay,
            EventDomain.Infrastructure,
            EventDomain.Input,
            EventDomain.Presentation,
            EventDomain.Custom
        };
        private bool[] testResultsGeneric;
        private bool[] testResultsNonGeneric;
        private bool[] testTriggeredGeneric;
        private bool[] testTriggeredNonGeneric;

        private void Awake() {
            testResultsGeneric = new bool[testEventsGeneric.Length];
            testResultsNonGeneric = new bool[testEventsNonGeneric.Length];
            testTriggeredGeneric = new bool[testEventsGeneric.Length];
            testTriggeredNonGeneric = new bool[testEventsNonGeneric.Length];
            for (int i = 0; i < testEventsGeneric.Length; i++) {
                int idx = i;
                // Generic event
                EventBusHelper.Subscribe<string>(testEventsGeneric[i], (s) => { testResultsGeneric[idx] = s == "test"; });
                // Non-generic event
                EventBusHelper.Subscribe(testEventsNonGeneric[i], () => { testResultsNonGeneric[idx] = true; });
            }
        }

        private void Update() {
            timer += Time.deltaTime;
            if (timer >= interval) {
                timer = 0f;
                for (int i = 0; i < testEventsGeneric.Length; i++) {
                    testResultsGeneric[i] = false;
                    testResultsNonGeneric[i] = false;
                    testTriggeredGeneric[i] = false;
                    testTriggeredNonGeneric[i] = false;
                    // Trigger generic event
                    EventBusHelper.Trigger<string>(testEventsGeneric[i], "test");
                    testTriggeredGeneric[i] = true;
                    // Trigger non-generic event
                    EventBusHelper.Trigger(testEventsNonGeneric[i]);
                    testTriggeredNonGeneric[i] = true;
                }
            }
        }

        public bool[] GetTestResultsGeneric() {
            return testResultsGeneric;
        }
        public bool[] GetTestResultsNonGeneric() {
            return testResultsNonGeneric;
        }
        public bool[] GetTestTriggeredGeneric() {
            return testTriggeredGeneric;
        }
        public bool[] GetTestTriggeredNonGeneric() {
            return testTriggeredNonGeneric;
        }
        public string[] GetTestEventNamesGeneric() {
            return testEventsGeneric;
        }
        public string[] GetTestEventNamesNonGeneric() {
            return testEventsNonGeneric;
        }
    }
}
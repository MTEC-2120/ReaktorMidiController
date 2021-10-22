using UnityEngine;
using System;
using System.Collections.Generic;
using MidiJack;

namespace UnityMidiControl.Input {
	public sealed class InputManager : MonoBehaviour {
		public KeyMappings KeyMappings = new KeyMappings();
		public ControlMappings ControlMappings = new ControlMappings();

		private static InputManager _instance;
		private void Awake() {
			_instance = UnityEngine.Object.FindObjectOfType(typeof(InputManager)) as InputManager;
			if (_instance == null) {
				// try to load prefab
				UnityEngine.Object managerPrefab = Resources.Load("InputManager"); // looks inside all 'Resources' folders in 'Assets'
				if (managerPrefab != null) {
					UnityEngine.Object prefab = Instantiate(managerPrefab);
					prefab.name = "InputManager"; // otherwise creates a game object with "(Clone)" appended to the name
				} else if (UnityEngine.Object.FindObjectOfType(typeof(InputManager)) == null) {
					// no prefab found, create new input manager
					GameObject gameObject = new GameObject("InputManager");
					gameObject.AddComponent<InputManager>();
					DontDestroyOnLoad(gameObject);
					gameObject.hideFlags = HideFlags.HideInHierarchy;
				}
				_instance = UnityEngine.Object.FindObjectOfType(typeof(InputManager)) as InputManager;
			}
		}

		public void Update() {
			// update the state of each control mapping
			foreach (ControlMapping m in ControlMappings.Mappings) {
				float controlVal = MidiMaster.GetKnob(Util.GetMidiChannel(m.channel), m.control) * 127;
				bool conditionMet = (controlVal > m.minVal) && (controlVal <= m.maxVal);

				m.keyDown = false;
				m.keyUp = false;
				if ((conditionMet) && (!m.conditionMet)) {
					m.keyDown = true; // first time condition is met - KeyDown event
				} else if ((!conditionMet) && (m.conditionMet)) {
					m.keyUp = true; // condition was met last update and now isn't - KeyUp event
				}
				m.conditionMet = conditionMet;
			}
		}

		public bool MapsKey(string key) {
			return KeyMappings.MapsKey(key) || ControlMappings.MapsKey(key);
		}

		public void MapKey(int trigger, string key, int channel) {
			KeyMappings.MapKey(trigger, key, channel);
		}

		public void MapControl(int control, int minVal, int maxVal, string key, int channel) {
			ControlMappings.MapControl(control, minVal, maxVal, key, channel);
		}

		public void RemoveMapping(int trigger, string key, int channel) {
			KeyMappings.RemoveMapping(trigger, key, channel);
		}

		public void RemoveMapping(int control, int minVal, int maxVal, string key) {
			ControlMappings.RemoveMapping(control, minVal, maxVal, key);
		}
		
		public static bool GetKey(string name) {
			if (name == "none") return false;

			if ((_instance != null) && _instance.MapsKey(name)) {
				// check if any key mappings are triggered
				bool keyTriggered = false;
				foreach (KeyMapping m in _instance.KeyMappings.GetMappings(name)) {
					if (MidiMaster.GetKey(Util.GetMidiChannel(m.channel), m.trigger) > 0.0f) {
						keyTriggered = true;
						break;
					}
				}

				// check if any control mappings are triggered
				bool controlTriggered = false;
				foreach (ControlMapping m in _instance.ControlMappings.GetMappings(name)) {
					if (m.conditionMet && !m.keyDown &!m.keyUp) {
						controlTriggered = true;
						break;
					}
				}
				
				return keyTriggered || controlTriggered || UnityEngine.Input.GetKey(name);
			} else {
				return UnityEngine.Input.GetKey(name);
			}
		}

		public static bool GetKey(KeyCode key) {
			return GetKey(key.ToString().ToLower());
		}

		public static bool GetKeyDown(string name) {
			if (name == "none") return false;

			if ((_instance != null) && _instance.MapsKey(name)) {
				// check if any key mappings are triggered
				bool keyTriggered = false;
				foreach (KeyMapping m in _instance.KeyMappings.GetMappings(name)) {
					if (MidiMaster.GetKeyDown(Util.GetMidiChannel(m.channel), m.trigger)) {
						keyTriggered = true;
						break;
					}
				}

				// check if any control mappings are triggered
				bool controlTriggered = false;
				foreach (ControlMapping m in _instance.ControlMappings.GetMappings(name)) {
					if (m.keyDown) {
						controlTriggered = true;
						break;
					}
				}

				return keyTriggered || controlTriggered || UnityEngine.Input.GetKeyDown(name);
			} else {
				return UnityEngine.Input.GetKeyDown(name);
			}
		}

		public static bool GetKeyDown(KeyCode key) {
			return GetKeyDown(key.ToString().ToLower());
		}

		public static bool GetKeyUp(string name) {
			if (name == "none") return false;

			if ((_instance != null) && _instance.MapsKey(name)) {
				// check if any key mappings are triggered
				bool keyTriggered = false;
				foreach (KeyMapping m in _instance.KeyMappings.GetMappings(name)) {
					if (MidiMaster.GetKeyUp(Util.GetMidiChannel(m.channel), m.trigger)) {
						keyTriggered = true;
						break;
					}
				}

				// check if any control mappings are triggered
				bool controlTriggered = false;
				foreach (ControlMapping m in _instance.ControlMappings.GetMappings(name)) {
					if (m.keyUp) {
						controlTriggered = true;
						break;
					}
				}
				
				return keyTriggered || controlTriggered || UnityEngine.Input.GetKeyUp(name);
			} else {
				return UnityEngine.Input.GetKeyUp(name);
			}
		}

		public static bool GetKeyUp(KeyCode key) {
			return GetKeyUp(key.ToString().ToLower());
		}
	}
}
using UnityEngine;
using System;
using System.Collections.Generic;

namespace UnityMidiControl.Input {
	[Serializable]
	public class KeyMappings {
		public List<KeyMapping> Mappings = new List<KeyMapping>();

		public void ClearMappings() {
			Mappings = new List<KeyMapping>();
		}

		public void RemoveMapping(int trigger, string key, int channel) {
			for (int i = Mappings.Count - 1; i >= 0; --i) {
				KeyMapping m = Mappings[i];
				if ((m.trigger == trigger) && (m.key == key) && (m.channel == channel)) {
					Mappings.RemoveAt(i);
					return; // if there are multiple mappings with the same settings, only the first will be removed
				}
			}
		}

		public void MapKey(int trigger, string key, int channel) {
			Mappings.Insert(0, new KeyMapping(trigger, key, channel));
		}

		public bool MapsKey(string key) {
			foreach (KeyMapping m in Mappings) {
				if (m.key == key) return true;
			}

			return false;
		}

		public List<KeyMapping> GetMappings(string key) {
			List<KeyMapping> mappings = new List<KeyMapping>();
			foreach (KeyMapping m in Mappings) {
				if (m.key == key) mappings.Add(m);
			}
			
			return mappings;
		}
	}

	[Serializable]
	public class KeyMapping {
		public int trigger; // note number (e.g., 60)
		public string key; // key activated (e.g., "x")
		public int channel; // MIDI channel
		
		public KeyMapping(int trigger, string key, int channel) {
			this.trigger = trigger;
			this.key = key;
			this.channel = channel;
		}
	}
}
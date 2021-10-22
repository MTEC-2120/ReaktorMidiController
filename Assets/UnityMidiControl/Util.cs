using UnityEngine;
using System;
using System.Collections.Generic;
using MidiJack;

namespace UnityMidiControl.Input {
	public sealed class Util {
		private static Dictionary<int, MidiChannel> CHANNEL_ENUM_MAPPINGS = new Dictionary<int, MidiChannel>() {
			{-1, MidiChannel.All},
			{1, MidiChannel.Ch1},
			{2, MidiChannel.Ch2},
			{3, MidiChannel.Ch3},
			{4, MidiChannel.Ch4},
			{5, MidiChannel.Ch5},
			{6, MidiChannel.Ch6},
			{7, MidiChannel.Ch7},
			{8, MidiChannel.Ch8},
			{9, MidiChannel.Ch9},
			{10, MidiChannel.Ch10},
			{11, MidiChannel.Ch11},
			{12, MidiChannel.Ch12},
			{13, MidiChannel.Ch13},
			{14, MidiChannel.Ch14},
			{15, MidiChannel.Ch15},
			{16, MidiChannel.Ch16},
		};

		public static MidiChannel GetMidiChannel(int channel) {
			MidiChannel mapping;
			try {
				mapping = CHANNEL_ENUM_MAPPINGS[channel];
			} catch (KeyNotFoundException) {
				mapping = MidiChannel.All;
			}

			return mapping;
		}
	}
}
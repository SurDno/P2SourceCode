﻿namespace SteamNative;

internal enum SNetSocketState {
	Invalid = 0,
	Connected = 1,
	Initiated = 10,
	LocalCandidatesFound = 11,
	ReceivedRemoteCandidates = 12,
	ChallengeHandshake = 15,
	Disconnecting = 21,
	LocalDisconnect = 22,
	TimeoutDuringConnect = 23,
	RemoteEndDisconnected = 24,
	ConnectionBroken = 25
}
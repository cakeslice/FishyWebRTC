WebRTC transport plugin for [FishNetworking](https://github.com/FirstGearGames/FishNet)

You can now use UDP in the browser with FishNetworking!

Based on [Bayou](https://github.com/FirstGearGames/Bayou) and [SimpleWebTransport](https://github.com/James-Frowen/SimpleWebTransport)

Limitations:
	- Only WebGL is supported for clients
	- There's no P2P support and clients cannot be hosts
	- Clients cannot be tested in Unity Editor, it only works in WebGL builds
	- HTTPS support is not implemented

Installation: 
	- Copy this folder to Assets/FishNet/Runtime/Transporting/Transports/
	- Add the FishyWebRTC transport to your TransportManager

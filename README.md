WebRTC transport plugin for [FishNetworking](https://github.com/FirstGearGames/FishNet)

You can now use UDP in the browser with FishNetworking!

Based on [Bayou](https://github.com/FirstGearGames/Bayou) and [SimpleWebTransport](https://github.com/James-Frowen/SimpleWebTransport)

Limitations:
- Only WebGL is supported for clients
  - You can use UDP on other platforms with a Multipass transport
- There's no P2P support and clients cannot be hosts
- The server can be built for most platforms but only Linux Dedicated Server build has been tested
- Clients cannot be tested in Unity Editor, it only works in WebGL builds
- HTTPS support for the initial connection (signaling) to the server is not implemented
  - You can still use HTTPS with a reverse proxy

Installation: 
- Add `https://github.com/cakeslice/com.unity.webrtc.git` using the Package Manager (Add from git URL)
  - This is a fork of `com.unity.webrtc` that has the compiled Linux binary that works with a Linux Dedicated Server build
- Copy this folder to `Assets/FishNet/Runtime/Transporting/Transports/`
- Modify the assembly definition `Assets/FishNet/Runtime/FishNet.Runtime.asmdef` to include the `SimpleWebRTC` and `Unity.WebRTC.Runtime` assemblies:
<img width="348" alt="assemblies" src="https://user-images.githubusercontent.com/7516142/233090203-4a787338-25a4-48a7-9fb2-5e57beac04c9.png">

- Add the `FishyWebRTC` transport component and use it in your `TransportManager`:
<img width="327" alt="example_component" src="https://user-images.githubusercontent.com/7516142/233031818-7ad0c782-9b98-4f0a-896a-8690de758f9b.png">


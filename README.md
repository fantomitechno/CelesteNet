Original [README](https://github.com/0x0ade/CelesteNet)
---
# CelesteNet (hosted)
> "Really adding hosted and this is the name of the fork?", me after searching for a real name

This branch (`hosted`) is a fork of CelesteNet only focused on modifying its server... and then there is the other branches that are here for me to contribute to main CelesteNet.

These features are mainly focused on content creation.

## Added features
### Commands
- `/vanish` (requires auth): hides you from all other players, you can still see their movements
- `/hostme` (requires auth): Sets you as an host
- `/hostgame` (requires auth): Sets the server in Host mode
- `/countdown [N]` (requires auth): sends a countdown as a server status (bottom right of the screen) to all players
- `/tpall` (requires auth): Teleports everyone to you
- `/state` (requires auth): Shows the state of the server (host mode: on/off, list of hosts, list of vanished)

### Host Mode
Host mode will make everyone invisible to everyone else except hosts that can see everyone and everything.

### Main server reliant Authentification
(jank but useful)\
When connecting to a CelesteNet server with a key (obtained by connecting to Discord), this key can only be use on the instance it was generated.

Keys generated on celestenet.0x0a.de can only be used on CelesteNet hosted at celestenet.0x0a.de.

This module makes it so you can use a key generated on celestenet.0x0a.de (or an other instance selected in server settings) on your instance.

This allows to switch from official CelesteNet server to one using this system flawlessly.

⚠️ When connecting to a server using this, you need to trust them, your key will navigate through their server **clear as glass**. This key can be used to impersonate you on the main CelesteNet server! If you ever think, you joined an untrusted server: visit https://celestenet.0x0a.de, connect and click `Revoke key`.

### Whitelist
A whitelist using Discord IDs... do not use the ingame commands, there is no paste in chat...

---

#### License notice

This project follows the original CelesteNet Licensing and stays under MIT.

The CelesteNet.Server.FrontendModule project/module uses [ImageSharp](https://github.com/SixLabors/ImageSharp/) under Apache 2.0 license terms per [Six Labors Split License 1.0](https://github.com/SixLabors/ImageSharp/blob/main/LICENSE)
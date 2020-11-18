Juggernaut Gamemode
======
## Description
Every player has a Com15 and a USP. The Com15 has a single bullet, but its ammo increases on kill. The USP has infinite ammo, but has an extremely short, configurable, kill range. Each player is playing to get a specific amount of points, or be the last one standing.

### Features
Optional integration with the GamemodeManager.

### Config Settings
Config option | Config Type | Default Value | Description
:---: | :---: | :---: | :------
is_enabled | bool | true | Whether or not this gamemode is availible on the server.
starting_broadcast_message | string | One In The Chamber. Starting in $time second(s) | The starting broadcast. The countdown is 5 seconds before the game starts. $time is the time until start.
elimination_broadcast_message | string | %user has been eliminated. $count players remain. | The elimination broadcast. %user is the user eliminated, $count is the number of players left.
max_duration | float | 600 | The maximum duration of the game in seconds.
respawn_time | float | 5 | The time to respawn in seconds.
score_to_win | int | 21 | The score required to win by score.
lives | int | 3 | The number of lives per player.
usp_kill_range | float | 4 | The kill range of the usp.
only_use_surface | bool | true | Whether or not to only use the surface.
disable_decontamination | bool | false | Whether or not to disable the decontamination sequence.
instant_decontamination | bool | false | Whether or not to instantly decontaminate the light containment zone. Disregarded if `disable_decontamination` is true.


### Commands
Command | Optional Arguments | | Description
:---: | :---: | :---: | :------
**Aliases** | **oneinthechamber** | **oitc**
(alias) enable | force, -f | ~~ | Enables the gamemode, starting on the next round start. If 'force' or '-f' is used, the gamemode will start instantly.
(alias) disable | force, -f | ~~ | Disables the gamemode after the current round. If 'force' or '-f' is used, the gamemode will end immediately, and it will attempt to start a new, standard round as best it can. (Decon and nuke timer/status will not be reset.)

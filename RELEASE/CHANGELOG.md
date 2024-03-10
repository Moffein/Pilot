`0.3.12`

- Fixed MOFFEIN_PILOT_BODY_OUTRO_FAILURE

`0.3.11`

- Cluster Fire
	- Reduced combo shot AoE from 11m -> 9m
	
- Suppressed Fire
	- Enabled damage falloff.
	
- Bombs Away!
	- Increased damage from 190% -> 220%
	- Increased AoE from 6m -> 9m
	
	*Damage increase should make this able to kill Lemurians/Beetles in 1 burst earlygame, and also accounts for the skill being harder to use than the default secondary.*

`0.3.10`

- Air Strike
	- Always Dash Backwards config option
		- Now dashes backwards even when stationary.

`0.3.9`

- Parachute
	- Now plays jump anim when exiting midair.

- Air Strike
	- Added option to make it only dash backwards like in Returns. (Disabled by default)
	- Now shows radius indicator.

- Aerial Support
	- Midair shorthop is now a config option. (Enabled by default)

`0.3.8`

- Cluster Fire
	- Piercing Version: Combo shot radius increased from 1 -> 1.5
	
- Fixed parachute visuals not showing for online clients.
- Air Strike markers now disappear after death.

`0.3.7`

- Cluster Fire
	- Added config option to choose between the current combo explosion and Returns' combo piercing laser. Can be changed in-game with Risk of Options.

`0.3.6`

- Cluster Fire
	- Fixed online bug where combo shot would apply upwards knockback when used after one of the Specials applied upwards knockback.
	- Increased shot radius from 0.2 -> 0.5
	- Fixed combo shot playing the Defense Matrix sound.
	- Combo shot no longer deals self damage with Chaos.
	- Fixed combo shot's internal raycast being able to proc.

`0.3.5`

- Fixed models for silenced pistol and regular primary being swapped.
- Lobby weapon model now changes based on selected primary.
- Fixed alt Air Strike placement targeting not checking for enemy bodies.

`0.3.4`
- Fixed internal version number.

`0.3.3`
  - Fixed Parachute linerenderers lagging behind the model.
  - Cluster Fire
	- AoE is reduced at point blank (11m -> 3m), and scales up to full at 11m.

`0.3.2`
  - Fixed gun in emote rig.
  - Updated r2api dependencies.

`0.3.1`
  - Fixed EntityStates not being registered. This should fix most MP bugs.
  - Fixed Special projectile visuals not showing online.
  - Emote support. (Currently a bit bugged, gun position will permanently change after emotes. Fix coming later.)

`0.3.0`
  - Release
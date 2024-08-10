`0.3.15`

- Added PT-BR TL (Thanks kauzok!)
- Thanks to Kaban for updating the RU TL as well!

- Rapid Deployment
	- Skill description now mentions the airborn/grounded difference.
	- Reduced height 25%
	
	*Old height was based on Ion Surge, while new height aims to be closer to the feel of Returns.*
	
- Aerobatics
	- Now grants +30% Attack Speed while wallclinging, lasts until you hit the ground.

`0.3.14`

- Updated RU TL
- Added skill icons (Thanks Leaf_It!)
	- Some skills still need icons.
	
- Rapid Fire
	- Reduced spread per shot from 0.5 to 0.3
	- Tweaked tracer VFX.

- Target Acquired
	- Changed tracer VFX.

- Air Strike
	- Increased trigger radius from 6m to 10m to match the explosive radius.
	- Fixed the trigger radius indicator visual being 50% smaller than the actual radius.
	
- Aerial Support
	- Reduced shorthop velocity from 24 -> 6
	
	*Having a full hop on this felt weird. New velocity will make it simply break your fall.*

`0.3.13`

- Added RU TL (Thanks Lecard!)

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
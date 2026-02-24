`0.6.4`

- Added Chinese translation. (Thanks guoba00874!)

`0.6.3`

- Added Korean translation. (Thanks Dice-001!)

`0.6.2`

- Dithering is fixed in basegame, so Parachute shader has been swapped back to HGStandard.

	*Now that Operator is out, I'd like to take some time to review Pilot's gameplay. What lessons can be leanred from Operator, what aspects can separate him from Operator, etc.*

`0.6.1`

- Fixed misnamed material from 0.6.0

`0.6.0`

- Added updated skill icons from Leaf_it.
- Cluster Fire
	- Reduced blast radius from 3-9m -> 2.5-8m
	
- Rapid Fire
	- Now is Agile.
	
- Suppressed Fire
	- Hitting enemies below 50% HP now shows an extra impact effect + damage color.
	
- Bombs Away
	- Fixed wallclinging sometimes counting as being grounded.
	
- Rapid Deployment
	- Now forces sprinting during state.
	- Added a small forwards boost when exiting Parachute.
	- Added a temporary fix for the parachute not fading.
		- Uses Unity's standard shader instead of the hopoo shader so it might look a bit jank.

`0.5.2`

- Oops.

`0.5.1`

- Removed dontAllowPastMaxStocks check
- Drone no longer spawns in hidden realms

`0.5.0`

- Reduced size 12.5% to be around Commando-sized.
- Adjusted camera params to be less zoomed-out, so he won't look awkwardly small anymore.
- Increased hitbox size to be closer to vanilla survivors.

*Always was bugged by how he both looked small in the camera while also being really tall for some reason.*

- Bombs Away
	- While grounded, shoots a single projectile for 480% damage with no drop and 2x travel speed.
	- Airborne behavior works as before, 3x220% with drop and low speed.
	
*Trying to lean into the grounded/airborne kit differences that Returns has. How does this feel?*

- Added drone to skybox.

`0.4.0`

- Fixed for latest update.
- Fixed Luminous Shot interaction.
- Target Acquired
		- Damage reduced from 3x190% -> 3x160%
		- Gains a 50% damage bonus against airborne enemies (3x240% damage total)
		
		*Encouraging comboing this with the Special.*

`0.3.18`

- Fixed nullref when dashing.

`0.3.17`

- UpdatedTakeDamage hook to TakeDamageProcess.

`0.3.16`

- DLC2 fix.

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
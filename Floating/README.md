## Floatation

Changes gravity so that lighter objects will float in liquid instead of sink.

### Mechanic Properties
- Objects can float in any liquid.
- All objects that have the Pickupable tag and component will be eligible to float (including modded objects), provided they do not fall under an exception
- Objects will only float if its max mass is less than the max mass of the liquid element it is trying to float in.
- Objects will choose a random horizontal direction to move in and bounce off the walls which provides a mechanism for movement.
- Objects will be submerged but remain close to the surface of the liquid.
- Bottled liquids and gasses will always float.

Objects do NOT float under the following conditions:
- They are a creature or creature egg
- They are a Duplicant (but Duplicant corpses will float)
- The liquid is too shallow
- The object element's default or max mass is greater or equal to the liquid's max mass

### Uses
- Collect items that have fallen into water more easily
- Ferry slime and algae naturally towards a pickup station
- Collect fish meat from the liquid surface

May have interesting synergies with other mods and future content. Please share other creative ideas and mechanisms.

### Known Bugs
- Objects in shallow water that attach to the floor may be significantly offset horizontally
- Objects can sometimes be jittery when it is close to the ground or moving to different height levels
- Occasionally spazzes out when moving up into mesh or any solid tile ceiling

### Potential Bugs
I have not tested extremely tight 1-cell areas or other unnatural situations.

### Future Plans
- Make items move with bias from higher to lower levels of liquid (pretend that it is the flow of liquid). (Idea from Hanmac)

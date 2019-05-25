## Floatation

Changes gravity so that lighter objects will float in liquid instead of sink.

### Mechanic Properties
- Objects can float in any liquid.
- All objects that have the Pickupable tag and component will be eligible to float (including modded objects), provided they do not fall under an exception
- Objects will only float if it weighs less than 1/4 the weight of liquid in the cell. Denser liquids can float heavier objects.
- Objects will choose a random horizontal direction to move in and bounce off the walls which provides a mechanism for movement.
- Objects will be submerged but remain close to the surface of the liquid.

Objects do NOT float under the following conditions:
- They are a creature or creature egg
- They are a Duplicant (but Duplicant corpses will float)
- The liquid is too shallow
- The object is too heavy

### Uses
- Collect items that have fallen into water more easily
- Ferry slime and algae naturally towards a pickup station
- Collect fish meat from the liquid surface

May have interesting synergies with other mods and future content. Please share other creative ideas and mechanisms.

### Known Bugs
- Objects in shallow water that attach to the floor may be significantly offset horizontally
- Occasionally some objects might not leave the ground, i.e. corpse on the bottom of a pool
- Objects can sometimes float above the water surface depending on whether the surface level is closer to snapping to a tile or not, as well as the object's hitbox
- Objects can sometimes be jittery when it is close to the ground or moving to different height levels

### Potential Bugs
Things that I didn't test yet:
- Objects in a 1-cell width area and other more extreme situations.

### Future Plans
- Make items move with bias from higher to lower levels of liquid (pretend that it is the flow of liquid). (Idea from Hanmac)

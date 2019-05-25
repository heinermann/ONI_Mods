## Floatation

Changes gravity so that lighter objects will float in liquid instead of sink.

### Mechanic Properties
- Objects float in any liquid.
- All objects that have the Pickupable tag will float (including modded ones), except for anything tagged with Minion or Creature (unless it is also tagged with Corpse).
- Objects will only float if it weighs less than 1/4 the weight of liquid in the cell. That is, denser liquids can float heavier objects.
- Objects will choose a random direction to move in and bounce off the walls which provides a mechanism of movement.
- Objects will be submerged but remain close to the surface of the liquid.

### Uses
- Collect items that have fallen into water more easily
- Ferry slime and algae naturally towards a pickup station
- Collect fish meat from the liquid surface

May have interesting synergies with future mods and content.

### Known Bugs
- Objects stuck to the floor will not leave it.
- objects in shallow water that attach to the floor may be offset horizontally
- Duplicants that die on the bottom of water will have their bodies remain at the bottom

### Potential Bugs
Things that I didn't test yet:
- Objects in a 1-cell width area.

### Future Plans
- Make items move with bias from higher to lower levels of liquid (pretend that it is the flow of liquid). (Idea from Hanmac)

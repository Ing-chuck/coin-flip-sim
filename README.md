# coin-flip-sim
statistics simulation for coin flipping in unity3D (how many times the coin lands on side depending on coin thickness)

## Features

### Single simulation

with a user set amount of coins and coin thickness.

### Benchmarking

the user can select from 2 to 10 intervals, the simulation is repeated the selected number of intervals with varying thincknesses and a graph is displayed showing the number of coins that landed on the side by thickness.

A file is generated containing the number of coins that landed on the side with the corresponding thickness.

### Searching

Performs many simulations to search for the thickness that makess the coin as fair as possible (fair meaning it has equal chances of landid on its side as it has of landing on either face). The user can set the starting thickness, the maximum thickness, the increment size and the number of times the simulation will be repeated before increasing the thickness (if more than once, the final calculation will be made with the average of all simulations for that thickness).

A file is generated with the number of heads, tails, sides and the z-score for each thickness. The z-score represents statiscally how close the simulation was to an ideal result, the smaller z the better, the program also reports the best thickness found during the run.

**all simulations can be stopped by the user at any time.**

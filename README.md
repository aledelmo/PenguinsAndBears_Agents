# Penguins & Bears - MLAgents
North Pole Madness using Deep Reinforcement Learning

![gif](https://i.imgur.com/GVG4bAS.gif)

A penguin must feed a baby. A bear must eat. And they play all by themselves.
Short game implemented in [Unity3D](https://unity.com) using the
[ML Agents](https://github.com/Unity-Technologies/ml-agents) library for deep reinforcement training. The
penguin is rewarded when he picks up a fish and when it reach the baby.

Trained on CPU. Making multiple copies of the game arena can greatly speed up the process,
allow for parallel training (suggested >8 arenas). Activate heuristic mode to manually control the penguin agent.

Initial positions randomly initialized.

- Penguin
    - Pick a fish
    - Avoid the bear
    - Feed the baby
- Bear
    - EAT THE PENGUIN !!!

Game finishes when there are no fish remaining in the pool or when the bear eat the penguin.

## System Requirements

Unity3D 2019.4.18

ML-Agents 1.7.2

Barracuda 1.3.1

## Contacts

For any inquiries please contact:
[Alessandro Delmonte](https://aledelmo.github.io) @ [alessandro.delmonte@institutimagine.org](mailto:alessandro.delmonte@institutimagine.org)

## License

This project is licensed under the [Apache License 2.0](LICENSE) - see the [LICENSE](LICENSE) file for
details
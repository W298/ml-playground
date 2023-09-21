# ml-playground

Test & Implement agents in various environment using unity ml-agents.  

Each environment is implemented as a submodule and can be found in the sections below.  
Policies, hyperparameters, and scripts are described in the blog post.

## List of Environments

| Env                                       | Detail Description                                                 | Submodule Repo                                                       |
| ----------------------------------------- | ------------------------------------------------------------------ | -------------------------------------------------------------------- |
| [Knight Battle](#knight-battle)           | [mlagent_09](https://w298.dev/posts/mlagent_09)                    | [ml-playground-battle](https://github.com/W298/ml-playground-battle) |
| [Domination](#domination)                 | [mlagent_08](https://w298.dev/posts/mlagent_08)                    | -                                                                    |
| [Predator and Prey](#predator-and-prey)   | [mlagent_07](https://w298.dev/posts/mlagent_07)                    | -                                                                    |
| [Obstacle Avoidance](#obstacle-avoidance) | [mlagent_01 ~ 06](https://w298.dev/posts/series/rl-obstacle-avoid) | -                                                                    |

### Knight Battle

<img width="550" alt="knight-battle" src="https://github.com/W298/ml-playground/assets/25034289/0a28d0d2-9623-4291-bbbc-51ecdd199152" />

- **Train agent to do melee combat (1 vs 1, many vs many) in limited area.**
- Learning with MA-POCA, self-play (co-op & competitive)
- Agents can use sword to take attack or use shield to block attack.
- Also, you can play with trained agents. [Beat them out!](https://github.com/W298/ml-playground-battle/releases)
- Repository : [ml-playground-battle](https://github.com/W298/ml-playground-battle)
- Detail Description : [Blog Post](https://w298.dev/posts/mlagent_09)

### Domination

<img width="550" alt="domination" src="https://github.com/W298/ml-playground/assets/25034289/a9da4e9e-6f0f-4e68-9f24-ecf3bc9e93ce" />

- **Train agent to fill tiles with its color for win.**
- Learning with MA-POCA, self-play
- Each time the agent moves, it paints a tile with its color.
- When the closed path is complete, all of the interior tiles are painted with its color.
- Detail Description : [Blog Post](https://w298.dev/posts/mlagent_08)

### Predator and Prey

<img width="550" alt="domination" src="https://github.com/W298/ml-playground/assets/25034289/94145ae1-6098-40b1-a644-0c7f7958382e" />

- **Train Predator to catch Prey, Train Prey to escape from Predator.**
- Predator wins if it catches Prey, and Prey wins if it alive at the end of time.
- Learning with MA-POCA (Multi-Agent Posthumous Credit Assignment), self-play
- Detail Description : [Blog Post](https://w298.dev/posts/mlagent_07)

### Obstacle Avoidance

<img width="480" alt="domination" src="https://github.com/W298/ml-playground/assets/25034289/db666d73-2471-4752-b115-d65e842fbb29" />
<img width="400" alt="domination" src="https://github.com/W298/ml-playground/assets/25034289/29d60b18-9450-4171-85ac-774c720ad9ba" />

- **Train agent to avoid obstacles and reach the goal in various environments.**
- The preview above is a snippet, the full list is at [here](https://w298.dev/posts/series/rl-obstacle-avoid)


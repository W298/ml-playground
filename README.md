# ml-playground

Test & Implement agents in various environment using unity ml-agents.  

Each environment is implemented as a submodule and can be found in the sections below.  
Policies, hyperparameters, and scripts are described in the blog post.

## List of Environments

### Knight Battle

<img width="500" alt="knight-battle" src="https://github.com/W298/ml-playground/assets/25034289/0a28d0d2-9623-4291-bbbc-51ecdd199152" />

- Learning close combat (1 vs 1, many vs many) in limited area.
- Repository : [ml-playground-battle](https://github.com/W298/ml-playground-battle)
- Detail Description : [Blog Post](https://w298.dev/posts/mlagent_09)

### Domination

- Fill the tile with its color to win.
- Each time the agent moves, it paints a tile with its color.
- When the closed path is complete, all of the interior tiles are painted with its color.
- Detail Description : [Blog Post](https://w298.dev/posts/mlagent_08)

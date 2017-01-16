#PROJECT COLOSSALUM - DESIGN DOCUMENT

This document details the background, concept and game mechanics of the Colossalum project.

##PROJECT BACKGROUND
This game project is created using the Unity Engine as part of my Digital Media Arts 299 study at Bellevue College under instructor Christopher Orth. DMA299 is an independent study, so I am developing on my own, managing my time as I go.
The prototype was coincidentally built march 2016, just before I traveled to Seattle, seeing as I did not know I would use the project for school before departure. The period of development lasts from 4th or April until the 20th of June, and seeing as I already had the prototype handy, I thought to use the project for my study.

###Prototype
The prototype consists of a sword-wielding snowman facing a helpless dummy, which wobbles when hit. As an experiment, I created a skeleton that crumbles when struck, with each individual bone capable of falling to the ground on it’s own. The skeleton will reanimate if struck when “dead”, allowing for the process to repeat indefinitely.

###Vision
For a very long time I have wanted to create video games. I have had several projects, but this is the first time I get a significant, allotted amount of time to finish a game. As a result of that, I am drawing inspiration from games of some of my favorite genres: hack and slash, action rpg, and roguelike with fantasy themed sprinkles. Diablo and Skyrim are great sources of inspiration for this particular title. I wanted the game to end up with fighting mechanics inspired by the vast open-world rpg, while I think the environment and itemization should take more after the devilish franchise.

##GAME FEATURES
Extensive descriptions of all core game mechanics and how to play game. 
Gameplay Loop
The player starts off in a cave equipped with a dagger. Leaving the cave is possible by two options: towards the arena or the forest. Going to the forest, the player has the opportunity to fight enemies and collect new weapons. Entering the arena is only possible by putting a weapon onto the shrine in front of the entrance. The dagger is very much eligible for this action, but entering the forest and acquiring more powerful weapons is the only way to progress through the game. Returning from the forest is done by consuming a teleport gem...

###Controls
The player can steer the character using the camera and movement input as per ancient video game convention. Invert the camera’s y axis by pressing “i” on the keyboard. Sprinting and jumping are fairly self-explanatory.
“Attacking” and “consuming” are the main actions the player can take at will and are their primary means of acting upon the world. Holding “throw” while engaging either of these will toss away the item in the respective hand.

**Action: M/K / controller.**
- Movement: WASD / left stick
- Camera (i to invert): mouse / right stick
- Attack (right arm): left mouse / right trigger
- Consume (left arm): right mouse / left trigger
- Throw: Q / Y
- Sprint: left shift / left shoulder
- Jump: space / A

###Combat & Weapons
The player gets to use a variety of different melee attacks, which are determined by the equipped weapon. To unlock a weapon, the player first goes to the forest to procure the item. They then find their way back to the cave to access the arena using the weapon. Defeating the battle unlocks that weapon.
The player may only carry one weapon at any given time, but is able to toss their weapon at will, to pick up another weapon in it’s stead. The damaging effect of strikes dealt towards susceptible targets is determined on a 5-step scale:

0. Zero effect.
1. Light damage.
2. Medium damage.
3. Heavy damage.
4. Devastation.

##COLOSSALICON
Details on the content of the game.

###The World
The world has 3 key areas, which tie the gameplay loop together: The cave, the arena, and the forest.

- Cave - The cave is the hub of the game. Here the player is introduced to attacking dummies and putting weapons on the shrine, which is part of furthering the progress of the game. Also, the player will have access to all unlocked weapons from here.
- Arena - Here the player fights different sets of monsters depending on which weapon has been put on the shrine. Defeating the battle unlocks that weapon, and takes the player back to the cave.
- Forest - The forest is a randomly generated area where the main part of the gameplay loop takes place. The player will encounter monsters and pick up weapons and loot here. Each random layout of the forest has a portal, which leads into a new and slightly bigger forest. Eventually the player will be able to escape with a weapon, and attempt to unlock it in the arena.

###Bestiary
Creatures in the world. Most of them just want to kill you. Monsters are randomly generated throughout the forest.

- Skeleton - Boney enemy with a variety of weapons. Will follow and attack the player. Every strike will scatter some of it’s bones, and eventually not enough will be left to keep it standing.
- Goblin - Small mob with a variety of aesthetic equipment. Engages and attacks player in range. Runs fast and stabs rapidly. May lose a few limbs. Also bleeds.
- Hooman - Cheering bystander. Very susceptible to damage!

###Designer’s Thoughts
Some spiking or recurring thoughts and ideas I’ve had throughout the project.

- I decided to utilize only the structures within Unity for building and modeling. Everything in the game is built using Unity’s primitive geometry.
- Meshes were destroying memory, and so I opted to swap all detailed wilderness (trees in between trees) out with big, simple rocks. 
- Thought of creating flat, spatula-like fingers for performance optimization, but the artist in me would not let me do it.
- One of the tricky parts of building a game around the combat of a character is, that eventually you will have to strip away a huge chunk of that characters power, without taking the fun out of the game.
- Being a one-man-team, I tend to pick the task with lowest implementation complexity per feature increment. This way I can focus on expanding the game in a forward fashion as opposed being sidetracked with implementing nifty little features that does not add to the overall scope of the game.
- The player can see the entire character they’re playing!
- A funny thing about random damage scales and random outfit generation is, that people may misconstrue them as being connected by some logic. They kill a naked goblin and have trouble with an armored one, and instantly there’s a connection between armor and damage. Convincing someone of the opposite is more difficult, so this is a cheap illusion of scaling armor quality - By making the models and just using Random.value.
